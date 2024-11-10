namespace PhoneParty.Hubs.Infastructure;

public class MemoryRep: IMemoryRep<User>
{
    public Dictionary<string, HashSet<User>> Lobbies;

    public MemoryRep()
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