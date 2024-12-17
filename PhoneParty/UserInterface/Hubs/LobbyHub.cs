using Domain;
using Domain.Enums;
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
    private readonly IRepository<LobbyId, Lobby> _lobbyRepository;
    private readonly IRepository<string, WebApplicationUser> _userRepository;

    public LobbyHub(IRepository<LobbyId, Lobby> lobbyRepository, IRepository<string, WebApplicationUser> userRepository)
    {
        _lobbyRepository = lobbyRepository;
        _userRepository = userRepository;
        var ids = UserIdGenerator.GetAllOccupiedIds();
        foreach (var id in ids)
        {
            if(!_userRepository.Contains(id))
                _userRepository.Add(id, new WebApplicationUser(id));
        }
    }
        
    public async Task CheckHost(string userId, string lobbyId)
    {
        if (!_lobbyRepository.Get(new LobbyId(lobbyId), out var lobby))
        {
            Console.WriteLine("Unknown id " + lobbyId);
            return;
        }
        await Clients.Caller.SendAsync("IsHost", lobby.Host.Id == userId);
    }

    public async void RegisterUser()
    {
        var id = UserIdGenerator.GenerateUserId();
        var user = new WebApplicationUser(id);
        user.SetConnection(Context.ConnectionId);
        _userRepository.Add(id, user);
        Console.WriteLine($"reg {id}");
        await Clients.Caller.SendAsync("UserCreated", id);
    }

    public async void UpdateUserConnection(string userId)
    {
        if (!_userRepository.Get(userId, out var user))
        {
            Console.WriteLine("Unknown id " + userId);
            return;
        }
        user.SetConnection(Context.ConnectionId);
        Console.WriteLine("Update connection: " + Context.ConnectionId);
    }

    public async void UpdateGroupConnection(string userId, string lobbyIdString)
    {
        if (!_userRepository.Get(userId, out var user))
        {
            Console.WriteLine("Unknown id " + userId);
            return;
        }
        await Groups.RemoveFromGroupAsync(user.ConnectionId, lobbyIdString);
        user.SetConnection(Context.ConnectionId);
        await Groups.AddToGroupAsync(user.ConnectionId, lobbyIdString);
    }

    public async void UpdateUserName(string userId, string name)
    {
        if (!_userRepository.Get(userId, out var user))
        {
            Console.WriteLine("Unknown id " + userId);
            return;
        }
        user.SetName(name);
    }

    public async Task UpdateLobby(string lobbyId)
    {
        if (!_lobbyRepository.Get(new LobbyId(lobbyId), out var lobby))
        {
            Console.WriteLine("Unknown id " + lobbyId);
            return;
        }
        if (!_userRepository.Get(lobby.Host.Id, out var host))
        {
            Console.WriteLine("Unknown id " + lobby.Host.Id);
            return;
        }
        await Clients.Group(lobbyId).SendAsync("UpdateLobbyUsers", GetLobbyUsers(lobbyId), host);
    }

    public async Task StartGame(string lobbyId)
    {
        if (!_lobbyRepository.Get(new LobbyId(lobbyId), out var lobby))
        {
            Console.WriteLine("Unknown id " + lobbyId);
            return;
        }
        lobby.ChangeGame(new WhoAmIGame());
        var startStatus = lobby.CheckIfCanStartGame();
        if (startStatus == GameStartingStatusCheck.Correct)
        {
            lobby.StartGame();
            await Clients.Group(lobbyId).SendAsync("GameStarted");
        }
        else
        {
            await Clients.Group(lobbyId).SendAsync("GameStartFail", startStatus.ToString());
        }
    }

    public async Task CreateLobby(string userId)
    {
        var lobbyId = LobbyIdGenerator.GenerateLobbyId();
        // while (!LobbyRepository.Contains(new LobbyId(lobbyId)))
        //     lobbyId = RandomIds.GenerateUserId();
        
        if (!_userRepository.Get(userId, out var user))
        {
            Console.WriteLine("Unknown id " + userId);
            return;
        }
        user.SetName(user.UserName);

        var lobby = new Lobby(lobbyId, user.Player);
        _lobbyRepository.Add(lobbyId, lobby);
        await Groups.AddToGroupAsync(user.ConnectionId, lobbyId.ToString());

        // Уведомляем пользователя, что лобби создано
        await Clients.Caller.SendAsync("LobbyCreated", lobbyId.ToString());
    }

    public async Task JoinLobby(string lobbyId, string userId)
    {
        var newLobbyId = new LobbyId(lobbyId);
        if (!_lobbyRepository.Contains(newLobbyId))
            return;
        if (!_userRepository.Get(userId, out var user))
        {
            Console.WriteLine("Unknown id " + userId);
            return;
        }
        if (!_lobbyRepository.Get(new LobbyId(lobbyId), out var lobby))
        {
            Console.WriteLine("Unknown id " + lobbyId);
            return;
        }
        lobby.RegisterPlayer(user.Player);
        await Groups.AddToGroupAsync(user.ConnectionId, lobbyId);
        
        await Clients.Caller.SendAsync("LobbyJoinAccept", lobbyId, user.ConnectionId);

        // Уведомляем всех в группе о новом участнике
        if (!_userRepository.Get(lobby.Host.Id, out var host))
        {
            Console.WriteLine("Unknown id " + lobby.Host.Id);
            return;
        }
        await Clients.Group(lobbyId).SendAsync("UpdateLobbyUsers", GetLobbyUsers(lobbyId), host);
    }

    public async Task LeaveLobby(string userId ,string lobbyId)
    {
        var newLobbyId = new LobbyId(lobbyId);
        if (_lobbyRepository.Contains(newLobbyId))
        {
            if (!_userRepository.Get(userId, out var user))
            {
                Console.WriteLine("Unknown id " + userId);
                return;
            }
            if (!_lobbyRepository.Get(new LobbyId(lobbyId), out var lobby))
            {
                Console.WriteLine("Unknown id " + lobbyId);
                return;
            }
            var lastHost = lobby.Host;
            lobby.KickPlayer(user.Player);
            if (!Equals(lastHost, lobby.Host))
            {
                if (!_userRepository.Get(userId, out var host))
                {
                    Console.WriteLine("Unknown id " + userId);
                    return;
                }
                await Clients.Client(host.ConnectionId).SendAsync("IsHost", true);
            }
            await Groups.RemoveFromGroupAsync(user.ConnectionId, lobbyId);

            if (lobby.PlayersCount == 0)
                _lobbyRepository.Remove(newLobbyId);
            else
            {
                if (!_userRepository.Get(lobby.Host.Id, out var host))
                {
                    Console.WriteLine("Unknown id " + lobby.Host.Id);
                    return;
                }
                await Clients.Group(lobbyId).SendAsync("UpdateLobbyUsers", GetLobbyUsers(lobbyId),
                    host);
            }
        }
    }

    private List<WebApplicationUser> GetLobbyUsers(string lobbyId)
    {
        if (!_lobbyRepository.Get(new LobbyId(lobbyId), out var lobby))
        {
            Console.WriteLine("Unknown id " + lobbyId);
            return null;
        }

        var users = new List<WebApplicationUser>();
        foreach (var player in lobby.GetPlayers)
        {
            if (!_userRepository.Get(player.Id, out var user))
                Console.WriteLine("Unknown id " + player.Id);
            else
                users.Add(user);
        }
        return users;
    }
}