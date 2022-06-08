using EfModel;
using Microsoft.EntityFrameworkCore;
using Service;

namespace Workers;

public class Worker2 : WorkerBase<Worker2>
{
    private readonly BloggingContext _blogContext;

    public Worker2(ILogger<Worker2> logger, IConfiguration config, IDbContextFactory<BloggingContext> blogContextFactory) : base(logger, config)
    {
        _blogContext = blogContextFactory.CreateDbContext();

        WorkerAction = DoWork;
    }

    public async Task DoWork()
    {
        var blogCount = await _blogContext.Blogs.CountAsync();

        _logger.LogInformation($"{blogCount} Blogs");

        await _blogContext.Blogs.AddAsync(new Blog());

        await _blogContext.SaveChangesAsync();
    }
}