using Microsoft.AspNetCore.SignalR;
using PhoneParty.Hubs.Infastructure;
using PhoneParty.Hubs.UserInterface.Interfaces;

namespace PhoneParty.Hubs;

public class LobbyHub : Hub
{
    // Словарь для хранения участников по ID лобби
    private readonly IMemoryRep<User> Lobbies;

    public LobbyHub(IMemoryRep<User> memoryRep)
    {
        Lobbies = memoryRep;
    }

    public async Task CreateLobby(string name)
    {
        var lobbyId = new Random().NextString();
        var user = new User(name, Context.ConnectionId);
        
        if (!Lobbies.Contains(lobbyId))
            Lobbies.AddValue(lobbyId, []);
        
        // // Добавляем пользователя в группу (лобби) и в словарь
        //await Groups.AddToGroupAsync(user.connectionId, lobbyId);
        // Lobbies.GetValue(lobbyId).Add(Context.ConnectionId);

        // Уведомляем пользователя, что лобби создано
        await Clients.Caller.SendAsync("LobbyCreated", lobbyId, name);
    }

    public async Task JoinLobby(string lobbyId, string name)
    {
        if (!Lobbies.Contains(lobbyId))
            return;
        // Добавляем пользователя в группу и в словарь
        var user = new User(name, Context.ConnectionId);
        await Groups.AddToGroupAsync(user.ConnectionId, lobbyId);
        Lobbies.GetValue(lobbyId).Add(user);
        
        await Clients.Caller.SendAsync("JoinedToLobby", lobbyId, Context.ConnectionId, GetLobbyUsers(lobbyId));

        // Уведомляем всех в группе о новом участнике
        await Clients.Group(lobbyId).SendAsync("UserJoined", Context.ConnectionId, GetLobbyUsers(lobbyId));
    }

    public async Task LeaveLobby(string lobbyId)
    {
        if (Lobbies.Contains(lobbyId))
        {
            var user = Lobbies.GetValue(lobbyId).FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            Lobbies.GetValue(lobbyId).Remove(user);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyId);

            if (Lobbies.GetValue(lobbyId).Count == 0)
                Lobbies.Remove(lobbyId);
            else
                await Clients.Group(lobbyId).SendAsync("UserLeft", Context.ConnectionId, GetLobbyUsers(lobbyId));
        }
    }

    private List<string> GetLobbyUsers(string lobbyId)
    {
        return Lobbies.Contains(lobbyId) ? new List<string>(Lobbies.GetValue(lobbyId).Select(x => x.UserName)) : new List<string>();
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