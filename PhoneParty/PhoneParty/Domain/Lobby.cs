using PhoneParty.Domain.AbstractClasses;
using PhoneParty.Domain.Enums;
using PhoneParty.Domain.Interfaces;

namespace PhoneParty.Domain;

public class Lobby
{
    public LobbyId Id { get; private set; }
    public event Action<IEnumerable<IDifference>> GameStateChanged;

    private List<Player> _players = new();
    private Player _host;
    private Game? _game = null;

    public Lobby(LobbyId id, Player host)
    {
        Id = id;
        _host = host;
    }
    
    private void CheckGameNullability()
    {
        if (_game is null) throw new NullReferenceException("There is no game defined");
    }

    private void UnsubscribeFromOldEvent()
    {
        if (_game is null) return;
        foreach (var invocation in GameStateChanged.GetInvocationList())
        {
            _game.GameStateChanged -= (Action<IEnumerable<IDifference>>)invocation;
        } 
    }

    private void SubscribeToNewEvent()
    {
        CheckGameNullability();
        foreach (var invocation in GameStateChanged.GetInvocationList())
        {
            _game.GameStateChanged += (Action<IEnumerable<IDifference>>)invocation;
        }
    }
    public void ChangeGame(Game game)
    {
        UnsubscribeFromOldEvent();
        _game = game;
        SubscribeToNewEvent();
        _game.ConnectPlayers(_players);
    }
    
    public void StartGame(Game game)
    {
        CheckGameNullability();
        _game.StartGame();
    }

    public PlayerRegistrationResult RegisterPlayer(Player player)
    {
        CheckGameNullability();
        return _game.RegisterPlayer(player);
    }
}