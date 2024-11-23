namespace Infrastructure;

public static class UserIdGenerator
{
    private static readonly List<string> IdList = [];

    public static string GenerateUserId()
    {
        var userId = Guid.NewGuid().ToString();
        
        IdList.Add(userId);

        return userId;
    }

    public static List<string> GetAllOccupiedIds() => IdList;
}