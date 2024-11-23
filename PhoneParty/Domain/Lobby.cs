using Domain.Enums;
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

    public GameStartingStatusCheck CheckIfCanChangeGame(Game game)
    {
        if (Game is null) return GameStartingStatusCheck.Correct;
        if (Game.State == GameState.InProgress) return GameStartingStatusCheck.GameInProgress;
        return GameStartingStatusCheck.Correct;
    }

    public GameStartingStatusCheck CheckIfCanStartGame()
    {
        if (Game is null) return GameStartingStatusCheck.NoGameDefined;
        if (_players.Count < Game.MinimumPlayers) return GameStartingStatusCheck.LessThenMinimumAmountOfPlayers;
        if (_players.Count > Game.MaximumPlayers) return GameStartingStatusCheck.MoreThenMaximumAmountOfPlayers;
        if (Game.State == GameState.InProgress) return GameStartingStatusCheck.GameInProgress;
        return GameStartingStatusCheck.Correct;
    }
    
    public PlayerRegistrationResult CheckIfCanRegisterPlayer(Player player)
    {
        if (Game is not null && Game.State == GameState.InProgress) return PlayerRegistrationResult.GameInProgress;
        if (_players.Contains(player)) return PlayerRegistrationResult.PlayerAlreadyRegistered;
        return PlayerRegistrationResult.SuccessfulRegistered;
    }
    
    public PlayerKickResult CheckIfCanKickPlayer(Player player)
    {
        if (Game is not null && Game.State == GameState.InProgress) return PlayerKickResult.GameInProgress;
        if (!_players.Contains(player)) return PlayerKickResult.PlayerNotInLobby;
        return PlayerKickResult.SuccessfulKicked;
    }

    public void ChangeGame(Game game)
    {
        var check = CheckIfCanChangeGame(game);
        if (check != GameStartingStatusCheck.Correct) throw new InvalidOperationException($"Can`t change game due to {check}");
        if (Game is not null) Game.GameStateChanged -= GameStateChangedHandler;
        Game = game;
        Game.GameStateChanged += GameStateChangedHandler;
        Game.ConnectPlayers(_players);
    }

    public void StartGame()
    {
        var check = CheckIfCanStartGame();
        if (check == GameStartingStatusCheck.Correct)
        {
            Game.ConnectPlayers(_players);
            Game.StartGame();
        }
        else throw new InvalidOperationException($"Can`t start game due to {check}");
    }

    public void RegisterPlayer(Player player)
    {
        var check = CheckIfCanRegisterPlayer(player);
        if (check == PlayerRegistrationResult.SuccessfulRegistered) _players.Add(player);
        else throw new InvalidOperationException($"Can`t register player due to {check}");
    }

    public void KickPlayer(Player player)
    {
        var check = CheckIfCanKickPlayer(player);
        if (check == PlayerKickResult.SuccessfulKicked)
        {
            _players.Remove(player);
            ChangeHost();
        }
        else throw new InvalidOperationException($"Can`t kick player due to {check}");
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