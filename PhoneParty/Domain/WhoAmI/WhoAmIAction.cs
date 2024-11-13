using Domain;
using PhoneParty.Domain.AbstractClasses;
using PhoneParty.Domain.Interfaces;
using Action = PhoneParty.Domain.AbstractClasses.Action;

namespace PhoneParty.Domain.WhoAmI;

public abstract class WhoAmIAction : Action
{
    protected WhoAmIAction(Player player) : base(player)
    {
    }
}