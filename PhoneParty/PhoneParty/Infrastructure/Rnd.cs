using System.Text;

namespace PhoneParty.Hubs.Infastructure;

public static class RandomExtensions
{
    private static readonly char[] Chars = "ABCDEF0123456789".ToCharArray();

    public static string NextString(this Random rnd)
    {
        var lobbyId = new StringBuilder();
        for (var i = 0; i < 4; i++)
            lobbyId.Append(Chars[rnd.Next(Chars.Length)]);
        return lobbyId.ToString();
    }
}