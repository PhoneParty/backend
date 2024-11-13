using System.Text;

namespace Infrastructure;

public static class RandomIds
{
    private static readonly char[] Chars = "ABCDEF0123456789".ToCharArray();
    public static string GenerateUserId() => Guid.NewGuid().ToString();
    
    public static string GenerateLobbyId()
    {
        var rnd = new Random();
        var lobbyId = new StringBuilder();
        for (var i = 0; i < 4; i++)
            lobbyId.Append(Chars[rnd.Next(Chars.Length)]);
        return lobbyId.ToString();
    }
}