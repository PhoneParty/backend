using PhoneParty.Domain.AbstractClasses;
using PhoneParty.Domain.Enums;
using PhoneParty.Domain.Interfaces;

namespace PhoneParty.Domain.WhoAmI;

public class WhoAmIGame: Game
{
    private const int MaximumPlayers = 6;
    private void RebasePlayersInGameInfo()
    {
        foreach (var player in _players)
        {
            //TODO Сделать рандомный выбор картинок
            player.InGameInfo = new WhoAmIInGameInfo(new FileInfo("aboba"));
        }
    }
    public override void HandleAction(IAction action)
    {
        if (action is not WhoAmIAction) throw new ArgumentException($"{action.GetType()} is not valid for {GetType()}");
        throw new NotImplementedException();
    }

    public override void StartGame()
    {
        if (IsInProgress) throw new InvalidOperationException("This Game already started");
        throw new NotImplementedException();
    }

    public override PlayerRegistrationResult RegisterPlayer(Player player)
    {
        if (_players.Count >= MaximumPlayers) return PlayerRegistrationResult.NoMoreSlots;
        if (IsInProgress) return PlayerRegistrationResult.GameInProgress;
        //TODO RebasePlayerInGameInfo(player)
        _players.Add(player);
        //TODO Invoke GameStateChangedEvent
        return PlayerRegistrationResult.SuccessfulRegistered;
    }
}