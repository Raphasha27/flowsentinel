using FlowSentinel.Core.AbuseDetection;
using Xunit;

namespace FlowSentinel.UnitTests;

public class TrajectoryAnalysisTests
{
    [Fact]
    public void ValidUserFlow_ShouldNotBeFlagged()
    {
        // Arrange
        var engine = new PatternRecognitionEngine();
        string clientId = "legit-user";

        // Act: Follows logic /home -> /products -> /cart
        engine.AnalyzeTrajectory(clientId, "/home");
        bool isAbuse = engine.AnalyzeTrajectory(clientId, "/products");

        // Assert
        Assert.False(isAbuse);
    }

    [Fact]
    public void BotDiscoveryJump_ShouldBeFlagged()
    {
        // Arrange
        var engine = new PatternRecognitionEngine();
        string clientId = "scanner-bot";

        // Act: Jumps from /home (0,0) directly to /admin (5,5)
        engine.AnalyzeTrajectory(clientId, "/home");
        bool isAbuse = engine.AnalyzeTrajectory(clientId, "/admin");

        // Assert
        Assert.True(isAbuse); // Flagged because distance > 3 hops
    }
}
