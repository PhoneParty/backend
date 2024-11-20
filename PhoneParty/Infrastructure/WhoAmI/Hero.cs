namespace Infrastructure.WhoAmI;

public class Hero(int id, string name, string picture)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
    public string Picture { get; } = picture;
}