namespace PhoneParty.Hubs.Infastructure;

public interface IMemoryRep
{
    public bool Contains(string value);
                                   
    public void AddValue(string key, HashSet<string> value);
                                   
    public HashSet<string> GetValue(string key);
                                   
    public void Remove(string key);
}