using Infrastructure;
using PhoneParty.Domain.Enums.WhoAmI;
using PhoneParty.Domain.Interfaces;

namespace Domain.WhoAmI;

public class WhoAmIInGameInfo(int attachedHeroId) : IInGameInfo
{
    public int Points { get; set; } = 0;
    public WhoAmIRole GameRole { get; set; } = WhoAmIRole.Player;
    public int AttachedHeroId { get; } = attachedHeroId;
    public bool IsDecisionMaker { get; set; }
}