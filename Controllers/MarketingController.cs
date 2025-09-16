using Demo.Configurations;
using Demo.Context;
using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Demo.Controllers
{
    public class MarketingController : Controller
    {
        private readonly MyDBContext _context;
        private readonly SmtpSettings _smtpSettings;

        public MarketingController(MyDBContext context, IOptions<SmtpSettings> smtpSettings)
        {
            _context = context;
            _smtpSettings = smtpSettings.Value;

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

        public IActionResult SendEmail()
        {
            return View();
        }// POST: Marketing/SendEmail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendEmail(EmailSent model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var recipients = await _context.EmailInvites
                .Select(e => e.EmailAddress)
                .ToListAsync();

            if (!recipients.Any())
            {
                ViewBag.Message = "No subscribers found.";
                return View(model);
            }

            try
            {
                using (var smtp = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port))
                {
                    smtp.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
                    smtp.EnableSsl = true;

                    foreach (var email in recipients)
                    {
                        var mail = new MailMessage();
                        mail.From = new MailAddress(_smtpSettings.Username, "TheSocialNi&&ers");
                        mail.To.Add(email);
                        mail.Subject = model.Subject;
                        mail.Body = model.Message;
                        mail.IsBodyHtml = false;

                        await smtp.SendMailAsync(mail);
                    }
                }

                ViewBag.Message = "Emails sent successfully!";
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error: {ex.Message}";
            }

            return View(model);
        }
    }
}
