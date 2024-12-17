using Domain;
using Domain.Enums;
using Domain.WhoAmI;
using PhoneParty.Domain;
using PhoneParty.Domain.Enums.WhoAmI;
using PhoneParty.Domain.WhoAmI;
using PhoneParty.Hubs.UserInterface.Interfaces.Repositories;

namespace Application;

public static class WhoAmIGameInteractions
{
    public static void StartGame(string lobbyId, IRepository<LobbyId, Lobby> lobbyRepository)
    {
        var game = new WhoAmIGame();
        lobbyRepository.TryGet(new LobbyId(lobbyId), out var lobby);
        lobby.ChangeGame(game);
        lobby.StartGame();
    }

    public static (WhoAmIRole, Hero, bool) ShowTurnInfo(string lobbyId, Player player, IRepository<LobbyId, Lobby> lobbyRepository)
    {
        lobbyRepository.TryGet(new LobbyId(lobbyId), out var lobby);
        var role = ((WhoAmIInGameInfo)player.InGameInfo).GameRole;
        var character = HeroRepository.GetHero(((WhoAmIGame)lobby.Game).CurrentGuessedHeroId);
        var flag = ((WhoAmIInGameInfo)player.InGameInfo).IsDecisionMaker;
        return (role, character, flag);
    }

    public static GameState ChangeTurn(string lobbyId, Player player, bool decision, IRepository<LobbyId, Lobby> lobbyRepository)
    {
        lobbyRepository.TryGet(new LobbyId(lobbyId), out var lobby);
        lobby.Game.HandleAction(new WhoAmIDecisionAction(player, decision));
        return lobby.Game.State;
    }
}