using System;
using System.Collections.Generic;

namespace Demo.Models;

public class EmailInvite
{
    public Guid EmailInviteId { get; set; }

    public string EmailAddress { get; set; } = null!;

    public DateTime CollectedAt { get; set; }
}
