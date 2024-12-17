using Domain;
using Domain.Enums;
using Domain.WhoAmI;
using PhoneParty.Domain;
using PhoneParty.Hubs.UserInterface.Interfaces.Repositories;

namespace Application;

public static class LobbyInteractions
{
     public static LobbyId CreateLobby(Player host, IRepository<LobbyId, Lobby> lobbyRepository)
    { 
        var lobbyId = LobbyIdGenerator.GenerateLobbyId();
        var lobby = new Lobby(lobbyId, host);
        lobbyRepository.TryAdd(lobbyId, lobby);
        return lobbyId;
     }

     public static GameStartingStatusCheck GetStartStatus(string lobbyId, IRepository<LobbyId, Lobby> lobbyRepository)
     {
         lobbyRepository.TryGet(new LobbyId(lobbyId), out var lobby);
         lobby.ChangeGame(new WhoAmIGame());
         var startStatus = lobby.CheckIfCanStartGame();
         if (startStatus == GameStartingStatusCheck.Correct)
         {
             lobby.StartGame();
         }

         return startStatus;
     }

     public static void JoinLobby(string lobbyId, Player player, IRepository<LobbyId, Lobby> lobbyRepository)
     {
         var newLobbyId = new LobbyId(lobbyId);
         if (!lobbyRepository.Contains(newLobbyId))
             return;
         lobbyRepository.TryGet(new LobbyId(lobbyId), out var lobby);
         lobby.RegisterPlayer(player);
     }

     public static Player GetHost(string lobbyId, IRepository<LobbyId, Lobby> lobbyRepository)
     {
         lobbyRepository.TryGet(new LobbyId(lobbyId), out var lobby);
         return lobby.Host;
     }

     public static bool LeaveLobby(string lobbyId, Player player, IRepository<LobbyId, Lobby> lobbyRepository)
     {
         var newLobbyId = new LobbyId(lobbyId);
         if (lobbyRepository.Contains(newLobbyId))
         {
             lobbyRepository.TryGet(new LobbyId(lobbyId), out var lobby);
             var lastHost = lobby.Host;
             lobby.KickPlayer(player);
             return Equals(player, lastHost);
         }

         return true;
     }

     public static int GetLobbyPlayersCount(string lobbyId, IRepository<LobbyId, Lobby> lobbyRepository)
     {
         lobbyRepository.TryGet(new LobbyId(lobbyId), out var lobby);
         return lobby.PlayersCount;
     }
}