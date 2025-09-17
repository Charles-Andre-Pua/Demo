using System;
using System.Collections.Generic;

namespace Demo.Models;

public partial class Employee
{
    public string EmployeeId { get; set; } = null!;

    public string EmployeeName { get; set; } = null!;

    public DateTime BirthDate { get; set; }

    public DateTime HiredDate { get; set; }

    public bool Status { get; set; }

    public string Gender { get; set; } = null!;

    public virtual Account Accounts { get; set; } = null!;
}
