using System.Text.Json;

namespace Infrastructure.WhoAmI;

public static class HeroRepository
{
    private static readonly Dictionary<HeroEnum, Hero> _heroes;
    private const string JsonFilePath = "Heroes.json";

    static HeroRepository()
    {
        Console.WriteLine(Directory.GetCurrentDirectory());
        if (!File.Exists(JsonFilePath))
            throw new FileNotFoundException($"JSON file not found at {JsonFilePath}");

        var jsonContent = File.ReadAllText(JsonFilePath);
        var heroesFromJson = JsonSerializer.Deserialize<List<JsonHero>>(jsonContent);

        if (heroesFromJson == null)
            throw new InvalidOperationException("Failed to deserialize JSON.");

        _heroes = heroesFromJson
            .Select((hero, index) => new Hero(
                (HeroEnum)index,
                hero.Name,
                new FileInfo(hero.ImgName)))
            .ToDictionary(hero => hero.Enum, hero => hero);
    }

    public static Hero GetHero(HeroEnum heroEnum)
    {
        if (_heroes.TryGetValue(heroEnum, out var hero)) return hero;

        throw new KeyNotFoundException($"Hero with enum {heroEnum} not found.");
    }

    private class JsonHero
    {
        public string Name { get; set; }
        public string ImgName { get; set; }
    }
}