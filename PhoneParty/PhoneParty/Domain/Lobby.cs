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
        _host.Lobby = this;
    }

    private void CheckGameNullability()
    {
        if (_game is null) throw new NullReferenceException("There is no game defined");
    }

    private void GameStateChangedHandler(IEnumerable<IDifference> argument)
    {
        GameStateChanged.Invoke(argument);
    }

    public void ChangeGame(Game game)
    {
        if (_game is not null) _game.GameStateChanged -= GameStateChangedHandler;
        _game = game;
        _game.GameStateChanged += GameStateChangedHandler;
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
        var result = _game.RegisterPlayer(player);
        if (result is PlayerRegistrationResult.SuccessfulRegistered) player.Lobby = this;
        return result;
    }
}