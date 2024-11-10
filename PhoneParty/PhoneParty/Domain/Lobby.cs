using PhoneParty.Domain.AbstractClasses;
using PhoneParty.Domain.Enums;
using InvalidOperationException = System.InvalidOperationException;

namespace PhoneParty.Domain;

public class Lobby
{
    public LobbyId Id { get; private set; }
    public event Action<IEnumerable<Player>> GameStateChanged;

    private readonly List<Player> _players = new();
    private readonly Player _host;
    private Game? _game = null;

    public Lobby(LobbyId id, Player host)
    {
        Id = id;
        _host = host;
        _host.Lobby = this;
    }

    private void GameStateChangedHandler(IEnumerable<Player> argument)
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

    public GameStartCheck CheckIfCanStartGame()
    {
        if (_game is null) return GameStartCheck.NoGameDefined;
        if (_players.Count < _game.MinimumPlayers) return GameStartCheck.LessThenMinimumAmountOfPlayers;
        if (_players.Count > _game.MaximumPlayers) return GameStartCheck.MoreThenMaximumAmountOfPlayers;
        if (_game.IsInProgress) return GameStartCheck.GameInProgress;
        return GameStartCheck.Successful;
    }

    public void StartGame()
    {
        var check = CheckIfCanStartGame();
        if (check == GameStartCheck.Successful)
        {
            _game.ConnectPlayers(_players);
            _game.StartGame();
        }
        else throw new InvalidOperationException($"Can`t start game due to {check}");
    }

    public PlayerRegistrationResult RegisterPlayer(Player player)
    {
        if (_game is not null && _game.IsInProgress) return PlayerRegistrationResult.GameInProgress;
        if (!_players.Contains(player)) _players.Add(player);
        return PlayerRegistrationResult.SuccessfulRegistered;
    }

    public void CloseGame()
    {
        if (_game is null) throw new InvalidOperationException($"Can`t close game due to it`s not defined");
        _game.CloseGame();
    }
}