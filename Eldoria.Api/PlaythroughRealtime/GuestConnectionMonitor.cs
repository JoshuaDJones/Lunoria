namespace Eldoria.Api.PlaythroughRealtime;

public sealed class GuestConnectionMonitor(
    GuestConnectionRegistry connections,
    ILogger<GuestConnectionMonitor> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try 
                {
                    await connections.CheckAsync(null, null, stoppingToken); 
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) 
                { 
                    break; 
                }
                catch (Exception)
                {
                    // Do not log invitation hashes or tokens. Delivery checks fail closed too.
                    logger.LogWarning("Guest connection eligibility check failed; it will be retried.");
                }
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { }
    }
}
