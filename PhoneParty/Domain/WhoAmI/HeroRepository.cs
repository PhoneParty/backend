using System.Text.Json;

namespace Domain.WhoAmI;

public static class HeroRepository
{
    private static readonly Dictionary<int, Hero> Heroes;
    private const string JsonFilePath = "Heroes.json";
    public static List<int> AvailableHeroesIds => Heroes.Keys.ToList();

    static HeroRepository()
    {
        Console.WriteLine(Directory.GetCurrentDirectory());
        if (!File.Exists(JsonFilePath))
            throw new FileNotFoundException($"JSON file not found at {JsonFilePath}");

        var jsonContent = File.ReadAllText("Heroes.json");
        var heroData = JsonSerializer.Deserialize<HeroData>(jsonContent);
        var heroes = heroData!.Characters;

        if (heroes == null)
            throw new InvalidOperationException("Failed to deserialize JSON.");

        Heroes = heroes
            .Select(hero => new Hero(
                hero.Id,
                hero.Name,
                hero.ImgName))
            .ToDictionary(hero => hero.Id, hero => hero);
    }

    public static Hero GetHero(int heroId)
    {
        if (Heroes.TryGetValue(heroId, out var hero)) return hero;

        throw new KeyNotFoundException($"Hero with id {heroId} not found.");
    }

    public class JsonHero
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImgName { get; set; }
    }

    public class HeroData
    {
        public List<JsonHero> Characters { get; init; }
    }
}