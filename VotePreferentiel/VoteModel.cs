namespace VotePreferentiel;

public class VoteModel
{
    public List<VoteBallot> Votes { get; init; } = [];
}

public class VoteBallot
{
    public List<string> Choices { get; init; } = [];
}