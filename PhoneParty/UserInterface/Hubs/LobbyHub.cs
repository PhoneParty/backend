using Domain;
using Domain.WhoAmI;
using Infrastructure;
using Microsoft.AspNetCore.SignalR;
using PhoneParty.Domain;
using PhoneParty.Hubs.UserInterface.Interfaces;
using PhoneParty.Hubs.UserInterface.Interfaces.Repositories;

namespace UserInterface.Hubs;

public class LobbyHub : Hub
{
    // Словарь для хранения участников по ID лобби
    private readonly IRepository<LobbyId, Lobby> LobbyRepository;
    private readonly IRepository<string, WebApplicationUser> UserRepository;

    public LobbyHub(IRepository<LobbyId, Lobby> lobbyRepository, IRepository<string, WebApplicationUser> userRepository)
    {
        LobbyRepository = lobbyRepository;
        UserRepository = userRepository;
        var ids = UserIdGenerator.GetAllOccupiedIds();
        foreach (var id in ids)
        {
            if(!UserRepository.Contains(id))
                UserRepository.Add(id, new WebApplicationUser(id));
        }
    }
        
    public async Task CheckHost(string userId, string lobbyId)
    {
        var lobby = LobbyRepository.Get(new LobbyId(lobbyId));
        await Clients.Caller.SendAsync("IsHost", lobby.Host.Id == userId);
    }

    public async void RegisterUser()
    {
        var id = UserIdGenerator.GenerateUserId();
        var user = new WebApplicationUser(id);
        user.SetConnection(Context.ConnectionId);
        UserRepository.Add(id, user);
        Console.WriteLine($"reg {id}");
        await Clients.Caller.SendAsync("UserCreated", id);
    }

    public async void UpdateUserConnection(string id)
    {
        var user = UserRepository.Get(id);
        user.SetConnection(Context.ConnectionId);
        Console.WriteLine("Update connection: " + Context.ConnectionId);
    }

    public async void UpdateGroupConnection(string userId, string lobbyIdString)
    {
        var user = UserRepository.Get(userId);
        await Groups.RemoveFromGroupAsync(user.ConnectionId, lobbyIdString);
        user.SetConnection(Context.ConnectionId);
        await Groups.AddToGroupAsync(user.ConnectionId, lobbyIdString);
    }

    public async void UpdateUserName(string id, string name)
    {
        var user = UserRepository.Get(id);
        user.SetName(name);
    }

    public async Task UpdateLobby(string lobbyIdString)
    {
        var lobby = LobbyRepository.Get(new LobbyId(lobbyIdString));
        await Clients.Group(lobbyIdString).SendAsync("UpdateLobbyUsers", GetLobbyUsers(lobbyIdString), UserRepository.Get(lobby.Host.Id));
    }

    public async Task StartGame(string lobbyId)
    {
        var lobby = LobbyRepository.Get(new LobbyId(lobbyId));
        lobby.ChangeGame(new WhoAmIGame());
        lobby.StartGame();
        await Clients.Group(lobbyId).SendAsync("GameStarted");
    }

    public async Task CreateLobby(string userId)
    {
        var lobbyId = LobbyIdGenerator.GenerateLobbyId();
        // while (!LobbyRepository.Contains(new LobbyId(lobbyId)))
        //     lobbyId = RandomIds.GenerateUserId();
        
        var user = UserRepository.Get(userId);
        user.SetName(user.UserName);

        var lobby = new Lobby(lobbyId, user.Player);
        LobbyRepository.Add(lobbyId, lobby);
        await Groups.AddToGroupAsync(user.ConnectionId, lobbyId.ToString());

        // Уведомляем пользователя, что лобби создано
        await Clients.Caller.SendAsync("LobbyCreated", lobbyId.ToString());
    }

    public async Task JoinLobby(string lobbyIdString, string userId)
    {
        var newLobbyId = new LobbyId(lobbyIdString);
        if (!LobbyRepository.Contains(newLobbyId))
            return;
        var user = UserRepository.Get(userId);
        var lobby = LobbyRepository.Get(newLobbyId);
        lobby.RegisterPlayer(user.Player);
        await Groups.AddToGroupAsync(user.ConnectionId, lobbyIdString);
        
        await Clients.Caller.SendAsync("LobbyJoinAccept", lobbyIdString, user.ConnectionId);

        // Уведомляем всех в группе о новом участнике
        await Clients.Group(lobbyIdString).SendAsync("UpdateLobbyUsers", GetLobbyUsers(lobbyIdString), UserRepository.Get(lobby.Host.Id));
    }

    public async Task LeaveLobby(string userId ,string lobbyIdString)
    {
        var newLobbyId = new LobbyId(lobbyIdString);
        if (LobbyRepository.Contains(newLobbyId))
        {
            var user = UserRepository.Get(userId);
            var lobby = LobbyRepository.Get(newLobbyId);
            var lastHost = lobby.Host;
            lobby.KickPlayer(user.Player);
            if (!Equals(lastHost, lobby.Host))
            {
                await Clients.Client(UserRepository.Get(lobby.Host.Id).ConnectionId).SendAsync("IsHost", true);
            }
            await Groups.RemoveFromGroupAsync(user.ConnectionId, lobbyIdString);

            if (lobby.PlayersCount == 0)
                LobbyRepository.Remove(newLobbyId);
            else
                await Clients.Group(lobbyIdString).SendAsync("UpdateLobbyUsers", GetLobbyUsers(lobbyIdString), UserRepository.Get(lobby.Host.Id));
        }
    }

    private List<WebApplicationUser> GetLobbyUsers(string lobbyIdString)
    {
        var res = LobbyRepository
            .Get(new LobbyId(lobbyIdString))
            .GetPlayers
            .Select(x => UserRepository.Get(x.Id))
            .ToList();
        return res;
    }
}