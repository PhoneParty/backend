using PhoneParty.Domain.Interfaces;

namespace PhoneParty.Domain.WhoAmI;

public class WhoAmIInGameInfo : IInGameInfo
{
    public WhoAmIInGameInfo(FileInfo attachedPicture)
    {
        AttachedPicture = attachedPicture;
    }

    public int Points { get; private set; } = 0;
    public string? OnScreenText { get; private set; }
    public bool IsInGame { get; private set; } = true;
    public FileInfo AttachedPicture { get; private set; }
    public FileInfo? OnScreenPicture { get; private set; }
}