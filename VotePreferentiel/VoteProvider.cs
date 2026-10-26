using System.Text.Json;

namespace VotePreferentiel;

public class VoteProvider(HttpClient httpClient)
{
    private const string DefaultVotesUrl = "https://raw.githubusercontent.com/JLou/dotnet-course/refs/heads/main/eval-1/votes.example.json";

    public async Task<VoteModel> GetVotesAsync(string? url = null, CancellationToken cancellationToken = default)
    {
        var json = await httpClient.GetStringAsync(url ?? DefaultVotesUrl, cancellationToken);
        var voteModel = JsonSerializer.Deserialize<VoteModel>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return voteModel ?? throw new InvalidOperationException("Votes payload is empty or invalid.");
    }
}