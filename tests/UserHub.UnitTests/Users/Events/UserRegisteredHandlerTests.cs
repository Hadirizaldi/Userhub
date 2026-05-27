using FluentAssertions;
using Moq;
using UserHub.Application.Users.Events;
using UserHub.Application.Email;
using Microsoft.Extensions.Logging.Abstractions;
using UserHub.Application.Abstractions.Email;

namespace UserHub.UnitTests.Users.Events;

public class UserRegisteredHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldSendWelcomeEmail_ToRegisteredUser()
    {
        // Arrange
        var emailSender = new Mock<IEmailSender>();
        var handler = new UserRegisteredHandler(
            emailSender:emailSender.Object,
            logger: NullLogger<UserRegisteredHandler>.Instance
        );

        var evt = new UserRegisteredEvent(42, "coba@test.local", "coba testing nih");

        // act
        await handler.HandleAsync(
            evt,
            CancellationToken.None
        );

        // assert
        emailSender.Verify(s => s.SendAsync(
            It.Is<EmailMessage>(m => m.To == evt.Email),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }
}

