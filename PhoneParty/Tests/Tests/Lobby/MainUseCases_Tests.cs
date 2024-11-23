using System.Reflection;
using Domain;
using Domain.Enums;
using Domain.WhoAmI;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using PhoneParty.Domain;
using PhoneParty.Domain.Enums;
using PhoneParty.Domain.WhoAmI;

namespace PhonePartyTests.Tests.Lobby;

[TestFixture]
public class MainUseCaseTests
{
    [Test]
    public void TestLobbyCreation()
    {
        var player = new Player("1");
        var lobby = new Domain.Lobby(new LobbyId("4F3B"), player);

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
        var lobby = new Domain.Lobby(new LobbyId("4F3B"), host);
        var result = lobby.CheckIfCanRegisterPlayer(player);
        lobby.RegisterPlayer(player);

        var players = lobby.GetPlayers;
        Assert.That(result, Is.EqualTo(PlayerRegistrationResult.SuccessfulRegistered));
        Assert.That(players, Is.Not.Null);
        Assert.That(players.Count, Is.EqualTo(2));
        Assert.That(players[0], Is.EqualTo(host));
        Assert.That(players[1], Is.EqualTo(player));
    }
    
    [Test]
    public void TestSinglePlayerKick()
    {
        var host = new Player("1");
        var player = new Player("2");
        var lobby = new Domain.Lobby(new LobbyId("4F3B"), host);
        lobby.RegisterPlayer(player);
        var result = lobby.CheckIfCanKickPlayer(player);
        lobby.KickPlayer(player);
        var players = lobby.GetPlayers;
        
        Assert.That(result, Is.EqualTo(PlayerKickResult.SuccessfulKicked));
        Assert.That(players, Is.Not.Null);
        Assert.That(players.Count, Is.EqualTo(1));
        Assert.That(players[0], Is.EqualTo(host));
    }
    
    [Test]
    public void TestSinglePlayerKickAfterGameStarted()
    {
        var host = new Player("1");
        var player = new Player("2");
        var lobby = new Domain.Lobby(new LobbyId("4F3B"), host);
        lobby.RegisterPlayer(player);
        lobby.ChangeGame(new WhoAmIGame());
        lobby.StartGame();
        var players = lobby.GetPlayers;
        var result = lobby.CheckIfCanKickPlayer(player);
        
        Assert.That(result, Is.EqualTo(PlayerKickResult.GameInProgress));
        Assert.That(players, Is.Not.Null);
        Assert.That(players.Count, Is.EqualTo(2));
        Assert.That(players[0], Is.EqualTo(host));
        Assert.That(players[1], Is.EqualTo(player));
    }

    [Test]
    public void TestSinglePlayerRegistrationAfterGameStarted()
    {
        var host = new Player("1");
        var player2 = new Player("2");
        var player3 = new Player("3");
        var lobby = new Domain.Lobby(new LobbyId("4F3B"), host);
        lobby.ChangeGame(new WhoAmIGame());
        lobby.RegisterPlayer(player2);
        lobby.StartGame();
        var result = lobby.CheckIfCanRegisterPlayer(player3);

        var players = lobby.GetPlayers;
        Assert.That(result, Is.EqualTo(PlayerRegistrationResult.GameInProgress));
        Assert.That(players, Is.Not.Null);
        Assert.That(players.Count, Is.EqualTo(2));
        Assert.That(players[0], Is.EqualTo(host));
        Assert.That(players[1], Is.EqualTo(player2));
    }
    
    [Test]
    public void TestCanChangeGameChecker()
    {
        var host = new Player("1");
        var lobby = new Domain.Lobby(new LobbyId("4F3B"), host);
        var game = new WhoAmIGame();
        Assert.That(lobby.CheckIfCanChangeGame(game), Is.EqualTo(GameStartingStatusCheck.Correct));
        
        lobby.RegisterPlayer(new Player("2"));
        Assert.That(lobby.CheckIfCanChangeGame(game), Is.EqualTo(GameStartingStatusCheck.Correct));
        
        lobby.RegisterPlayer(new Player("3"));
        lobby.RegisterPlayer(new Player("4"));
        lobby.RegisterPlayer(new Player("5"));
        lobby.RegisterPlayer(new Player("6"));
        var lastPlayer = new Player("7");
        lobby.RegisterPlayer(lastPlayer);
        
        Assert.That(lobby.CheckIfCanChangeGame(game), Is.EqualTo(GameStartingStatusCheck.Correct));

        lobby.KickPlayer(lastPlayer);
        Assert.That(lobby.CheckIfCanChangeGame(game), Is.EqualTo(GameStartingStatusCheck.Correct));
        
        lobby.ChangeGame(game);
        lobby.StartGame();
        Assert.That(lobby.CheckIfCanChangeGame(game), Is.EqualTo(GameStartingStatusCheck.GameInProgress));
    }

    [Test]
    public void TestCanStartGameChecker()
    {
        var host = new Player("1");
        var lobby = new Domain.Lobby(new LobbyId("4F3B"), host);
        var result = lobby.CheckIfCanStartGame();
        Assert.That(result, Is.EqualTo(GameStartingStatusCheck.NoGameDefined));

        lobby.ChangeGame(new WhoAmIGame());
        result = lobby.CheckIfCanStartGame();
        Assert.That(result, Is.EqualTo(GameStartingStatusCheck.LessThenMinimumAmountOfPlayers));

        lobby.RegisterPlayer(new Player("2"));
        result = lobby.CheckIfCanStartGame();
        Assert.That(result, Is.EqualTo(GameStartingStatusCheck.Correct));

        lobby.RegisterPlayer(new Player("3"));
        lobby.RegisterPlayer(new Player("4"));
        lobby.RegisterPlayer(new Player("5"));
        lobby.RegisterPlayer(new Player("6"));
        var lastPlayer = new Player("7");
        lobby.RegisterPlayer(lastPlayer);
        result = lobby.CheckIfCanStartGame();
        Assert.That(result, Is.EqualTo(GameStartingStatusCheck.MoreThenMaximumAmountOfPlayers));

        lobby.KickPlayer(lastPlayer);
        lobby.StartGame();
        result = lobby.CheckIfCanStartGame();
        Assert.That(result, Is.EqualTo(GameStartingStatusCheck.GameInProgress));
    }
    
