using Domain;
using Infrastructure;

namespace Application;

public class WebApplicationUser: Entity<string>
{
    public Player Player { get; private set; }
    public string UserName { get; private set;  }
    public string ConnectionId { get; private set;  }

    public WebApplicationUser(string id) : base(id)
    {
        Player = new Player(Id);
    }

    public void SetName(string name) => UserName = name;

    public void SetConnection(string connectionId) => ConnectionId = connectionId;
}