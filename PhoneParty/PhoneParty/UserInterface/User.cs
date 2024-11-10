using Ddd.Taxi.Infrastructure;
using NuGet.ContentModel;
using PhoneParty.Domain;

namespace PhoneParty.Hubs.UserInterface.Interfaces;

public class User: Entity<string>
{
    public Player player { get; private set; }
    public string UserName { get; private set;  }
    public string ConnectionId { get; private set;  }

    public User(string id) : base(id)
    {
        // player = new Player(Id);
    }

    public void SetName(string name) => UserName = name;

    public void SetConnection(string connectionId) => ConnectionId = connectionId;
}