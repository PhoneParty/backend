namespace PhoneParty.Domain.Interfaces;

public interface IPlayer
{
    public int Id { get; protected set; }
    public LobbyId LobbyId { get; protected set; }
}