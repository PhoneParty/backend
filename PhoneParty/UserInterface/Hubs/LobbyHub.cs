using Domain;
using Infrastructure;
using Microsoft.AspNetCore.SignalR;
using PhoneParty.Domain;
using PhoneParty.Hubs.UserInterface.Interfaces;
using PhoneParty.Hubs.UserInterface.Interfaces.Repositories;

namespace PhoneParty.Hubs;

public class LobbyHub : Hub
{
    // Словарь для хранения участников по ID лобби
    private readonly IRepository<LobbyId, Lobby> LobbyRepository;
    private readonly IRepository<string, User> UserRepository;

    public LobbyHub(IRepository<LobbyId, Lobby> lobbyRepository, IRepository<string, User> userRepository)
    {
        LobbyRepository = lobbyRepository;
        UserRepository = userRepository;
        var ids = UserIdGenerator.GetAllIds();
        foreach (var id in ids)
        {
            var user = new User(id);
            if(!UserRepository.Contains(id))
                UserRepository.Add(id, user);
        }
    }

    public async void RegisterUser()
    {
        var id = UserIdGenerator.GenerateUserId();
        // while (!UserRepository.Contains(id))
        //     id = RandomIds.GenerateUserId();
        var user = new User(id);
        user.SetConnection(Context.ConnectionId);
        UserRepository.Add(id, user);
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
        await Clients.Group(lobbyIdString).SendAsync("UpdateLobbyUsers", GetLobbyUsers(lobbyIdString));
    }

    public async Task CreateLobby(string userId)
    {
        var lobbyId = LobbyIdGenerator.GenerateLobbyId();
        // while (!LobbyRepository.Contains(new LobbyId(lobbyId)))
        //     lobbyId = RandomIds.GenerateUserId();
        
        var user = UserRepository.Get(userId);
        user.SetName(user.UserName + '*');

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
        LobbyRepository.Get(newLobbyId).RegisterPlayer(user.Player);
        await Groups.AddToGroupAsync(user.ConnectionId, lobbyIdString);
        
        await Clients.Caller.SendAsync("JoinedToLobby", lobbyIdString, user.ConnectionId, GetLobbyUsers(lobbyIdString));

        // Уведомляем всех в группе о новом участнике
        await Clients.Group(lobbyIdString).SendAsync("UserJoined", user.ConnectionId, GetLobbyUsers(lobbyIdString));
    }

    public async Task LeaveLobby(string userId ,string lobbyIdString)
    {
        var newLobbyId = new LobbyId(lobbyIdString);
        if (LobbyRepository.Contains(newLobbyId))
        {
            var user = UserRepository.Get(userId);
            var lobby = LobbyRepository.Get(newLobbyId);
            lobby.KickPlayer(user.Player);
            await Groups.RemoveFromGroupAsync(user.ConnectionId, lobbyIdString);

            if (lobby.PlayersCount == 0)
                LobbyRepository.Remove(newLobbyId);
            else
                await Clients.Group(lobbyIdString).SendAsync("UserLeft", GetLobbyUsers(lobbyIdString));
        }
    }

    private List<string> GetLobbyUsers(string lobbyIdString)
    {
        var res = LobbyRepository
            .Get(new LobbyId(lobbyIdString))
            .GetPlayers
            .Select(x => UserRepository.Get(x.Id).UserName)
            .ToList();
        return res;
    }

    // public override async Task OnDisconnectedAsync(Exception exception)
    // {
    //     foreach (var lobby in Lobbies.Lobbies)
    //     {
    //         if (lobby.Value.Contains(Context.ConnectionId))
    //         {
    //             await LeaveLobby(lobby.Key);
    //             break;
    //         }
    //     }
    //     await base.OnDisconnectedAsync(exception);
    // }
}