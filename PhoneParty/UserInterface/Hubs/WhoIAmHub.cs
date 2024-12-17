using Domain;
using Domain.Enums;
using Domain.WhoAmI;
using Infrastructure;
using Microsoft.AspNetCore.SignalR;
using PhoneParty.Domain;
using PhoneParty.Domain.Enums.WhoAmI;
using PhoneParty.Domain.WhoAmI;
using PhoneParty.Hubs.UserInterface.Interfaces;
using PhoneParty.Hubs.UserInterface.Interfaces.Repositories;

namespace UserInterface.Hubs;

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
                _userRepository.TryAdd(id, user);
        }
    }

    public async Task StartGame(string lobbyId)
    {
        var game = new WhoAmIGame();
        _lobbyRepository.TryGet(new LobbyId(lobbyId), out var lobby);
        lobby.ChangeGame(game);
        lobby.StartGame();
        await Clients.Group(lobbyId).SendAsync("StartGame");
    }

    public async Task ShowTurnInfo(string userId, string lobbyId)
    {
        _userRepository.TryGet(userId, out var user);
        _lobbyRepository.TryGet(new LobbyId(lobbyId), out var lobby);
        var role = ((WhoAmIInGameInfo)user.Player.InGameInfo).GameRole;
        var character = HeroRepository.GetHero(((WhoAmIGame)lobby.Game).CurrentGuessedHeroId);
        var flag = ((WhoAmIInGameInfo)user.Player.InGameInfo).IsDecisionMaker;
        await Clients.Caller.SendAsync("ShowTurn", role, flag,  character);
    }

    public async Task ChangeTurn(string userId ,string lobbyId, bool decision)
    {
        _userRepository.TryGet(userId, out var user);
        _lobbyRepository.TryGet(new LobbyId(lobbyId), out var lobby);
        lobby.Game.HandleAction(new WhoAmIDecisionAction(user.Player, decision));
        if (lobby.Game.State == GameState.Finished)
            await Clients.Group(lobbyId).SendAsync("GameEnd");
        else
            await Clients.Group(lobbyId).SendAsync("ChangeTurn");
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
}