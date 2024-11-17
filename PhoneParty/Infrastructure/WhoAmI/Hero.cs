namespace Infrastructure.WhoAmI;

public class Hero(HeroEnum @enum, string name, FileInfo picture)
{
    public HeroEnum Enum { get; } = @enum;
    public string Name { get; } = name;
    public FileInfo Picture { get; } = picture;
}