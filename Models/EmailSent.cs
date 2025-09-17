using System;
using System.Collections.Generic;

namespace Demo.Models;

public class EmailSent
{
    public Guid EmailSentId { get; set; }

    public string Subject { get; set; } = null!;

    public string Message { get; set; } = null!;

    public DateTime SentAt { get; set; }
}
