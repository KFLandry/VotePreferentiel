namespace VotePreferentiel.Tests;

using VotePreferentiel;
using VotePreferentiel.Worker;

public class VoteCounterTests
{
    private readonly VoteCounter _voteCounter = new();

    [Test]
    public async Task GetWinner_NominalCase_ReturnsChloe()
    {
        var voteModel = BuildVoteModel(
            ["Alice", "Bilal", "Chloe"],
            ["Alice", "Chloe", "Bilal"],
            ["Bilal", "Chloe", "Alice"],
            ["Chloe", "Bilal", "Alice"],
            ["Chloe", "Alice", "Bilal"]);

        var winner = _voteCounter.GetWinner(voteModel);

        await Assert.That(winner).IsEqualTo("Chloe");
    }

    [Test]
    public async Task GetWinner_SingleCandidate_ReturnsAlice()
    {
        var voteModel = BuildVoteModel(["Alice"]);

        var winner = _voteCounter.GetWinner(voteModel);

        await Assert.That(winner).IsEqualTo("Alice");
    }

    [Test]
    public async Task GetWinner_PreferenceTransfers_ReturnsChloe()
    {
        var voteModel = BuildVoteModel(
            ["Alice", "Bilal", "Chloe", "Diane"],
            ["Alice", "Bilal", "Chloe", "Diane"],
            ["Alice", "Bilal", "Chloe", "Diane"],
            ["Alice", "Bilal", "Chloe", "Diane"],
            ["Bilal", "Chloe", "Alice", "Diane"],
            ["Bilal", "Chloe", "Alice", "Diane"],
            ["Bilal", "Chloe", "Alice", "Diane"],
            ["Chloe", "Alice", "Bilal", "Diane"],
            ["Chloe", "Alice", "Bilal", "Diane"],
            ["Chloe", "Alice", "Bilal", "Diane"],
            ["Diane", "Chloe", "Bilal", "Alice"]);

        var winner = _voteCounter.GetWinner(voteModel);

        await Assert.That(winner).IsEqualTo("Chloe");
    }

    [Test]
    public async Task GetWinner_EqualLastPlaceElimination_ReturnsChloe()
    {
        var voteModel = BuildVoteModel(
            ["Alice", "Chloe", "Bilal"],
            ["Bilal", "Chloe", "Alice"],
            ["Chloe", "Alice", "Bilal"],
            ["Chloe", "Bilal", "Alice"]);

        var winner = _voteCounter.GetWinner(voteModel);

        await Assert.That(winner).IsEqualTo("Chloe");
    }

    [Test]
    public async Task GetWinner_EmptyBallot_ThrowsException()
    {
        var voteModel = BuildVoteModel([]);

        Action action = () => _voteCounter.GetWinner(voteModel);

        await Assert.That(action).Throws<InvalidOperationException>();
    }

    private static VoteModel BuildVoteModel(params List<string>[] ballots)
    {
        return new VoteModel
        {
            Votes = ballots.Select(ballot => new VoteBallot { Choices = ballot }).ToList()
        };
    }
}