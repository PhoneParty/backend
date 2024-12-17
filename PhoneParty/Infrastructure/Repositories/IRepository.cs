namespace PhoneParty.Hubs.UserInterface.Interfaces.Repositories;

public interface IRepository<in TId, TValue>
{
    public bool TryGet(TId id, out TValue? value);

    public bool TryAdd(TId id, TValue value);
    public bool TryRemove(TId id);

    public bool Contains(TId id);
}