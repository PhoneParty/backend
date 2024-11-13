using Domain;

namespace PhoneParty.Domain.AbstractClasses;

public abstract class Action
{
    protected Action(Player player)
    {
        Player = player;
    }

    public Player Player { get; }
}