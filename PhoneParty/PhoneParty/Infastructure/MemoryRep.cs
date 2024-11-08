namespace PhoneParty.Hubs.Infastructure;

public class MemoryRep: IMemoryRep
{
    public Dictionary<string, HashSet<string>> Lobbies;

    public MemoryRep()
    {
        Lobbies = new Dictionary<string, HashSet<string>>();
    }

    public bool Contains(string value)
    {
        return Lobbies.ContainsKey(value);
    }

    public void AddValue(string key, HashSet<string> value)
    {
        Lobbies[key] = value;
    }

    public HashSet<string> GetValue(string key)
    {
        return Lobbies[key];
    }

    public void Remove(string key)
    {
        Lobbies.Remove(key);
    }
}