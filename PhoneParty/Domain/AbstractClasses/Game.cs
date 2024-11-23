using Domain;
using Domain.Enums;
using PhoneParty.Domain.Enums;
using PhoneParty.Domain.Interfaces;

namespace PhoneParty.Domain.AbstractClasses;

public abstract class Game
{
    public GameState State { get; protected set; } = GameState.WaitingForStart;
    protected List<Player> Players = [];
    public abstract void HandleAction(Action action);
    public abstract void StartGame();
    public abstract int MaximumPlayers { get; protected set; }
    public abstract int MinimumPlayers { get; protected set; }

    public void ConnectPlayers(List<Player> players)
    {
        Players = players;
    }
    
    public void CloseGame()
    {
        foreach (var player in Players)
        {
            player.InGameInfo = null;
        }
        Players.Clear();
        State = GameState.Closed;
    }
    public abstract event Action<IEnumerable<Player>> GameStateChanged;
}