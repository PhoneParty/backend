using System.Collections.Concurrent;
using PhoneParty.Hubs.UserInterface.Interfaces.Repositories;

namespace Infrastructure.Repositories;

public class Repository<TId, TValue>: IRepository<TId, TValue> where TId : notnull
{
    private static ConcurrentDictionary<TId, TValue> repo;

    public Repository()
    {
        repo = new ConcurrentDictionary<TId, TValue>();
    }
    
    
    public bool Get(TId id, out TValue? value)
    {
        if (repo.TryGetValue(id, out var val))
        {
            value = val;
            return true;
        }

        value = default;
        return false;
    }

    public bool Add(TId id, TValue value) => repo.TryAdd(id, value);

    public void Remove(TId id) => repo.TryRemove(id, out TValue value);

    public bool Contains(TId id) => repo.ContainsKey(id);
}