using PhoneParty.Hubs.Infastructure;
using PhoneParty.Hubs.UserInterface.Interfaces;
using PhoneParty.Hubs.UserInterface.Interfaces.Repositories;

namespace PhoneParty.Hubs;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddSignalR();
        services.AddSingleton<IMemoryRep<User>, MemoryRep>();
        services.AddSingleton<IRepository<string, User>, Repository<string, User>>();
    }
 
    public void Configure(IApplicationBuilder app)
    {
        app.UseDeveloperExceptionPage();
 
        app.UseDefaultFiles();
        app.UseStaticFiles();
 
        app.UseRouting();
 
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapHub<LobbyHub>("/lobbyHub");
        });
    }
}