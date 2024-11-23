using Infrastructure;
using PhoneParty.Domain;
using PhoneParty.Domain.AbstractClasses;
using PhoneParty.Domain.Enums;
using Action = PhoneParty.Domain.AbstractClasses.Action;
using InvalidOperationException = System.InvalidOperationException;

namespace Domain;

public class Lobby : Entity<LobbyId>
{
    private readonly List<Player> _players = [];
    public Game? Game { get; private set; }
    public Player Host { get; private set; }

    public event Action<IEnumerable<Player>>? GameStateChanged;

    public Lobby(LobbyId id, Player host) : base(id)
    {
        Host = host;
        Host.Lobby = this;
        _players.Add(Host);
    }

    private void GameStateChangedHandler(IEnumerable<Player> argument)
    {
        GameStateChanged?.Invoke(argument);
    }

    public void HandleAction(Action action)
    {
        if (Game is null) throw new InvalidOperationException("Game is not defined");
        if (!_players.Contains(action.Player))
            throw new InvalidOperationException("Cannot handle action because player not in this lobby");
        Game.HandleAction(action);
    }

    public GameStatusCheck CheckIfCanChangeGame(Game game)
    {
        if (Game is null) return GameStatusCheck.Correct;
        if (Game.IsInProgress) return GameStatusCheck.GameInProgress;
        return GameStatusCheck.Correct;
    }

    public GameStatusCheck CheckIfCanStartGame()
    {
        if (Game is null) return GameStatusCheck.NoGameDefined;
        if (_players.Count < Game.MinimumPlayers) return GameStatusCheck.LessThenMinimumAmountOfPlayers;
        if (_players.Count > Game.MaximumPlayers) return GameStatusCheck.MoreThenMaximumAmountOfPlayers;
        if (Game.IsInProgress) return GameStatusCheck.GameInProgress;
        return GameStatusCheck.Correct;
    }

    public void ChangeGame(Game game)
    {
        var check = CheckIfCanChangeGame(game);
        if (check != GameStatusCheck.Correct) throw new InvalidOperationException($"Can`t change game due to {check}");
        if (Game is not null) Game.GameStateChanged -= GameStateChangedHandler;
        Game = game;
        Game.GameStateChanged += GameStateChangedHandler;
        Game.ConnectPlayers(_players);
    }

    public void StartGame()
    {
        var check = CheckIfCanStartGame();
        if (check == GameStatusCheck.Correct)
        {
            Game.ConnectPlayers(_players);
            Game.StartGame();
        }
        else throw new InvalidOperationException($"Can`t start game due to {check}");
    }

    public PlayerRegistrationResult RegisterPlayer(Player player)
    {
        if (Game is not null && Game.IsInProgress) return PlayerRegistrationResult.GameInProgress;
        if (!_players.Contains(player)) _players.Add(player);
        else
            throw new InvalidOperationException($"This player (playerId: {player.Id}) already in lobby");
        return PlayerRegistrationResult.SuccessfulRegistered;
    }

    public PlayerKickResult KickPlayer(Player player)
    {
        if (Game is not null && Game.IsInProgress) return PlayerKickResult.GameInProgress;
        _players.Remove(player);
        ChangeHost();
        return PlayerKickResult.SuccessfulKicked;
    }
    
    private void ChangeHost()
    {
        if(_players.Count != 0)
            Host = _players[0];
    }

    public IReadOnlyList<Player> GetPlayers => _players;

    public int PlayersCount => _players.Count;

    public void CloseGame()
    {
        if (Game is null) throw new InvalidOperationException("Can`t close game due to it`s not defined");
        Game.CloseGame();
    }
}