using System;
using System.Collections.Generic;

namespace Demo.Models;

public partial class Account
{
    public string AccountId { get; set; } = null!;

    public string EmployeeId { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Salt { get; set; } = null!;

    public string RoleId { get; set; } = null!;

    public virtual Employee Employee { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;

}
