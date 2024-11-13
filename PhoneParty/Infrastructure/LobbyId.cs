using System.Text;

namespace PhoneParty.Hubs.Infastructure;

public static class LobbyId
{
    private const string Alphabet = "ABCDEF0123456789";
    
    private static readonly Stack<string> Ids = new Stack<string>();
    
    public static string GetLobbyId()
    {
        if(Ids.Count == 0)
            GetAllIds();
        return Ids.Pop();
    }

    public static void RestoreId(string id) => Ids.Push(id);
    
    private static void GetAllIds()
    {
        foreach (var firstChar in Alphabet)
            foreach (var secondChar in Alphabet)
                foreach (var thirdChar in Alphabet)
                    foreach (var fourthChar in Alphabet)
                        Ids.Push(firstChar.ToString() + secondChar.ToString() + thirdChar.ToString() + fourthChar.ToString());
    }
    
}