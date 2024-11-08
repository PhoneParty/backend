using System.Text;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using PhoneParty.Hubs.Infastructure;

namespace PhoneParty.Hubs;

public class LobbyHub : Hub
{
    // Словарь для хранения участников по ID лобби
    private readonly IMemoryRep Lobbies;

    public LobbyHub(IMemoryRep memoryRep)
    {
        Lobbies = memoryRep;
    }

    public async Task CreateLobby(string name)
    {
        var lobbyId = new Random().NextString();
        
        if (!Lobbies.Contains(lobbyId))
            Lobbies.AddValue(lobbyId, []);
        
        var user = new User(name, Context.ConnectionId);
        
        // Добавляем пользователя в группу (лобби) и в словарь
        await Groups.AddToGroupAsync(user.connectionId, lobbyId);
        Lobbies.GetValue(lobbyId).Add(Context.ConnectionId);

        // Уведомляем пользователя, что лобби создано
        await Clients.Caller.SendAsync("LobbyCreated", lobbyId, name);
    }

    public async Task JoinLobby(string lobbyId, string name)
    {
        if (!Lobbies.Contains(lobbyId))
            return;
        // Добавляем пользователя в группу и в словарь
        var user = new User(name, Context.ConnectionId);
        await Groups.AddToGroupAsync(user.connectionId, lobbyId);
        Console.WriteLine(Groups);
        Lobbies.GetValue(lobbyId).Add(Context.ConnectionId);
        
        await Clients.Caller.SendAsync("JoinedToLobby", lobbyId, Context.ConnectionId, GetLobbyUsers(lobbyId));

        // Уведомляем всех в группе о новом участнике
        await Clients.Group(lobbyId).SendAsync("UserJoined", Context.ConnectionId, GetLobbyUsers(lobbyId));
    }

    public async Task LeaveLobby(string lobbyId)
    {
        if (Lobbies.Contains(lobbyId))
        {
            // var user = Lobbies[lobbyId].FirstOrDefault(x => x.connectionId == Context.ConnectionId);
            Lobbies.GetValue(lobbyId).Remove(Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyId);

            if (Lobbies.GetValue(lobbyId).Count == 0)
                Lobbies.Remove(lobbyId);
            else
                await Clients.Group(lobbyId).SendAsync("UserLeft", Context.ConnectionId, GetLobbyUsers(lobbyId));
        }
    }

    private List<string> GetLobbyUsers(string lobbyId)
    {
        return Lobbies.Contains(lobbyId) ? new List<string>(Lobbies.GetValue(lobbyId)) : new List<string>();
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