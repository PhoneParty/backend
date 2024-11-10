using System.Text;

namespace PhoneParty.Hubs.Infastructure;

public static class RandomExtensions
{
    private const string Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static string NextString(this Random rnd)
    {
        var lobbyId = new StringBuilder();
        for (var i = 0; i < 4; i++)
            lobbyId.Append(Chars[rnd.Next(Chars.Length)]);
        return lobbyId.ToString();
    }
}