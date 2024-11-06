using Microsoft.AspNetCore;
using PhoneParty;

public class Programm
{
    public static void Main(string[] args)
    {
        BuildWebHost(args).Run();
    }

    public static IWebHost BuildWebHost(string[] args)
    {
        return WebHost
            .CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .Build();
    }
}