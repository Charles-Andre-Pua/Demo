using System;
using System.Collections.Generic;

namespace Demo.Models;

public partial class EmailSent
{
    public Guid EmailSent1 { get; set; }

    public string? Subject { get; set; }

    public string? Message { get; set; }
}
