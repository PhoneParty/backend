using PhoneParty.Domain;

namespace Domain;

public static class LobbyIdGenerator
{
    private static readonly List<LobbyId> Ids = new ((int)Math.Pow(LobbyId.AllowedCharacters.Length, LobbyId.Length));

    static LobbyIdGenerator()
    {
        GenerateAllPossibleIds();
    }
    
    public static LobbyId GenerateLobbyId()
    {
        if (Ids.Count == 0) throw new InvalidOperationException("No LobbyId`s left.");
        var index = new Random().Next(0, Ids.Count);
        var result = Ids[index];
        Ids.RemoveAt(index);
        return result;
    }

    public static void RestoreId(LobbyId id) => Ids.Add(id);
    
    private static void GenerateAllPossibleIds()
    {
        Ids.AddRange(
            from firstChar in LobbyId.AllowedCharacters
            from secondChar in LobbyId.AllowedCharacters
            from thirdChar in LobbyId.AllowedCharacters
            from fourthChar in LobbyId.AllowedCharacters
            select new LobbyId($"{firstChar}{secondChar}{thirdChar}{fourthChar}")
        );
    }
    
}