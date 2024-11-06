using PhoneParty.Domain.AbstractClasses;
using PhoneParty.Domain.Enums;
using PhoneParty.Domain.Interfaces;

namespace PhoneParty.Domain.WhoAmI;

public class WhoAmIGame: Game
{
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
    }

    public override PlayerRegistrationResult RegisterPlayer(Player player)
    {
        throw new NotImplementedException();
    }
}