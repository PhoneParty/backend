using Microsoft.AspNetCore.SignalR;
using PhoneParty.Hubs.Infastructure;
using PhoneParty.Hubs.UserInterface.Interfaces;
using PhoneParty.Hubs.UserInterface.Interfaces.Repositories;

namespace PhoneParty.Hubs;

public class LobbyHub : Hub
{
    // Словарь для хранения участников по ID лобби
    private readonly IMemoryRep<User> Lobbies;
    private readonly IRepository<string, User> UserRepository;

    public LobbyHub(IMemoryRep<User> memoryRep, IRepository<string, User> userRepository)
    {
        Lobbies = memoryRep;
        UserRepository = userRepository;
        var ids = new string[] { "5132b2b4-b15d-4475-b5d0-55e2f8f0f28e", "ec5be520-2704-4567-8c5f-e39e955e1541" };
        foreach (var id in ids)
        {
            var user = new User(id);
            if(!UserRepository.Contains(id))
                UserRepository.Add(id, user);
        }
    }

    public async void RegisterUser(string id)
    {
        var user = new User(id);
        user.SetConnection(Context.ConnectionId);
        UserRepository.Add(id, user);
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

    public async Task CreateLobby(string userId)
    {
        var lobbyId = new Random().NextString();
        
        if (!Lobbies.Contains(lobbyId))
            Lobbies.AddValue(lobbyId, []);

        var user = UserRepository.Get(userId);
        user.SetName(user.UserName + '*');
        await Groups.AddToGroupAsync(user.ConnectionId, lobbyId);
        Lobbies.GetValue(lobbyId).Add(user);

        // Уведомляем пользователя, что лобби создано
        await Clients.Caller.SendAsync("LobbyCreated", lobbyId);
    }

    public async Task JoinLobby(string lobbyId, string userId)
    {
        if (!Lobbies.Contains(lobbyId))
            return;
        var user = UserRepository.Get(userId);
        await Groups.AddToGroupAsync(user.ConnectionId, lobbyId);
        Lobbies.GetValue(lobbyId).Add(user);
        
        await Clients.Caller.SendAsync("JoinedToLobby", lobbyId, user.ConnectionId, GetLobbyUsers(lobbyId));

        // Уведомляем всех в группе о новом участнике
        await Clients.Group(lobbyId).SendAsync("UserJoined", user.ConnectionId, GetLobbyUsers(lobbyId));
    }

    public async Task LeaveLobby(string userId ,string lobbyId)
    {
        if (Lobbies.Contains(lobbyId))
        {
            var user = UserRepository.Get(userId);
            Lobbies.GetValue(lobbyId).Remove(user);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyId);

            if (Lobbies.GetValue(lobbyId).Count == 0)
                Lobbies.Remove(lobbyId);
            else
                await Clients.Group(lobbyId).SendAsync("UserLeft", user.ConnectionId, GetLobbyUsers(lobbyId));
        }
    }

    private List<string> GetLobbyUsers(string lobbyId)
    {
        var res =  Lobbies.Contains(lobbyId) ? new List<string>(Lobbies.GetValue(lobbyId).Select(x => x.UserName)) : new List<string>();
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