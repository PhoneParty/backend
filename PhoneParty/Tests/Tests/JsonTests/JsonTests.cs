using System.Text.Json;
using Domain.WhoAmI;
using NUnit.Framework;

namespace PhonePartyTests.Tests.JsonTests;


[TestFixture]
public class JsonTests
{
    [TestCase]
    public void CheckNames()
    {
        var jsonContent = File.ReadAllText("Heroes.json");
        var heroData = JsonSerializer.Deserialize<HeroRepository.HeroData>(jsonContent);
        var heroes = heroData!.Characters;
        var names = Directory.EnumerateFiles(@"..\..\..\..\UserInterface\wwwroot\Characters").Select(Path.GetFileName);
        foreach (var hero in heroes)
            Assert.That(names.Contains(hero.ImgName), "Error: Jpeg name " + hero.ImgName + "Character name " + hero.Name);
        
    }
    
}