using System;
using System.Collections.Generic;

namespace UserHub.Infrastructure.Persistence.Entities;

public partial class ProcessedMessages
{
    public Guid Id { get; set; }

    public DateTime ProcessedAt { get; set; }
}
