namespace VotePreferentiel.Worker;

public class Worker(
    ILogger<Worker> logger,
    IConfiguration configuration,
    IHostApplicationLifetime applicationLifetime,
    VoteProvider voteProvider,
    IVoteCounter voteCounter) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var voteUrl = configuration["Votes:Url"];
            var voteModel = await voteProvider.GetVotesAsync(voteUrl, stoppingToken);
            var winner = voteCounter.GetWinner(voteModel);

            logger.LogInformation("Winner: {winner}", winner);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Vote processing failed.");
        }
        finally
        {
            applicationLifetime.StopApplication();
        }

        await Task.CompletedTask;
    }
}