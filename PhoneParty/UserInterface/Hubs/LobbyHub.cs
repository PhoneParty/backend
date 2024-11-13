using Domain;
using Infrastructure;
using Microsoft.AspNetCore.SignalR;
using PhoneParty.Domain;
using PhoneParty.Hubs.Infastructure;
using PhoneParty.Hubs.UserInterface.Interfaces;
using PhoneParty.Hubs.UserInterface.Interfaces.Repositories;

namespace PhoneParty.UserInterface.Hubs;

public class LobbyHub : Hub
{
    // Словарь для хранения участников по ID лобби
    private readonly IRepository<Domain.LobbyId, Lobby> LobbyRepository;
    private readonly IRepository<string, User> UserRepository;

    public LobbyHub(IRepository<Domain.LobbyId, Lobby> lobbyRepository, IRepository<string, User> userRepository)
    {
        LobbyRepository = lobbyRepository;
        UserRepository = userRepository;
        foreach (var id in UserIdRepo.GetAllIds())
        {
            var user = new User(id);
            if(!UserRepository.Contains(id))
                UserRepository.Add(id, user);
        }
    }

    public async void RegisterUser()
    {
        var id = UserIdRepo.GenerateUserId();
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

    public async void UpdateGroupConnection(string userId, string lobbyId)
    {
        var user = UserRepository.Get(userId);
        await Groups.RemoveFromGroupAsync(user.ConnectionId, lobbyId);
        user.SetConnection(Context.ConnectionId);
        await Groups.AddToGroupAsync(user.ConnectionId, lobbyId);
    }

    public async void UpdateUserName(string id, string name)
    {
        var user = UserRepository.Get(id);
        user.SetName(name);
    }

    public async Task UpdateLobby(string lobbyId)
    {
        await Clients.Group(lobbyId).SendAsync("UpdateLobbyUsers", GetLobbyUsers(lobbyId));
    }

    public async Task CheckHost(string userId, string lobbyId)
    {
       var lobby = LobbyRepository.Get(new Domain.LobbyId(lobbyId));
       await Clients.Caller.SendAsync("IsHost", lobby.Host.Id == userId);
    }

    public async Task CreateLobby(string userId)
    {
        var lobbyId = LobbyIdRepo.GetLobbyId();
        // while (!LobbyRepository.Contains(new LobbyId(lobbyId)))
        //     lobbyId = RandomIds.GenerateUserId();
        
        var user = UserRepository.Get(userId);
        user.SetName(user.UserName + '*');

        var lobby = new Lobby(new Domain.LobbyId(lobbyId), user.Player);
        LobbyRepository.Add(new Domain.LobbyId(lobbyId), lobby);
        await Groups.AddToGroupAsync(user.ConnectionId, lobbyId);

        // Уведомляем пользователя, что лобби создано
        await Clients.Caller.SendAsync("LobbyCreated", lobbyId);
    }

    public async Task JoinLobby(string lobbyId, string userId)
    {
        var newLobbyId = new Domain.LobbyId(lobbyId);
        if (!LobbyRepository.Contains(newLobbyId))
            return;
        var user = UserRepository.Get(userId);
        LobbyRepository.Get(newLobbyId).RegisterPlayer(user.Player);
        await Groups.AddToGroupAsync(user.ConnectionId, lobbyId);
        
        await Clients.Caller.SendAsync("JoinedToLobby", lobbyId, user.ConnectionId, GetLobbyUsers(lobbyId));

        // Уведомляем всех в группе о новом участнике
        await Clients.Group(lobbyId).SendAsync("UserJoined", user.ConnectionId, GetLobbyUsers(lobbyId));
    }

    public async Task LeaveLobby(string userId ,string lobbyId)
    {
        var newLobbyId = new Domain.LobbyId(lobbyId);
        if (LobbyRepository.Contains(newLobbyId))
        {
            var user = UserRepository.Get(userId);
            var lobby = LobbyRepository.Get(newLobbyId);
            lobby.KickPlayer(user.Player);
            await Groups.RemoveFromGroupAsync(user.ConnectionId, lobbyId);

            if (lobby.PlayersCount == 0)
            {
                LobbyIdRepo.RestoreId(lobbyId);
                LobbyRepository.Remove(newLobbyId);
            }
            else
                await Clients.Group(lobbyId).SendAsync("UserLeft", GetLobbyUsers(lobbyId));
        }
    }

    private List<string> GetLobbyUsers(string lobbyId)
    {
        var res = LobbyRepository
            .Get(new Domain.LobbyId(lobbyId))
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