using Demo.Models;

namespace Demo.ViewModels
{
    public class EmployeeViewModel
    {
        // Employee Info
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public DateTime HiredDate { get; set; } = DateTime.Now;
        public string Gender { get; set; } = string.Empty;
        public bool Status { get; set; } = true;

        // Account Info
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;

        // Role Assignment
        public string RoleId { get; set; } = string.Empty;

        // For dropdown list of roles in the view
        public IEnumerable<Role>? AvailableRoles { get; set; }
    }
}
