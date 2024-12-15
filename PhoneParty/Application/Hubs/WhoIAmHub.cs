using Domain;
using Domain.Enums;
using Domain.WhoAmI;
using Infrastructure;
using Microsoft.AspNetCore.SignalR;
using PhoneParty.Domain;
using PhoneParty.Domain.WhoAmI;
using PhoneParty.Hubs.UserInterface.Interfaces.Repositories;

namespace Application.Hubs;

public class WhoIAmHub: Hub
{
    private readonly IRepository<LobbyId, Lobby> _lobbyRepository;
    private readonly IRepository<string, WebApplicationUser> _userRepository;
    
    public WhoIAmHub(IRepository<LobbyId, Lobby> lobbyRepository, IRepository<string, WebApplicationUser> userRepository)
    {
        _lobbyRepository = lobbyRepository;
        _userRepository = userRepository;
        var ids = UserIdGenerator.GetAllOccupiedIds();
        foreach (var id in ids)
        {
            var user = new WebApplicationUser(id);
            if(!_userRepository.Contains(id))
                _userRepository.Add(id, user);
        }
    }

    public async Task StartGame(string lobbyId)
    {
        var game = new WhoAmIGame();
        if (!_lobbyRepository.Get(new LobbyId(lobbyId), out var lobby))
        {
            Console.WriteLine("Unknown id " + lobbyId);
            return;
        }
        lobby.ChangeGame(game);
        lobby.StartGame();
        await Clients.Group(lobbyId).SendAsync("StartGame");
    }

    public async Task ShowTurnInfo(string userId, string lobbyId)
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
        var role = ((WhoAmIInGameInfo)user.Player.InGameInfo).GameRole;
        var character = HeroRepository.GetHero(((WhoAmIGame)lobby.Game).CurrentGuessedHeroId);
        var flag = ((WhoAmIInGameInfo)user.Player.InGameInfo).IsDecisionMaker;
        await Clients.Caller.SendAsync("ShowTurn", role, flag,  character);
    }

    public async Task ChangeTurn(string userId ,string lobbyId, bool decision)
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
        lobby.Game.HandleAction(new WhoAmIDecisionAction(user.Player, decision));
        if (lobby.Game.State == GameState.Finished)
            await Clients.Group(lobbyId).SendAsync("GameEnd");
        else
            await Clients.Group(lobbyId).SendAsync("ChangeTurn");
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

    // public async void UpdateUserName(string id, string name)
    // {
    //     var user = UserRepository.Get(id);
    //     user.SetName(name);
    // }
    //
    // public async Task UpdateLobby(string lobbyIdString)
    // {
    //     await Clients.Group(lobbyIdString).SendAsync("UpdateLobbyUsers", GetLobbyUsers(lobbyIdString));
    // }
}