using System;
using System.Collections.Generic;

namespace Demo.Models;

public partial class EmailInvite
{
    public Guid EmailInviteId { get; set; }

    public string EmailAddress { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
