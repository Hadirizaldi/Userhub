namespace UserHub.Application.Email;

public sealed record EmailMessage (
    string To,
    string Subject,
    string Body
);