using Domain;

namespace PhoneParty.Domain.WhoAmI;

public class WhoAmIDecisionAction(Player player, bool guessedCorrectly) : WhoAmIAction(player)
{
    public bool CurrentPlayerGuessedCorrectly { get; private set; } = guessedCorrectly;
}