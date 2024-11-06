using PhoneParty.Domain.Enums;
using PhoneParty.Domain.Interfaces;

namespace PhoneParty.Domain.AbstractClasses;

public abstract class Game
{
    public bool IsFinished { get; private set; } = false;
    protected bool IsInProgress { get; set; } = false;
    protected List<Player> _players = new();
    public abstract void HandleAction(IAction action);
    public abstract void StartGame();
    public abstract PlayerRegistrationResult RegisterPlayer(Player player);

    public void ConnectPlayers(List<Player> players)
    {
        _players = players;
    }
    public event Action<IEnumerable<IDifference>> GameStateChanged;
}