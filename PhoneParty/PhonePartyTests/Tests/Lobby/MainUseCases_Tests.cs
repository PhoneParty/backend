using System.Reflection;
using NUnit.Framework;
using PhoneParty.Domain;

namespace PhonePartyTests.Tests.Lobby;

[TestFixture]
public class MainUseCase_Tests
{
    [Test]
    public void TestLobbyCreation()
    {
        var player = new Player(1);
        var lobby = new PhoneParty.Domain.Lobby(new LobbyId("4F3B"), player);

        var playersField = typeof(PhoneParty.Domain.Lobby)
            .GetField("_players", BindingFlags.NonPublic | BindingFlags.Instance);

        var players = playersField?.GetValue(lobby) as IList<Player>;

        Assert.That(players, Is.Not.Null);
        Assert.That(players.Count, Is.EqualTo(1));
        Assert.That(players[0], Is.EqualTo(player));
        Assert.That(player.Lobby, Is.EqualTo(lobby));
    }
}