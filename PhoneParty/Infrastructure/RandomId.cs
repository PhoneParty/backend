using System.Text;

namespace PhoneParty.Hubs.Infastructure;

public static class RandomIds
{
    private const string Alphabet = "ABCDEF0123456789";
    public static string GenerateUserId() => Guid.NewGuid().ToString();
    
    private static Stack<string> _ids = new Stack<string>();
    
    public static string GetLobbyId()
    {
        if(_ids.Count == 0)
            GetAllIds();
        return _ids.Pop();
    }

    public static void RestoreId(string id) => _ids.Push(id);
    
    private static void GetAllIds()
    {
        foreach (var firstChar in Alphabet)
            foreach (var secondChar in Alphabet)
                foreach (var thirdChar in Alphabet)
                    foreach (var fourthChar in Alphabet)
                        _ids.Push(firstChar.ToString() + secondChar.ToString() + thirdChar.ToString() + fourthChar.ToString());
    }
    
}