using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using UserHub.Application.Abstractions.Messaging;
using UserHub.Application.Messaging;
using UserHub.Application.Messaging.Jobs;

namespace UserHub.UnitTests.Messaging;

public class OutboxRelayJobTests
{
    private static OutboxMessage Pending(Guid id) =>
        new(id, "user.registered", """{"userId":1,"email":"x@test.local","fullname":"X"}""");

    [Fact]
    public async Task RunAsync_PublishSucceeds_MarksProcessedAndNotFailed()
    {
        // Arrange
        var id = Guid.NewGuid();
        var reader = new Mock<IOutboxReader>();
        reader.Setup(r => r.GetPendingAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { Pending(id) });
        var publisher = new Mock<IEventPublisher>();
        var job = new OutboxRelayJob(reader.Object, publisher.Object, NullLogger<OutboxRelayJob>.Instance);

        // Act
        await job.RunAsync(CancellationToken.None);

        // Assert
        publisher.Verify(p => p.PublishAsync("user.registered", It.IsAny<string>(), id, It.IsAny<CancellationToken>()), Times.Once);
        reader.Verify(r => r.MarkProcessedAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        reader.Verify(r => r.MarkFailedAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RunAsync_PublishThrows_MarksFailedAndNotProcessed()
    {
        // Arrange
        var id = Guid.NewGuid();
        var reader = new Mock<IOutboxReader>();
        reader.Setup(r => r.GetPendingAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { Pending(id) });
        var publisher = new Mock<IEventPublisher>();
        publisher.Setup(p => p.PublishAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("broker down"));
        var job = new OutboxRelayJob(reader.Object, publisher.Object, NullLogger<OutboxRelayJob>.Instance);

        // Act
        await job.RunAsync(CancellationToken.None);

        // Assert
        reader.Verify(r => r.MarkFailedAsync(id, It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        reader.Verify(r => r.MarkProcessedAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
