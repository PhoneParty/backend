using Domain;

namespace PhoneParty.Domain.WhoAmI;

public class WhoAmIDecisionAction : WhoAmIAction
{
    public bool CurrentPlayerGuessedCorrectly { get; private set; }

    public WhoAmIDecisionAction(Player player) : base(player)
    {
    }
}