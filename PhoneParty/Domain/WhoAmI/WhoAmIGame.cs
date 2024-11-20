using Infrastructure;
using Infrastructure.WhoAmI;
using PhoneParty.Domain;
using PhoneParty.Domain.AbstractClasses;
using PhoneParty.Domain.Enums.WhoAmI;
using PhoneParty.Domain.WhoAmI;
using Action = PhoneParty.Domain.AbstractClasses.Action;

namespace Domain.WhoAmI;

public class WhoAmIGame : Game
{
    public override int MaximumPlayers { get; protected set; } = 6;
    public override int MinimumPlayers { get; protected set; } = 2;
    public override event Action<IEnumerable<Player>>? GameStateChanged;
    private readonly List<int> _remainingHeroesIds = HeroRepository.AvailableHeroesIds;
    private int _currentGuesserIndex;
    private int _currentDecisionMakerIndex;

    private void RebasePlayersInGameInfo()
    {
        foreach (var player in Players) player.InGameInfo = new WhoAmIInGameInfo(GetRandomHeroId());
    }

    private int GetRandomHeroId()
    {
        if (_remainingHeroesIds.Count == 0) throw new InvalidOperationException("There is no more remaining heroes");
        var randomHero = _remainingHeroesIds[new Random().Next(_remainingHeroesIds.Count)];
        _remainingHeroesIds.Remove(randomHero);
        return randomHero;
    }

    public int CurrentGuessedHeroId => ((WhoAmIInGameInfo)Players[_currentGuesserIndex].InGameInfo!).AttachedHeroId;

    public override void HandleAction(Action action)
    {
        if (!IsInProgress) throw new InvalidOperationException("Game haven`t started");
        if (action is not WhoAmIAction whoAmIAction)
            throw new InvalidOperationException($"{action.GetType()} is not valid for {GetType()}");
        var playerGuessed = ((WhoAmIDecisionAction)whoAmIAction).CurrentPlayerGuessedCorrectly;
        if (playerGuessed) HandlePlayerSuccess(Players[_currentGuesserIndex]);
        HandleNextMove();
        GameStateChanged?.Invoke(Players);
    }

    private void HandleNextMove()
    {
        for (var i = 1; i <= Players.Count; i++)
        {
            if (((WhoAmIInGameInfo)Players[(_currentGuesserIndex + i) % Players.Count].InGameInfo!).GameRole ==
                WhoAmIRole.Observer) continue;
            if (Players[_currentGuesserIndex].InGameInfo is WhoAmIInGameInfo { GameRole: WhoAmIRole.Guesser } info)
            {
                info.GameRole = WhoAmIRole.Player;
            }

            _currentGuesserIndex = (_currentGuesserIndex + i) % Players.Count;
            ((WhoAmIInGameInfo)Players[_currentGuesserIndex].InGameInfo!).GameRole = WhoAmIRole.Guesser;
            break;
        }

        ((WhoAmIInGameInfo)Players[_currentDecisionMakerIndex].InGameInfo!).IsDecisionMaker = false;
        _currentDecisionMakerIndex = _currentGuesserIndex == 0 ? Players.Count - 1 : _currentGuesserIndex - 1;
        ((WhoAmIInGameInfo)Players[_currentDecisionMakerIndex].InGameInfo!).IsDecisionMaker = true;

        if (Players.Any(player => ((WhoAmIInGameInfo)player.InGameInfo!).GameRole != WhoAmIRole.Observer)) return;
        IsInProgress = false;
        IsFinished = true;
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
        RebasePlayersInGameInfo(); // Вот тут вот надо будет добавить чтобы список героев обновлялся между перезапусками игры (именно игры а не раундов)
        IsInProgress = true;
        IsFinished = false;
        _currentDecisionMakerIndex = Players.Count - 1;
        ((WhoAmIInGameInfo)Players[_currentDecisionMakerIndex].InGameInfo!).IsDecisionMaker = true;
        _currentGuesserIndex = 0;
        ((WhoAmIInGameInfo)Players[_currentGuesserIndex].InGameInfo!).GameRole = WhoAmIRole.Guesser;
    }
}