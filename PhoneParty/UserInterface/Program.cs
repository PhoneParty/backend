using PhoneParty;
using UserInterface;

var builder = WebApplication.CreateBuilder(args);
#if !DEBUG
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80);
    options.ListenAnyIP(443, listenOptions => listenOptions.UseHttps("/etc/letsencrypt/live/www.phoneparty.fun/certificate.pfx",
        "certPassword"));
});
#endif
var start = new Startup();
start.ConfigureServices(builder.Services);
var app = builder.Build();
#if !DEBUG
app.UseHttpsRedirection();
#endif
start.Configure(app);
app.Run();