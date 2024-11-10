using PhoneParty.Hubs.Infastructure;
using PhoneParty.Hubs.UserInterface.Interfaces;

namespace PhoneParty.Hubs;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddSignalR();
        services.AddSingleton<IMemoryRep<User>, MemoryRep>();
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