using PhoneParty.Domain.Enums;
using PhoneParty.Domain.Interfaces;

namespace PhoneParty.Domain;

public class Lobby
{
    public LobbyId Id { get; private set; }
    private List<IPlayer> _players = new ();

    public Lobby(LobbyId id)
    {
        Id = id;
    }

    public PlayerRegistrationResult RegisterPlayer(IPlayer player)
    {
        throw new NotImplementedException();
    }
}