namespace PhoneParty.Domain.WhoAmI;

public class WhoAmIDecisionAction : WhoAmIAction
{
    public bool CurrentPlayerGuessCorrectly { get; private set; }
}