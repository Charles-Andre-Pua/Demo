using System.Security.Cryptography;
using System.Text;
using Demo.Context;
using Demo.Helpers;
using Demo.Models;
using Demo.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demo.Controllers
{
    public class EmployeeController : Controller
    {
        private static readonly Random _rng = new Random();
        private readonly MyDBContext _context;
        public EmployeeController(MyDBContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Manager,Front Staff")]
        public IActionResult Index()
        {
            var employees = _context.Employees
                .Include(e => e.Accounts)
                .ThenInclude(a => a.Role)
                .ToList();

            ViewBag.Roles = _context.Roles.ToList();
            return View(employees);
        }
        [HttpGet]
        [Authorize(Roles = "Manager")]
        public IActionResult Create()
        {
            ViewBag.Roles = _context.Roles.ToList();
            return View();
        }
        [HttpPost]
        public IActionResult Create(EmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = _context.Roles.ToList();
                return View(model);
            }

            // 1. Create Employee
            var employee = new Employee
            {
                EmployeeId = Guid.NewGuid().ToString(),
                EmployeeName = model.EmployeeName,
                BirthDate = model.BirthDate,
                HiredDate = model.HiredDate,
                Status = true,
                Gender = model.Gender
            };

            _context.Employees.Add(employee);

            // 2. Create Account
            var salt = PasswordHelper.GenerateSalt();
            var hash = PasswordHelper.HashPassword(model.Password, salt);

            var account = new Account
            {
                AccountId = Guid.NewGuid().ToString(),
                EmployeeId = employee.EmployeeId,
                Username = model.Username,
                PasswordHash = hash,
                Salt = salt,
                RoleId = model.RoleId
            };

            _context.Accounts.Add(account);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        // GET: Edit
        [HttpGet]
        [Authorize(Roles = "Manager")]
        public IActionResult Edit(string id)
        {
            var employee = _context.Employees
                .Include(e => e.Accounts)
                .FirstOrDefault(e => e.EmployeeId == id);

            if (employee == null) return NotFound();

            var vm = new EmployeeViewModel
            {
                EmployeeId = employee.EmployeeId,
                EmployeeName = employee.EmployeeName,
                BirthDate = employee.BirthDate,
                HiredDate = employee.HiredDate,
                Status = employee.Status,
                Gender = employee.Gender,
                Username = employee.Accounts?.Username,
                RoleId = employee.Accounts?.RoleId
            };

            ViewBag.Roles = _context.Roles.ToList();
            return View(vm);
        }

        // POST: Edit
        [HttpPost]
        public IActionResult Edit(EmployeeViewModel model)
        {
            var employee = _context.Employees
                .Include(e => e.Accounts)
                .FirstOrDefault(e => e.EmployeeId == model.EmployeeId);

            if (employee == null) return NotFound();

            // Update Employee
            employee.EmployeeName = model.EmployeeName;
            employee.BirthDate = model.BirthDate;
            employee.HiredDate = model.HiredDate;
            employee.Status = model.Status;
            employee.Gender = model.Gender;

            // Update Account
            if (employee.Accounts != null)
            {
                employee.Accounts.Username = model.Username;
                employee.Accounts.RoleId = model.RoleId;

                // Only update password if user typed one
                if (!string.IsNullOrEmpty(model.Password))
                {
                    var salt = PasswordHelper.GenerateSalt();
                    var hash = PasswordHelper.HashPassword(model.Password, salt);
                    employee.Accounts.PasswordHash = hash;
                    employee.Accounts.Salt = salt;
                }
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Authorize(Roles = "Manager")]
        public IActionResult Delete(string EmployeeId)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.EmployeeId == EmployeeId);
            if (employee != null)
            {
                employee.Status = false; // soft delete
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
