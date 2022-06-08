namespace Service;

public class WorkerBase<T> : BackgroundService
{
    public readonly ILogger<T> _logger;
    public readonly WorkerConfig WorkerConfig;

    public WorkerBase(ILogger<T> logger, IConfiguration config)
    {
        _logger = logger;
        WorkerConfig = config.GetSection("Workers:" + GetType().Name).Get<WorkerConfig>();
    }

    protected Func<Task> WorkerAction { get; set; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (WorkerAction == null)
        {
            throw new NotImplementedException();
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            await WorkerAction();
            _logger.LogInformation("Worker tick at: {time}", DateTimeOffset.Now);
            var minutesBetweenActions = WorkerConfig.MinutesBetweenActions ?? 0;
            await Task.Delay((int)Math.Ceiling(minutesBetweenActions * 1000m), stoppingToken);
        }
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        if (WorkerConfig?.MinutesBetweenActions == null)
            return Task.CompletedTask;

        _logger.LogInformation("Worker starting at: {time}", DateTimeOffset.Now);

        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Worker stopping at: {time}", DateTimeOffset.Now);

        return base.StopAsync(cancellationToken);
    }
}
