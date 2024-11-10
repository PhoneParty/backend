using NuGet.ContentModel;
using PhoneParty.Domain;

namespace PhoneParty.Hubs.UserInterface.Interfaces;

public class User
{
    public int Id { get; private set; }
    public Player player { get; private set; }
    public string UserName { get; private set;  }
    public string ConnectionId { get; private set;  }

    public User(string userName, string connectionId)
    {
        Id = new Random().Next(); //TODO Id заглушка
        UserName = userName;
        ConnectionId = connectionId;
        player = new Player(Id);
    }
}