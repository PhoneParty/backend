using PhoneParty.Domain.Enums;
using PhoneParty.Domain.Interfaces;

namespace PhoneParty.Domain;

public class Lobby
{
    public LobbyId Id { get; private set; }
    public event Action<IEnumerable<IDifference>> GameStateChanged;

    private List<Player> _players = new();
    private Player _host;
    private IGame? _game = null;

    public Lobby(LobbyId id)
    {
        Id = id;
    }

    private void UnsubscribeFromOldEvent()
    {
        if (_game is null) throw new NullReferenceException("There is no game defined");
        foreach (var invocation in GameStateChanged.GetInvocationList())
        {
            _game.GameStateChanged -= (Action<IEnumerable<IDifference>>)invocation;
        } 
    }

    private void SubscribeToNewEvent()
    {
        if (_game is null) throw new NullReferenceException("There is no game defined");
        foreach (var invocation in GameStateChanged.GetInvocationList())
        {
            _game.GameStateChanged += (Action<IEnumerable<IDifference>>)invocation;
        }
    }

    public PlayerRegistrationResult RegisterPlayer(Player player)
    {
        throw new NotImplementedException();
    }
}