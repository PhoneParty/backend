namespace Infrastructure.WhoAmI;

public class Hero(HeroEnum @enum, string name, string picture)
{
    public HeroEnum Enum { get; } = @enum;
    public string Name { get; } = name;
    public string Picture { get; } = picture;
}