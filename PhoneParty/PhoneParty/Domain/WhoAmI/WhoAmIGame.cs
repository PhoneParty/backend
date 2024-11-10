using PhoneParty.Domain.AbstractClasses;
using PhoneParty.Domain.Enums;
using PhoneParty.Domain.Enums.WhoAmI;
using PhoneParty.Domain.Interfaces;
using PhoneParty.Infrastructure;
using Action = PhoneParty.Domain.AbstractClasses.Action;

namespace PhoneParty.Domain.WhoAmI;

public class WhoAmIGame : Game
{
    public override int MaximumPlayers { get; protected set; } = 6;
    public override int MinimumPlayers { get; protected set; } = 2;
    public override event Action<IEnumerable<Player>>? GameStateChanged;
    private readonly List<HeroEnum> _remainingHeroes = Enum.GetValues(typeof(HeroEnum)).Cast<HeroEnum>().ToList();
    private int _currentGuesserIndex;
    private int _currentDescisionMakerIndex;

    private void RebasePlayersInGameInfo()
    {
        foreach (var player in Players) player.InGameInfo = new WhoAmIInGameInfo(GetRandomHero());
    }

    private HeroEnum GetRandomHero()
    {
        if (_remainingHeroes.Count == 0) throw new InvalidOperationException("There is no more remaining heroes");
        var randomHero = _remainingHeroes[new Random().Next(_remainingHeroes.Count)];
        _remainingHeroes.Remove(randomHero);
        return randomHero;
    }

    public override void HandleAction(Action action)
    {
        if (!IsInProgress) return;
        if (action is not WhoAmIAction whoAmIAction)
            throw new ArgumentException($"{action.GetType()} is not valid for {GetType()}");
        var playerGuessed = ((WhoAmIDecisionAction)whoAmIAction).CurrentPlayerGuessedCorrectly;
        if (playerGuessed) HandlePlayerSuccess(action.Player);
        HandleNextMove();
        GameStateChanged?.Invoke(Players);
    }

    private void HandleNextMove()
    {
        if (Players.All(player => ((WhoAmIInGameInfo)player.InGameInfo!).GameRole == WhoAmIRole.Observer))
        {
            IsInProgress = false;
            IsFinished = true;
            return;
        }
        
        for (var i = 0; i < Players.Count; i++)
        {
            if (((WhoAmIInGameInfo)Players[(_currentGuesserIndex + i) % Players.Count].InGameInfo!).GameRole ==
                WhoAmIRole.Observer) continue;
            if (Players[_currentGuesserIndex].InGameInfo is WhoAmIInGameInfo { GameRole: WhoAmIRole.Guesser } info)
            {
                info.GameRole = WhoAmIRole.Player;
            }
            _currentGuesserIndex = (_currentGuesserIndex + i) % Players.Count;
            break;
        }

        ((WhoAmIInGameInfo)Players[_currentDescisionMakerIndex].InGameInfo!).IsDecisionMaker = false;
        _currentDescisionMakerIndex = _currentGuesserIndex == 0 ? Players.Count - 1 : _currentGuesserIndex - 1;
    }

    private void HandlePlayerSuccess(Player player)
    {
        if (player.InGameInfo is not WhoAmIInGameInfo info)
            throw new InvalidOperationException(
                $"Player {player} have inappropriate InGameInfo {player.InGameInfo?.GetType()}");
        info.GameRole = WhoAmIRole.Observer;
    }

    public override void StartGame()
    {
        if (IsInProgress) throw new InvalidOperationException("This Game already started");
        if (IsFinished)
            RebasePlayersInGameInfo();
        IsInProgress = true;
        _currentDescisionMakerIndex = Players.Count - 1;
        _currentGuesserIndex = 0;
    }
}