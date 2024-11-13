using Infrastructure;
using PhoneParty.Domain;
using PhoneParty.Domain.AbstractClasses;
using PhoneParty.Domain.Enums;
using Action = PhoneParty.Domain.AbstractClasses.Action;
using InvalidOperationException = System.InvalidOperationException;

namespace Domain;

public class Lobby: Entity<LobbyId>
{
    public event Action<IEnumerable<Player>> GameStateChanged;

    private readonly List<Player> _players = new();
    private readonly Player _host;
    private Game? _game = null;

    public Lobby(LobbyId id, Player host) : base(id)
    {
        _host = host;
        _host.Lobby = this;
        _players.Add(_host);
    }

    private void GameStateChangedHandler(IEnumerable<Player> argument)
    {
        GameStateChanged.Invoke(argument);
    }

    public void HandleAction(Action action)
    {
        if (_game is null) throw new InvalidOperationException("Game is not defined");
        _game.HandleAction(action);
    }

    public GameStatusCheck CheckIfCanChangeGame(Game game)
    {
        if (_game is null) return GameStatusCheck.Correct;
        if (_game.IsInProgress) return GameStatusCheck.GameInProgress;
        return GameStatusCheck.Correct;
    }

    public GameStatusCheck CheckIfCanStartGame()
    {
        if (_game is null) return GameStatusCheck.NoGameDefined;
        if (_players.Count < _game.MinimumPlayers) return GameStatusCheck.LessThenMinimumAmountOfPlayers;
        if (_players.Count > _game.MaximumPlayers) return GameStatusCheck.MoreThenMaximumAmountOfPlayers;
        if (_game.IsInProgress) return GameStatusCheck.GameInProgress;
        return GameStatusCheck.Correct;
    }
    
    public void ChangeGame(Game game)
    {
        var check = CheckIfCanChangeGame(game);
        if (check != GameStatusCheck.Correct) throw new InvalidOperationException($"Can`t change game due to {check}");
        if (_game is not null) _game.GameStateChanged -= GameStateChangedHandler;
        _game = game;
        _game.GameStateChanged += GameStateChangedHandler;
        _game.ConnectPlayers(_players);
    }

    public void StartGame()
    {
        var check = CheckIfCanStartGame();
        if (check == GameStatusCheck.Correct)
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
    
    public PlayerKickResult KickPlayer(Player player)
    {
        if (_game is not null && _game.IsInProgress) return PlayerKickResult.GameInProgress;
        _players.Remove(player);
        return PlayerKickResult.SuccessfulKicked;
    }

    public IReadOnlyList<Player> GetPlayers => _players;

    public int PlayersCount => _players.Count;

    public Player Host => _host;

    public void CloseGame()
    {
        if (_game is null) throw new InvalidOperationException("Can`t close game due to it`s not defined");
        _game.CloseGame();
    }
}