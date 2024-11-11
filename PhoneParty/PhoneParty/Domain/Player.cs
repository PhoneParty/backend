using Ddd.Taxi.Infrastructure;
using PhoneParty.Domain.Interfaces;

namespace PhoneParty.Domain;

public class Player(string id) : Entity<string>(id)
{
    public Lobby? Lobby { get; set; }
    public IInGameInfo? InGameInfo { get; set; }
}