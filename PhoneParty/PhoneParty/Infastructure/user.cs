namespace PhoneParty.Hubs.Infastructure;

public class User
{
    public string userName;
    public string connectionId;

    public User(string name, string connectionId)
    {
        userName = name;
        this.connectionId = connectionId;
    }
}