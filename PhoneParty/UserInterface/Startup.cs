using Domain;
using PhoneParty.Domain;
using PhoneParty.Hubs.UserInterface.Interfaces;
using PhoneParty.Hubs.UserInterface.Interfaces.Repositories;
using PhoneParty.UserInterface.Hubs;

namespace PhoneParty;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddSignalR();
        services.AddSingleton<IRepository<LobbyId, Lobby>, Repository<LobbyId, Lobby>>();
        services.AddSingleton<IRepository<string, User>, Repository<string, User>>();
        services.AddSingleton<LobbyHub>();
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