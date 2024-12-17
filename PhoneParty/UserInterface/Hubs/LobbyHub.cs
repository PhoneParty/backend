// using Application;

using Application;
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
            if (!_userRepository.Contains(id))
                _userRepository.TryAdd(id, new WebApplicationUser(id));
        }
    }

    public async Task CheckHost(string userId, string lobbyId)
    {
        await Clients.Caller.SendAsync("IsHost", LobbyInteractions.GetHost(lobbyId, _lobbyRepository).Id == userId);
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
        _userRepository.TryGet(LobbyInteractions.GetHost(lobbyId, _lobbyRepository).Id, out var host);
        await Clients.Group(lobbyId).SendAsync("UpdateLobbyUsers", GetLobbyUsers(lobbyId), host);
    }

    public async Task StartGame(string lobbyId)
    {
        var startStatus = LobbyInteractions.GetStartStatus(lobbyId, _lobbyRepository);
        if (startStatus == GameStartingStatusCheck.Correct)
            await Clients.Group(lobbyId).SendAsync("GameStarted");
        else
            await Clients.Group(lobbyId).SendAsync("GameStartFail", startStatus.ToString());
    }

    public async Task CreateLobby(string userId)
    {
        _userRepository.TryGet(userId, out var user);
        user.SetName(user.UserName);
        var lobbyId = LobbyInteractions.CreateLobby(user.Player, _lobbyRepository);
        await Groups.AddToGroupAsync(user.ConnectionId, lobbyId.ToString());

        await Clients.Caller.SendAsync("LobbyCreated", lobbyId.ToString());
    }

    public async Task JoinLobby(string lobbyId, string userId)
    {
        _userRepository.TryGet(userId, out var user);
        LobbyInteractions.JoinLobby(lobbyId, user.Player, _lobbyRepository);
        await Groups.AddToGroupAsync(user.ConnectionId, lobbyId);

        await Clients.Caller.SendAsync("LobbyJoinAccept", lobbyId, user.ConnectionId);

        _userRepository.TryGet(LobbyInteractions.GetHost(lobbyId, _lobbyRepository).Id, out var host);
        await Clients.Group(lobbyId).SendAsync("UpdateLobbyUsers", GetLobbyUsers(lobbyId), host);
    }

    public async Task LeaveLobby(string userId, string lobbyId)
    {
        _userRepository.TryGet(userId, out var user);
        var kickedHost = LobbyInteractions.LeaveLobby(lobbyId, user.Player, _lobbyRepository);
        if (kickedHost)
        {
            await Clients.Client(user.ConnectionId).SendAsync("IsHost", true);
        }

        await Groups.RemoveFromGroupAsync(user.ConnectionId, lobbyId);

        if (LobbyInteractions.GetLobbyPlayersCount(lobbyId, _lobbyRepository) == 0)
            _lobbyRepository.TryRemove(new LobbyId(lobbyId));
        else
        {
            _userRepository.TryGet(LobbyInteractions.GetHost(lobbyId, _lobbyRepository).Id, out var host);
            await Clients.Group(lobbyId).SendAsync("UpdateLobbyUsers", GetLobbyUsers(lobbyId),
                host);
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