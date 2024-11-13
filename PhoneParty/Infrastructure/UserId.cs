using System;
using System.Collections.Generic;
using System.IO;

public class UserId
{
    private static readonly List<string> IdList = [];

    public static string GenerateUserId()
    {
        var userId = Guid.NewGuid().ToString();
        
        IdList.Add(userId);

        return userId;
    }

    public static List<string> GetAllIds() => IdList;

}