namespace PhoneParty.Hubs.UserInterface.Interfaces.Repositories;

public interface IRepository<in TId, TValue>
{
    public TValue Get(TId id);

    public void Add(TId id, TValue value);
    public void Remove(TId id);

    public bool Contains(TId id);
}