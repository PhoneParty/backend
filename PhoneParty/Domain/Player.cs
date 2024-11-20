using System.Text.Json.Serialization;
using Infrastructure;
using PhoneParty.Domain.Interfaces;

namespace Domain;

public class Player(string id) : Entity<string>(id)
{
    [JsonIgnore]
    public Lobby? Lobby { get; set; }
    public IInGameInfo? InGameInfo { get; set; }
}