    [Test]
    public void TestMultiplePlayerRegistration()
    {
        var host = new Player("1");
        var lobby = new Domain.Lobby(new LobbyId("A1B2"), host);

        var playersToRegister = new List<Player>
        {
            new Player("2"),
            new Player("3"),
            new Player("4"),
            new Player("5"),
            new Player("6")
        };

        foreach (var player in playersToRegister)
        {
            var result = lobby.RegisterPlayer(player);
            Assert.That(result, Is.EqualTo(PlayerRegistrationResult.SuccessfulRegistered), $"Не удалось зарегистрировать игрока {player.Id}");
        }

        var players = lobby.GetPlayers;
        Assert.That(players.Count, Is.EqualTo(6));
        CollectionAssert.Contains(players, host);
        foreach (var player in playersToRegister)
        {
            CollectionAssert.Contains(players, player);
        }
    }
    
    [Test]
    public void TestKickingNonExistentPlayer()
    {
        var host = new Player("1");
        var lobby = new Domain.Lobby(new LobbyId("C3D4"), host);

        var nonExistentPlayer = new Player("2");
        var result = lobby.KickPlayer(nonExistentPlayer);

        Assert.That(result, Is.EqualTo(PlayerKickResult.GameInProgress), "Попытка кика несуществующего игрока должна возвращать статус PlayerNotFound.");

        var players = lobby.GetPlayers;
        Assert.That(players.Count, Is.EqualTo(1));
        CollectionAssert.Contains(players, host);
    }

    [Test]
    public void TestChangingGameWhileGameInProgress()
    {
        var host = new Player("1");
        var player2 = new Player("2");
        var lobby = new Domain.Lobby(new LobbyId("D4E5"), host);
        lobby.RegisterPlayer(player2);

        var initialGame = new WhoAmIGame();
        lobby.ChangeGame(initialGame);
        lobby.StartGame();

        var newGame = new WhoAmIGame();
        var canChangeGame = lobby.CheckIfCanChangeGame(newGame);

        Assert.That(canChangeGame, Is.EqualTo(GameStatusCheck.GameInProgress), "Изменение игры во время активной игры должно возвращать статус GameInProgress.");
        Assert.Throws<InvalidOperationException>(() => lobby.ChangeGame(newGame), "Изменение игры во время активной игры должно выбрасывать исключение.");
    }
    
    [Test]
    public void TestAssignNewHostWhenHostIsKicked()
    {
        var host = new Player("1");
        var player2 = new Player("2");
        var player3 = new Player("3");
        var lobby = new Domain.Lobby(new LobbyId("E5F6"), host);
        lobby.RegisterPlayer(player2);
        lobby.RegisterPlayer(player3);

        var result = lobby.KickPlayer(host);
        Assert.That(result, Is.EqualTo(PlayerKickResult.SuccessfulKicked), "Хост должен быть успешно кикнут.");

        var players = lobby.GetPlayers;
        Assert.That(players.Count, Is.EqualTo(2));
        CollectionAssert.DoesNotContain(players, host);
        // считаю, что новый хост должен быть первый игрок в списке
        var newHost = players[0];
        Assert.That(lobby.Host.Equals(newHost), Is.True, "Новый хост должен быть назначен.");
    }

    
    [Test]
    public void TestClosingLobby()
    {
        var host = new Player("1");
        var player2 = new Player("2");
        var player3 = new Player("3");
        var lobby = new Domain.Lobby(new LobbyId("G7H8"), host);
        lobby.RegisterPlayer(player2);
        lobby.RegisterPlayer(player3);

        var game = new WhoAmIGame();
        lobby.ChangeGame(game);
        lobby.StartGame();

        lobby.CloseGame();

        var players = lobby.GetPlayers;
        Assert.That(players.Count, Is.EqualTo(0), "После закрытия лобби список игроков должен быть пустым.");
        Assert.That(game.IsInProgress, Is.False, "Свойство IsInProgress должно быть false после закрытия лобби.");
        Assert.That(game.IsFinished, Is.False, "Свойство IsFinished должно быть false после закрытия лобби.");
    }
    
    [Test]
    public void TestPlayersListImmutability()
    {
        var host = new Player("1");
        var lobby = new Domain.Lobby(new LobbyId("J0K1"), host);
        var player2 = new Player("2");
        lobby.RegisterPlayer(player2);

        var players = lobby.GetPlayers;
        Assert.That(players.Count, Is.EqualTo(2));
        Assert.Throws<NotSupportedException>(() => ((List<Player>)players).Add(new Player("3")), "Список игроков должен быть неизменяемым.");
    }

    [Test]
    public void TestGetCurrentHost()
    {
        var host = new Player("1");
        var player2 = new Player("2");
        var lobby = new Domain.Lobby(new LobbyId("N4O5"), host);
        lobby.RegisterPlayer(player2);

        var currentHost = lobby.Host;
        Assert.That(currentHost, Is.EqualTo(host), "Хост лобби должен быть корректно возвращен.");
    }

}