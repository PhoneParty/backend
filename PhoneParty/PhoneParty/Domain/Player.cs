using PhoneParty.Domain.Interfaces;

namespace PhoneParty.Domain;

public class Player
{
    public int Id { get; protected set; }
    public Lobby Lobby { get; protected set; }
    public IInGameInfo InGameInfo { get; protected set; }
}