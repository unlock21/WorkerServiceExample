using EfModel;
using Microsoft.EntityFrameworkCore;
using Workers;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "Windows Service Name";
    })
    .ConfigureServices(services =>
    {
        services.AddDbContextFactory<BloggingContext>(options => options.UseSqlite("Data Source=blogging.db"));
        services.AddHostedService<Worker1>();
        services.AddHostedService<Worker2>();
    })
    .Build();

var bloggingContext = host.Services.GetService<IDbContextFactory<BloggingContext>>().CreateDbContext();
//await bloggingContext.Database.EnsureDeletedAsync();
await bloggingContext.Database.EnsureCreatedAsync();

await host.RunAsync();

public partial class Program { }
