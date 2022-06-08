using EfModel;
using Microsoft.EntityFrameworkCore;
using Service;

namespace Workers;

public class Worker1 : WorkerBase<Worker1>
{
    public Worker1(ILogger<Worker1> logger, IConfiguration config) : base(logger, config)
    {
        WorkerAction = DoWork;
    }

    public async Task DoWork()
    {
        await Task.CompletedTask;
    }
}
