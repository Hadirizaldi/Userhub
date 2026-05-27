namespace UserHub.Infrastructure.Persistence.Entities;

public partial class OutboxMessages
{
    public Guid Id { get; set; }

    public string Type { get; set; } = null!;

    public string Payload { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? ProcessedAt { get; set; }

    public int Attempts { get; set; }

    public string? Error { get; set; }
}
