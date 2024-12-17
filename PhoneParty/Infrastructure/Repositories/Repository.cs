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
    
    
    public bool TryGet(TId? id, out TValue? value)
    {
        if (id != null && repo.TryGetValue(id, out var val))
        {
            value = val;
            return true;
        }

        value = default;
        return false;
    }

    public bool TryAdd(TId? id, TValue value) => id != null && repo.TryAdd(id, value);

    public bool TryRemove(TId? id) => id != null && repo.TryRemove(id, out TValue value);

    public bool Contains(TId? id) => id != null && repo.ContainsKey(id);
}