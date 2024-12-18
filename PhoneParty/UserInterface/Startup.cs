using Domain;
using Infrastructure.Repositories;
using PhoneParty.Domain;
using PhoneParty.Hubs.UserInterface.Interfaces.Repositories;
using Application.Hubs;
using Application;

namespace UserInterface;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddSignalR();
        services.AddSingleton<IRepository<LobbyId, Lobby>, Repository<LobbyId, Lobby>>();
        services.AddSingleton<IRepository<string, WebApplicationUser>, Repository<string, WebApplicationUser>>();
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
            endpoints.MapHub<WhoIAmHub>("/WhoIAmHub");
        });
    }
}