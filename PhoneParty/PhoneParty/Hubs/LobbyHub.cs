using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace PhoneParty.Hubs;

public class LobbyHub : Hub
{
    // Словарь для хранения участников по ID лобби
    private static readonly Dictionary<string, HashSet<string>> Lobbies = new Dictionary<string, HashSet<string>>();

    public async Task CreateLobby(string lobbyId)
    {
        if (!Lobbies.ContainsKey(lobbyId))
        {
            Lobbies[lobbyId] = new HashSet<string>();
        }

        // Добавляем пользователя в группу (лобби) и в словарь
        await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId);
        Lobbies[lobbyId].Add(Context.ConnectionId);

        // Уведомляем пользователя, что лобби создано
        await Clients.Caller.SendAsync("LobbyCreated", lobbyId, GetLobbyUsers(lobbyId));
    }

    public async Task JoinLobby(string lobbyId)
    {
        if (!Lobbies.ContainsKey(lobbyId))
        {
            Lobbies[lobbyId] = new HashSet<string>();
        }

        // Добавляем пользователя в группу и в словарь
        await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId);
        Lobbies[lobbyId].Add(Context.ConnectionId);

        // Уведомляем всех в группе о новом участнике
        await Clients.Group(lobbyId).SendAsync("UserJoined", Context.ConnectionId, GetLobbyUsers(lobbyId));
    }

    public async Task LeaveLobby(string lobbyId)
    {
        if (Lobbies.ContainsKey(lobbyId))
        {
            Lobbies[lobbyId].Remove(Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyId);

            if (Lobbies[lobbyId].Count == 0)
            {
                Lobbies.Remove(lobbyId);
            }
            else
            {
                await Clients.Group(lobbyId).SendAsync("UserLeft", Context.ConnectionId, GetLobbyUsers(lobbyId));
            }
        }
    }

    private List<string> GetLobbyUsers(string lobbyId)
    {
        return Lobbies.ContainsKey(lobbyId) ? new List<string>(Lobbies[lobbyId]) : new List<string>();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        foreach (var lobby in Lobbies)
        {
            if (lobby.Value.Contains(Context.ConnectionId))
            {
                await LeaveLobby(lobby.Key);
                break;
            }
        }
        await base.OnDisconnectedAsync(exception);
    }
}