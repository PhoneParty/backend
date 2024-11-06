using Microsoft.AspNetCore;

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

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        
    }

    public void Configure(IApplicationBuilder app, IHostEnvironment env)
    {
        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();
        app.Run(async context => await context.Response.WriteAsync("Hello World!"));
    }
}