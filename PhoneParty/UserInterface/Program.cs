using PhoneParty;
using UserInterface;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80);
    options.ListenAnyIP(443, listenOptions => listenOptions.UseHttps("/etc/letsencrypt/archive/www.phoneparty.fun/fullchain2.pem",
        "/etc/letsencrypt/archive/www.phoneparty.fun/privkey2.pem"));
});
var start = new Startup();
start.ConfigureServices(builder.Services);
var app = builder.Build();
app.UseHttpsRedirection();
start.Configure(app);
app.Run();