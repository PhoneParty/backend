using PhoneParty;

var builder = WebApplication.CreateBuilder(args);
var start = new Startup();
start.ConfigureServices(builder.Services);
var app = builder.Build();
start.Configure(app);
app.Run();