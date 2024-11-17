using Domain;
using Domain.WhoAmI;
using Infrastructure;
using Infrastructure.WhoAmI;
using Microsoft.AspNetCore.SignalR;
using PhoneParty.Domain;
using PhoneParty.Hubs.UserInterface.Interfaces;
using PhoneParty.Hubs.UserInterface.Interfaces.Repositories;

namespace UserInterface.Hubs;

public class WhoIAmHub: Hub
{
    private readonly IRepository<LobbyId, Lobby> LobbyRepository;
    private readonly IRepository<string, User> UserRepository;
    
    public WhoIAmHub(IRepository<LobbyId, Lobby> lobbyRepository, IRepository<string, User> userRepository)
    {
        LobbyRepository = lobbyRepository;
        UserRepository = userRepository;
        var ids = UserIdGenerator.GetAllIds();
        foreach (var id in ids)
        {
            var user = new User(id);
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

    public async Task ChangeTurn(string userId, string lobbyId)
    {
        var user = UserRepository.Get(userId);
        var lobby = LobbyRepository.Get(new LobbyId(lobbyId));
        var role = ((WhoAmIInGameInfo)user.Player.InGameInfo).GameRole;
        var character = HeroRepository.GetHero(((WhoAmIGame)lobby.Game).CurrentGuessedHero);
        await Clients.Caller.SendAsync("ChangeTurn", role, character);
    }
    
}