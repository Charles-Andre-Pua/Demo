using System.Net;
using System.Net.Mail;
using Demo.Configurations;
using Demo.Context;
using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using static System.Formats.Asn1.AsnWriter;

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
        // GET: Email/Send
        public ActionResult Send()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Send(string subject, string message)
        {
            var recipients = await _context.EmailInvites
               .Select(e => e.EmailAddress)
               .ToListAsync();

            if (!recipients.Any())
            {
                ViewBag.Message = "No subscribers found!";
                return View();
            }

            _ = Task.Run(async () =>
            {
                using var scope = HttpContext.RequestServices.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<MyDBContext>();
                var smtpSettings = scope.ServiceProvider.GetRequiredService<IOptions<SmtpSettings>>().Value;

                
                using (var smtp = new SmtpClient(smtpSettings.Host, smtpSettings.Port))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password);
                    smtp.EnableSsl = true;

                    foreach (var email in recipients)
                    {
                        var mail = new MailMessage
                        {
                            From = new MailAddress(smtpSettings.Username, "TheSocialName"),
                            Subject = subject,
                            Body = message,
                            IsBodyHtml = false
                        };
                        mail.To.Add(email);
                        await smtp.SendMailAsync(mail);
                    }

                    // Save sent email to DB
                    var sentEmail = new EmailSent
                    {
                        Subject = subject,
                        Message = message,
                        SentAt = DateTime.Now
                    };
                    context.EmailSents.Add(sentEmail);
                    await context.SaveChangesAsync();
                }
            });
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
