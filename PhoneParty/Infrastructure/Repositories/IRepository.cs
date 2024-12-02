namespace PhoneParty.Hubs.UserInterface.Interfaces.Repositories;

public interface IRepository<in TId, TValue>
{
    public bool Get(TId id, out TValue? value);

    public bool Add(TId id, TValue value);
    public bool Remove(TId id);

    public bool Contains(TId id);
}