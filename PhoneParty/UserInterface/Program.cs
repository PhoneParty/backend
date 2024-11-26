using PhoneParty;
using UserInterface;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80);
    options.ListenAnyIP(443, listenOptions => listenOptions.UseHttps("/etc/letsencrypt/live/www.phoneparty.fun/fullchain.pem",
        "/etc/letsencrypt/live/www.phoneparty.fun/privkey.pem"));
});
var start = new Startup();
start.ConfigureServices(builder.Services);
var app = builder.Build();
app.UseHttpsRedirection();
start.Configure(app);
app.Run();