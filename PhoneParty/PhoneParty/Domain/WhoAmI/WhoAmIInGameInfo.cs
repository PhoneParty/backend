using PhoneParty.Domain.Interfaces;

namespace PhoneParty.Domain.WhoAmI;

public class WhoAmIInGameInfo : IInGameInfo
{
    public WhoAmIInGameInfo(FileInfo attachedPicture)
    {
        AttachedPicture = attachedPicture;
    }

    public int Points { get; private set; } = 0;
    public bool IsDecisionMaker { get; set; } = false;
    public string? OnScreenText { get; private set; }
    public FileInfo AttachedPicture { get; private set; }
    public FileInfo? OnScreenPicture { get; private set; }
}