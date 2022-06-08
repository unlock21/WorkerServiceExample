using EfModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Workers;
using Xunit;

namespace Service.Tests;

public class WorkerUnitTests
{
    [Fact]
    public async Task Worker1_Runs()
    {
        var services = new ServiceMockHost().Services;
        var worker1 = services.GetService<Worker1>();

        await worker1.DoWork();

        Assert.True(true);
    }

    [Fact]
    public async Task Worker2_CreatesBlogPost()
    {
        var services = new ServiceMockHost().Services;
        var worker2 = services.GetService<Worker2>();
        using var db = await services.GetService<IDbContextFactory<BloggingContext>>().CreateDbContextAsync();

        Assert.False(await db.Blogs.AnyAsync());

        await worker2.DoWork();

        Assert.True(await db.Blogs.AnyAsync());
    }

    [Fact]
    public async Task Worker2_CreatesOneBlogPostPerCycle()
    {
        var services = new ServiceMockHost().Services;
        var worker2 = services.GetService<Worker2>();
        using var db = await services.GetService<IDbContextFactory<BloggingContext>>().CreateDbContextAsync();

        Assert.False(await db.Blogs.AnyAsync());

        await worker2.DoWork();
        await worker2.DoWork();

        Assert.Equal(2, await db.Blogs.CountAsync());
    }
}

internal class ServiceMockHost
{
    internal ServiceProvider Services { get; } = new ServiceCollection()
        .AddLogging(cfg => cfg.ClearProviders())
        .AddSingleton<IConfiguration>(new ConfigurationBuilder().Build())
        .AddDbContextFactory<BloggingContext>(options => options.UseInMemoryDatabase("Test", new InMemoryDatabaseRoot()))
        .AddScoped<Worker1>()
        .AddScoped<Worker2>()
        .BuildServiceProvider();
}