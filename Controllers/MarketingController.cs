using System.Net;
using System.Net.Mail;
using Demo.Configurations;
using Demo.Context;
using Demo.Models;
using Demo.Services;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Demo.Controllers
{
    public class MarketingController : Controller
    {
        private readonly MyDBContext _context;
        private readonly SmtpSettings _smtpSettings;
        private readonly EmailService _emailService;

        public MarketingController(MyDBContext context, IOptions<SmtpSettings> smtpSettings, EmailService emailService)
        {
            _context = context;
            _smtpSettings = smtpSettings.Value;
            _emailService = emailService;

        }
        public IActionResult Subscribe()
        {
            return View();
        }
        // POST: /MarketingInvite/Subscribe
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Subscribe(EmailInvite model)
        {
            if (ModelState.IsValid)
            {
                // Avoid duplicates
                var exists = _context.EmailInvites
                    .Any(x => x.EmailAddress == model.EmailAddress);

                if (!exists)
                {
                    _context.EmailInvites.Add(model);
                    _context.SaveChanges();

                    ViewBag.Message = "Thanks! You’ll receive our invites and offers.";
                }
                else
                {
                    ViewBag.Message = "This email is already subscribed.";
                }
            }

            return View(model);
        }

        // GET: Email/Send
        public ActionResult Send()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Send(string subject, string message)
        {

            if (!_context.EmailInvites.Any())
            {
                ViewBag.Message = "No subscribers found!";
                return View();
            }

            BackgroundJob.Enqueue(() =>
                     _emailService.SendEmailAsync(subject, message)
                 );

            ViewBag.Message = "Emails are being sent in the background!";
            return View();
        }

        // Show sent emails
        public ActionResult SentEmails()
        {
            var emails = _context.EmailSents.OrderByDescending(e => e.SentAt).ToList();
            return View(emails);
        }
    }
}
