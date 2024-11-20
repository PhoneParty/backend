using Domain;
using Domain.WhoAmI;
using Infrastructure;
using Infrastructure.WhoAmI;
using Microsoft.AspNetCore.SignalR;
using PhoneParty.Domain;
using PhoneParty.Domain.Enums.WhoAmI;
using PhoneParty.Domain.WhoAmI;
using PhoneParty.Hubs.UserInterface.Interfaces;
using PhoneParty.Hubs.UserInterface.Interfaces.Repositories;

namespace UserInterface.Hubs;

public class WhoIAmHub: Hub
{
    private readonly IRepository<LobbyId, Lobby> LobbyRepository;
    private readonly IRepository<string, WebApplicationUser> UserRepository;
    
    public WhoIAmHub(IRepository<LobbyId, Lobby> lobbyRepository, IRepository<string, WebApplicationUser> userRepository)
    {
        LobbyRepository = lobbyRepository;
        UserRepository = userRepository;
        var ids = UserIdGenerator.GetAllIds();
        foreach (var id in ids)
        {
            var user = new WebApplicationUser(id);
            if(!UserRepository.Contains(id))
                UserRepository.Add(id, user);
        }
    }

    public async Task StartGame(string lobbyId)
    {
        var game = new WhoAmIGame();
        var lobby = LobbyRepository.Get(new LobbyId(lobbyId));
        lobby.ChangeGame(game);
        lobby.StartGame();
        await Clients.Group(lobbyId).SendAsync("StartGame");
    }

    public async Task ShowTurnInfo(string userId, string lobbyId)
    {
        var user = UserRepository.Get(userId);
        var lobby = LobbyRepository.Get(new LobbyId(lobbyId));
        var role = ((WhoAmIInGameInfo)user.Player.InGameInfo).GameRole;
        var character = HeroRepository.GetHero(((WhoAmIGame)lobby.Game).CurrentGuessedHeroId);
        var flag = ((WhoAmIInGameInfo)user.Player.InGameInfo).IsDecisionMaker;
        await Clients.Caller.SendAsync("ShowTurn", role, flag,  character);
    }

    public async Task ChangeTurn(string userId ,string lobbyId, bool decision)
    {
        var lobby = LobbyRepository.Get(new LobbyId(lobbyId));
        lobby.Game.HandleAction(new WhoAmIDecisionAction(UserRepository.Get(userId).Player, decision));
        await Clients.Group(lobbyId).SendAsync("ChangeTurn");
    }
    
    public async void UpdateUserConnection(string id)
    {
        var user = UserRepository.Get(id);
        user.SetConnection(Context.ConnectionId);
        Console.WriteLine("Update connection: " + Context.ConnectionId);
    }

    public async void UpdateGroupConnection(string userId, string lobbyIdString)
    {
        var user = UserRepository.Get(userId);
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