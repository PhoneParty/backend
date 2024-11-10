namespace PhoneParty.Hubs.UserInterface.Interfaces;

public class PlayersRep: IPlayersRep<User>
{
    public Dictionary<string, HashSet<User>> Lobbies;

    public PlayersRep()
    {
        Lobbies = new Dictionary<string, HashSet<User>>();
    }

    public bool Contains(string value)
    {
        return Lobbies.ContainsKey(value);
    }

    public void AddValue(string key, HashSet<User> value)
    {
        Lobbies[key] = value;
    }

    public HashSet<User> GetValue(string key)
    {
        return Lobbies[key];
    }

    public void Remove(string key)
    {
        Lobbies.Remove(key);
    }
}