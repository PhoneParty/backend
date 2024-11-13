﻿using NUnit.Framework;
using System.Reflection;
using Domain;
using Domain.WhoAmI;
using PhoneParty.Domain;
using PhoneParty.Domain.Enums;
using PhoneParty.Domain.Enums.WhoAmI;
using PhoneParty.Domain.WhoAmI;

namespace PhonePartyTests.Tests.WhoAmI;

[TestFixture]
public class MainUseCases_Tests
{
    [Test]
    public void TestGeneralGameScenario()
    {
        var host = new Player("1");
        var player2 = new Player("2");
        var player3 = new Player("3");
        var player4 = new Player("4");
        var player5 = new Player("5");
        var player6 = new Player("6");
        
        var lobby = new Domain.Lobby(new LobbyId("4F3B"), host);
        
        lobby.RegisterPlayer(player2);
        lobby.RegisterPlayer(player3);
        lobby.RegisterPlayer(player4);
        lobby.RegisterPlayer(player5);
        lobby.RegisterPlayer(player6);

        var game = new WhoAmIGame();
        
        lobby.ChangeGame(game);
        
        Assert.That(game.IsInProgress, Is.False);
        Assert.That(game.IsFinished, Is.False);
        
        lobby.StartGame();
        
        Assert.That(game.IsInProgress, Is.True);
        Assert.That(game.IsFinished, Is.False);
        
        var players = lobby.GetPlayers;
        Assert.That(((WhoAmIInGameInfo)players[0].InGameInfo!).GameRole, Is.EqualTo(WhoAmIRole.Guesser));
        foreach (var player in players.Skip(1))
        {
            Assert.That(((WhoAmIInGameInfo)player.InGameInfo!).GameRole, Is.EqualTo(WhoAmIRole.Player));
        }
        Assert.That(((WhoAmIInGameInfo)players[^1].InGameInfo!).IsDecisionMaker, Is.True);
        
        lobby.HandleAction(new WhoAmIDecisionAction(host, false));
        
        Assert.That(((WhoAmIInGameInfo)players[0].InGameInfo!).GameRole, Is.EqualTo(WhoAmIRole.Player));
        Assert.That(((WhoAmIInGameInfo)players[1].InGameInfo!).GameRole, Is.EqualTo(WhoAmIRole.Guesser));
        foreach (var player in players.Skip(3))
        {
            Assert.That(((WhoAmIInGameInfo)player.InGameInfo!).GameRole, Is.EqualTo(WhoAmIRole.Player));
        }
        Assert.That(((WhoAmIInGameInfo)players[^1].InGameInfo!).IsDecisionMaker, Is.False);
        Assert.That(((WhoAmIInGameInfo)players[0].InGameInfo!).IsDecisionMaker, Is.True);
        
        lobby.HandleAction(new WhoAmIDecisionAction(players[1], true));
        
        Assert.That(((WhoAmIInGameInfo)players[0].InGameInfo!).GameRole, Is.EqualTo(WhoAmIRole.Player));
        Assert.That(((WhoAmIInGameInfo)players[1].InGameInfo!).GameRole, Is.EqualTo(WhoAmIRole.Observer));
        Assert.That(((WhoAmIInGameInfo)players[2].InGameInfo!).GameRole, Is.EqualTo(WhoAmIRole.Guesser));
        Assert.That(((WhoAmIInGameInfo)players[3].InGameInfo!).GameRole, Is.EqualTo(WhoAmIRole.Player));
        Assert.That(((WhoAmIInGameInfo)players[4].InGameInfo!).GameRole, Is.EqualTo(WhoAmIRole.Player));
        Assert.That(((WhoAmIInGameInfo)players[5].InGameInfo!).GameRole, Is.EqualTo(WhoAmIRole.Player));
        
        Assert.That(((WhoAmIInGameInfo)players[1].InGameInfo!).IsDecisionMaker, Is.True);
        
        lobby.HandleAction(new WhoAmIDecisionAction(players[2], false));
        
        Assert.That(((WhoAmIInGameInfo)players[0].InGameInfo!).GameRole, Is.EqualTo(WhoAmIRole.Player));
        Assert.That(((WhoAmIInGameInfo)players[1].InGameInfo!).GameRole, Is.EqualTo(WhoAmIRole.Observer));
        Assert.That(((WhoAmIInGameInfo)players[2].InGameInfo!).GameRole, Is.EqualTo(WhoAmIRole.Player));
        Assert.That(((WhoAmIInGameInfo)players[3].InGameInfo!).GameRole, Is.EqualTo(WhoAmIRole.Guesser));
        Assert.That(((WhoAmIInGameInfo)players[4].InGameInfo!).GameRole, Is.EqualTo(WhoAmIRole.Player));
        Assert.That(((WhoAmIInGameInfo)players[5].InGameInfo!).GameRole, Is.EqualTo(WhoAmIRole.Player));
        
        Assert.That(((WhoAmIInGameInfo)players[2].InGameInfo!).IsDecisionMaker, Is.True);
        
        lobby.HandleAction(new WhoAmIDecisionAction(players[3], false));
        
        Assert.That(((WhoAmIInGameInfo)players[0].InGameInfo!).GameRole, Is.EqualTo(WhoAmIRole.Player));
        Assert.That(((WhoAmIInGameInfo)players[1].InGameInfo!).GameRole, Is.EqualTo(WhoAmIRole.Observer));
        Assert.That(((WhoAmIInGameInfo)players[2].InGameInfo!).GameRole, Is.EqualTo(WhoAmIRole.Player));
        Assert.That(((WhoAmIInGameInfo)players[3].InGameInfo!).GameRole, Is.EqualTo(WhoAmIRole.Player));
        Assert.That(((WhoAmIInGameInfo)players[4].InGameInfo!).GameRole, Is.EqualTo(WhoAmIRole.Guesser));
        Assert.That(((WhoAmIInGameInfo)players[5].InGameInfo!).GameRole, Is.EqualTo(WhoAmIRole.Player));
        
        Assert.That(((WhoAmIInGameInfo)players[3].InGameInfo!).IsDecisionMaker, Is.True);
        
        lobby.HandleAction(new WhoAmIDecisionAction(players[4], false));
        lobby.HandleAction(new WhoAmIDecisionAction(players[5], false));
        
        Assert.That(((WhoAmIInGameInfo)players[0].InGameInfo!).GameRole, Is.EqualTo(WhoAmIRole.Guesser));
        Assert.That(((WhoAmIInGameInfo)players[1].InGameInfo!).GameRole, Is.EqualTo(WhoAmIRole.Observer));
        Assert.That(((WhoAmIInGameInfo)players[2].InGameInfo!).GameRole, Is.EqualTo(WhoAmIRole.Player));
        Assert.That(((WhoAmIInGameInfo)players[3].InGameInfo!).GameRole, Is.EqualTo(WhoAmIRole.Player));
        Assert.That(((WhoAmIInGameInfo)players[4].InGameInfo!).GameRole, Is.EqualTo(WhoAmIRole.Player));
        Assert.That(((WhoAmIInGameInfo)players[5].InGameInfo!).GameRole, Is.EqualTo(WhoAmIRole.Player));
        
        Assert.That(((WhoAmIInGameInfo)players[5].InGameInfo!).IsDecisionMaker, Is.True);
        
        lobby.HandleAction(new WhoAmIDecisionAction(players[0], false));
        
        Assert.That(((WhoAmIInGameInfo)players[0].InGameInfo!).GameRole, Is.EqualTo(WhoAmIRole.Player));
        Assert.That(((WhoAmIInGameInfo)players[1].InGameInfo!).GameRole, Is.EqualTo(WhoAmIRole.Observer));
        Assert.That(((WhoAmIInGameInfo)players[2].InGameInfo!).GameRole, Is.EqualTo(WhoAmIRole.Guesser));
        Assert.That(((WhoAmIInGameInfo)players[3].InGameInfo!).GameRole, Is.EqualTo(WhoAmIRole.Player));
        Assert.That(((WhoAmIInGameInfo)players[4].InGameInfo!).GameRole, Is.EqualTo(WhoAmIRole.Player));
        Assert.That(((WhoAmIInGameInfo)players[5].InGameInfo!).GameRole, Is.EqualTo(WhoAmIRole.Player));
        
        Assert.That(((WhoAmIInGameInfo)players[1].InGameInfo!).IsDecisionMaker, Is.True);
        
        lobby.HandleAction(new WhoAmIDecisionAction(players[2], true));
        lobby.HandleAction(new WhoAmIDecisionAction(players[3], true));
        lobby.HandleAction(new WhoAmIDecisionAction(players[4], true));
        lobby.HandleAction(new WhoAmIDecisionAction(players[5], true));
        lobby.HandleAction(new WhoAmIDecisionAction(players[0], true));
        
        Assert.That(game.IsInProgress, Is.False);
        Assert.That(game.IsFinished, Is.True);
        foreach (var player in players)
        {
            Assert.That(((WhoAmIInGameInfo)player.InGameInfo!).GameRole, Is.EqualTo(WhoAmIRole.Observer));
        }
    }
}