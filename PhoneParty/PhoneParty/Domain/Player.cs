using PhoneParty.Domain.Interfaces;

namespace PhoneParty.Domain;

public class Player
{
    public int Id { get; private set; }
    public Lobby? Lobby { get; set; }
    public IInGameInfo? InGameInfo { get; set; }

    public Player(int id)
    {
        Id = id;
    }
}