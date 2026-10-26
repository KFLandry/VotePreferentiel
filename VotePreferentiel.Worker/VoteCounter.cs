namespace VotePreferentiel.Worker;

public class VoteCounter : IVoteCounter
{
    public HashSet<string> GetCandidates(VoteModel voteModel)
    {
        EnsureVotes(voteModel);

        var candidates = voteModel.Votes
            .SelectMany(v => v.Choices)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .ToHashSet(StringComparer.Ordinal);

        if (candidates.Count == 0)
        {
            throw new InvalidOperationException("No candidates found in votes.");
        }

        return candidates;
    }

    public string GetWinner(VoteModel voteModel)
    {
        EnsureVotes(voteModel);

        var activeCandidates = GetCandidates(voteModel);

        while (activeCandidates.Count > 1)
        {
            var firstChoices = GetFirstChoicesForActiveCandidates(voteModel, activeCandidates);
            var firstChoiceCounts = CountFirstChoices(firstChoices);

            foreach (var candidate in activeCandidates)
            {
                firstChoiceCounts.TryAdd(candidate, 0);
            }

            var leastVotedCandidates = GetCandidatesWithLeastVotes(firstChoiceCounts);

            if (leastVotedCandidates.Length == activeCandidates.Count)
            {
                throw new InvalidOperationException("Unable to determine a winner due to a complete tie.");
            }

            foreach (var candidate in leastVotedCandidates)
            {
                activeCandidates.Remove(candidate);
            }
        }

        return activeCandidates.Single();
    }

    public List<string> GetAllFirstChoices(VoteModel voteModel)
    {
        EnsureVotes(voteModel);

        var firstChoices = voteModel.Votes
            .Select(v => v.Choices.FirstOrDefault(c => !string.IsNullOrWhiteSpace(c)))
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Select(c => c!)
            .ToList();

        if (firstChoices.Count == 0)
        {
            throw new InvalidOperationException("No valid first choices found in votes.");
        }

        return firstChoices;
    }

    public IDictionary<string, int> CountFirstChoices(List<string> firstChoices)
    {
        if (firstChoices.Count == 0)
        {
            throw new InvalidOperationException("Cannot count first choices on an empty list.");
        }

        return firstChoices
            .GroupBy(choice => choice, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.Count(), StringComparer.Ordinal);
    }

    public string[] GetCandidatesWithLeastVotes(IDictionary<string, int> firstChoiceCounts)
    {
        if (firstChoiceCounts.Count == 0)
        {
            throw new InvalidOperationException("Cannot determine least votes from an empty count set.");
        }

        var minimumVotes = firstChoiceCounts.Min(x => x.Value);

        return firstChoiceCounts
            .Where(x => x.Value == minimumVotes)
            .Select(x => x.Key)
            .ToArray();
    }

    private static void EnsureVotes(VoteModel voteModel)
    {
        if (voteModel.Votes.Count == 0)
        {
            throw new InvalidOperationException("Votes collection is empty.");
        }
    }

    private static List<string> GetFirstChoicesForActiveCandidates(VoteModel voteModel, HashSet<string> activeCandidates)
    {
        var firstChoices = new List<string>(voteModel.Votes.Count);

        foreach (var ballot in voteModel.Votes)
        {
            var candidate = ballot.Choices.FirstOrDefault(activeCandidates.Contains);
            if (!string.IsNullOrWhiteSpace(candidate))
            {
                firstChoices.Add(candidate);
            }
        }

        if (firstChoices.Count == 0)
        {
            throw new InvalidOperationException("No transferable votes remain.");
        }

        return firstChoices;
    }
}


