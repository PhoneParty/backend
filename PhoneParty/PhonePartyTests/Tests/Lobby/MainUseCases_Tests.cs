using System.Reflection;
using NUnit.Framework;
using PhoneParty.Domain;
using PhoneParty.Domain.Enums;
using PhoneParty.Domain.WhoAmI;

namespace PhonePartyTests.Tests.Lobby;

[TestFixture]
public class MainUseCase_Tests
{
    [Test]
    public void TestLobbyCreation()
    {
        var player = new Player("1");
        var lobby = new PhoneParty.Domain.Lobby(new LobbyId("4F3B"), player);

        var players = lobby.GetPlayers;

        Assert.That(players, Is.Not.Null);
        Assert.That(players.Count, Is.EqualTo(1));
        Assert.That(players[0], Is.EqualTo(player));
        Assert.That(player.Lobby, Is.EqualTo(lobby));
    }

    [Test]
    public void TestSinglePlayerRegistration()
    {
        var host = new Player("1");
        var player = new Player("2");
        var lobby = new PhoneParty.Domain.Lobby(new LobbyId("4F3B"), host);
        var result = lobby.RegisterPlayer(player);

        var players = lobby.GetPlayers;
        Assert.That(result, Is.EqualTo(PlayerRegistrationResult.SuccessfulRegistered));
        Assert.That(players, Is.Not.Null);
        Assert.That(players.Count, Is.EqualTo(2));
        Assert.That(players[0], Is.EqualTo(host));
        Assert.That(players[1], Is.EqualTo(player));
    }
    
    [Test]
    public void TestSinglePlayerUnregistration()
    {
        var host = new Player("1");
        var player = new Player("2");
        var lobby = new PhoneParty.Domain.Lobby(new LobbyId("4F3B"), host);
        lobby.RegisterPlayer(player);
        var result = lobby.UnregisterPlayer(player);
        var players = lobby.GetPlayers;
        
        Assert.That(result, Is.EqualTo(PlayerRegistrationResult.SuccessfulUnregistered));
        Assert.That(players, Is.Not.Null);
        Assert.That(players.Count, Is.EqualTo(1));
        Assert.That(players[0], Is.EqualTo(host));
    }

    [Test]
    public void TestSinglePlayerRegistrationAfterGameStarted()
    {
        var host = new Player("1");
        var player2 = new Player("2");
        var player3 = new Player("3");
        var lobby = new PhoneParty.Domain.Lobby(new LobbyId("4F3B"), host);
        lobby.ChangeGame(new WhoAmIGame());
        lobby.RegisterPlayer(player2);
        lobby.StartGame();
        var result = lobby.RegisterPlayer(player3);

        var players = lobby.GetPlayers;
        Assert.That(result, Is.EqualTo(PlayerRegistrationResult.GameInProgress));
        Assert.That(players, Is.Not.Null);
        Assert.That(players.Count, Is.EqualTo(2));
        Assert.That(players[0], Is.EqualTo(host));
        Assert.That(players[1], Is.EqualTo(player2));
    }

    [Test]
    public void TestCanStartGameChecker()
    {
        var host = new Player("1");
        var lobby = new PhoneParty.Domain.Lobby(new LobbyId("4F3B"), host);
        var result = lobby.CheckIfCanStartGame();
        Assert.That(result, Is.EqualTo(GameStartCheck.NoGameDefined));

        lobby.ChangeGame(new WhoAmIGame());
        result = lobby.CheckIfCanStartGame();
        Assert.That(result, Is.EqualTo(GameStartCheck.LessThenMinimumAmountOfPlayers));

        lobby.RegisterPlayer(new Player("2"));
        result = lobby.CheckIfCanStartGame();
        Assert.That(result, Is.EqualTo(GameStartCheck.Successful));

        lobby.RegisterPlayer(new Player("3"));
        lobby.RegisterPlayer(new Player("4"));
        lobby.RegisterPlayer(new Player("5"));
        lobby.RegisterPlayer(new Player("6"));
        var lastPlayer = new Player("7");
        lobby.RegisterPlayer(lastPlayer);
        result = lobby.CheckIfCanStartGame();
        Assert.That(result, Is.EqualTo(GameStartCheck.MoreThenMaximumAmountOfPlayers));

        lobby.UnregisterPlayer(lastPlayer);
        lobby.StartGame();
        result = lobby.CheckIfCanStartGame();
        Assert.That(result, Is.EqualTo(GameStartCheck.GameInProgress));
    }
}