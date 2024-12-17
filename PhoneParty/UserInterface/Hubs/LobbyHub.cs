// using Application;
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
                _userRepository.TryAdd(id, new WebApplicationUser(id));
        }
    }
        
    public async Task CheckHost(string userId, string lobbyId)
    {
        if (!_lobbyRepository.TryGet(new LobbyId(lobbyId), out var lobby))
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
        _userRepository.TryAdd(id, user);
        Console.WriteLine($"reg {id}");
        await Clients.Caller.SendAsync("UserCreated", id);
    }

    public async void UpdateUserConnection(string userId)
    {
        _userRepository.TryGet(userId, out var user);
        user.SetConnection(Context.ConnectionId);
        Console.WriteLine("Update connection: " + Context.ConnectionId);
    }

    public async void UpdateGroupConnection(string userId, string lobbyIdString)
    {
        _userRepository.TryGet(userId, out var user);
        await Groups.RemoveFromGroupAsync(user.ConnectionId, lobbyIdString);
        user.SetConnection(Context.ConnectionId);
        await Groups.AddToGroupAsync(user.ConnectionId, lobbyIdString);
    }

    public async void UpdateUserName(string userId, string name)
    {
        _userRepository.TryGet(userId, out var user);
        user.SetName(name);
    }

    public async Task UpdateLobby(string lobbyId)
    {
        _lobbyRepository.TryGet(new LobbyId(lobbyId), out var lobby);
        _userRepository.TryGet(lobby.Host.Id, out var host);
        await Clients.Group(lobbyId).SendAsync("UpdateLobbyUsers", GetLobbyUsers(lobbyId), host);
    }

    public async Task StartGame(string lobbyId)
    {
        _lobbyRepository.TryGet(new LobbyId(lobbyId), out var lobby);
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
        _userRepository.TryGet(userId, out var user);
        user.SetName(user.UserName);
        var lobbyId = LobbyIdGenerator.GenerateLobbyId();
        var lobby = new Lobby(lobbyId, user.Player);
        _lobbyRepository.TryAdd(lobbyId, lobby);
        await Groups.AddToGroupAsync(user.ConnectionId, lobbyId.ToString());
    
        // Уведомляем пользователя, что лобби создано
        await Clients.Caller.SendAsync("LobbyCreated", lobbyId.ToString());
    }

    public async Task JoinLobby(string lobbyId, string userId)
    {
        var newLobbyId = new LobbyId(lobbyId);
        if (!_lobbyRepository.Contains(newLobbyId))
            return;
        _userRepository.TryGet(userId, out var user);
        _lobbyRepository.TryGet(new LobbyId(lobbyId), out var lobby);
        lobby.RegisterPlayer(user.Player);
        await Groups.AddToGroupAsync(user.ConnectionId, lobbyId);
        
        await Clients.Caller.SendAsync("LobbyJoinAccept", lobbyId, user.ConnectionId);

        // Уведомляем всех в группе о новом участнике
        _userRepository.TryGet(lobby.Host.Id, out var host);
        await Clients.Group(lobbyId).SendAsync("UpdateLobbyUsers", GetLobbyUsers(lobbyId), host);
    }

    public async Task LeaveLobby(string userId ,string lobbyId)
    {
        var newLobbyId = new LobbyId(lobbyId);
        if (_lobbyRepository.Contains(newLobbyId))
        {
            _userRepository.TryGet(userId, out var user);
            _lobbyRepository.TryGet(new LobbyId(lobbyId), out var lobby);
            var lastHost = lobby.Host;
            lobby.KickPlayer(user.Player);
            if (!Equals(lastHost, lobby.Host))
            {
                _userRepository.TryGet(userId, out var host);
                await Clients.Client(host.ConnectionId).SendAsync("IsHost", true);
            }
            await Groups.RemoveFromGroupAsync(user.ConnectionId, lobbyId);

            if (lobby.PlayersCount == 0)
                _lobbyRepository.TryRemove(newLobbyId);
            else
            {
                _userRepository.TryGet(lobby.Host.Id, out var host);
                await Clients.Group(lobbyId).SendAsync("UpdateLobbyUsers", GetLobbyUsers(lobbyId),
                    host);
            }
        }
    }

    private List<WebApplicationUser> GetLobbyUsers(string lobbyId)
    {
        _lobbyRepository.TryGet(new LobbyId(lobbyId), out var lobby);

        var users = new List<WebApplicationUser>();
        foreach (var player in lobby.GetPlayers)
        {
            _userRepository.TryGet(player.Id, out var user);
            users.Add(user);
        }
        return users;
    }
}