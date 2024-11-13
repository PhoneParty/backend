using Infrastructure;
using PhoneParty.Domain.Enums.WhoAmI;
using PhoneParty.Domain.Interfaces;

namespace Domain.WhoAmI;

public class WhoAmIInGameInfo : IInGameInfo
{
    public WhoAmIInGameInfo(HeroEnum attachedHero)
    {
        AttachedHero = attachedHero;
    }

    public int Points { get; set; } = 0;
    public WhoAmIRole GameRole { get; set; } = WhoAmIRole.Player;
    public HeroEnum AttachedHero { get;}
    public bool IsDecisionMaker { get; set; }
}