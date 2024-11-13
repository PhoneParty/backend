using Infrastructure;
using PhoneParty.Domain.Interfaces;

namespace Domain;

public class Player(string id) : Entity<string>(id)
{
    public Lobby? Lobby { get; set; }
    public IInGameInfo? InGameInfo { get; set; }
}