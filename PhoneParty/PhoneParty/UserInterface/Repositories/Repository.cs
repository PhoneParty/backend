namespace PhoneParty.Hubs.UserInterface.Interfaces.Repositories;

public class Repository<TId, TValue>: IRepository<TId, TValue> where TId : notnull
{
    private static Dictionary<TId, TValue> repo;

    public Repository()
    {
        repo = new Dictionary<TId, TValue>();
    }
    
    
    public TValue Get(TId id)
    {
        if (repo.TryGetValue(id, out var value))
            return value;
        throw new Exception("Unknown id " + id);
    }
    
    public void Add(TId id, TValue value)
    {
        if (!repo.TryAdd(id, value))
            throw new Exception($"Id {id} already in repository");
    }

    public void Remove(TId id) => repo.Remove(id);

    public bool Contains(TId id) => repo.ContainsKey(id);
}