namespace PhoneParty.Hubs.UserInterface.Interfaces;

public interface IPlayersRep<T>
{
    public bool Contains(string value);
                                   
    public void AddValue(string key, HashSet<T> value);
                                   
    public HashSet<T> GetValue(string key);
                                   
    public void Remove(string key);
}