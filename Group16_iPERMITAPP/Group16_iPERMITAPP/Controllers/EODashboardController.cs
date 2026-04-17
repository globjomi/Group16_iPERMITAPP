using System.Web.Mvc;
using Group16_iPERMITAPP.Models;
using System.Net.Mail;
using System.Net;
using System.Configuration;

namespace Group16_iPERMITAPP.Controllers
{
    public class EODashboardController : Controller
    {
        public ActionResult Index()
        {
            if (Session["UserType"] == null || Session["UserType"].ToString() != "EO")
            {
                return RedirectToAction("Login", "Account");
            }

            var prRepo = new PermitRequestRepository();
            var model = prRepo.GetAllWithRE();

            var reRepo = new RERepository();
            var reList = reRepo.GetAll();
            ViewBag.REs = reList;

            return View(model);
        }

        // GET: EODashboard/Details/{id}
        public ActionResult Details(string id)
        {
            if (Session["UserType"] == null || Session["UserType"].ToString() != "EO")
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrEmpty(id))
                return HttpNotFound();

            var repo = new PermitRequestRepository();
            var model = repo.GetByRequestNo(id);
            if (model == null)
                return HttpNotFound();

            return View(model);
        }

        // POST: EODashboard/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(string requestNo, string status)
        {
            if (Session["UserType"] == null || Session["UserType"].ToString() != "EO")
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrEmpty(requestNo) || string.IsNullOrEmpty(status))
            {
                TempData["Error"] = "Invalid request.";
                return RedirectToAction("Details", new { id = requestNo });
            }

            var repo = new PermitRequestRepository();
            var ok = repo.AddRequestStatus(requestNo, status);

            if (ok)
            {
                // Retrieve updated request to get RE email
                var updated = repo.GetByRequestNo(requestNo);
                var emailService = new EmailService();

                var recipient = updated?.REEmail;
                if (!string.IsNullOrEmpty(recipient))
                {
                    var subject = $"Permit request {requestNo} status updated: {status}";
                    var body = $"Hello {updated.ContactPersonName ?? updated.REId},\n\nYour permit request {requestNo} has been updated to status: {status}.\n\nDescription: {updated.ActivityDescription}\n\nRegards,\nEnvironmental Officer";
                    try
                    {
                        emailService.SendAndArchive(recipient, subject, body);
                    }
                    catch
                    {
                        // Log or set TempData if desired; don't block update on email failure
                        TempData["EmailError"] = "Failed to send notification email to RE.";
                    }
                }

                TempData["Success"] = "Status updated.";
            }
            else
            {
                TempData["Error"] = "Failed to update status.";
            }

            return RedirectToAction("Details", new { id = requestNo });
        }

        // Optional: quick GET for email compose screen
        public ActionResult ComposeEmail()
        {
            if (Session["UserType"] == null || Session["UserType"].ToString() != "EO")
            {
                return RedirectToAction("Login", "Account");
            }

            var reRepo = new RERepository();
            ViewBag.REs = reRepo.GetAll();
            return View();
        }

        public ActionResult Email()
        {
            var reRepo = new RERepository();
            ViewBag.REs = reRepo.GetAll();
            return View("ComposeEmail"); // explicitly renders ComposeEmail.cshtml
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendEmail(string recipientEmail, string subject, string body)
        {
            if (Session["UserType"] == null || Session["UserType"].ToString() != "EO")
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrEmpty(recipientEmail) || string.IsNullOrEmpty(subject))
            {
                TempData["Error"] = "Recipient and subject required.";
                return RedirectToAction("ComposeEmail");
            }

            var emailService = new EmailService();
            try
            {
                emailService.SendAndArchive(recipientEmail, subject, body);
                TempData["Success"] = "Email sent.";
            }
            catch
            {
                TempData["Error"] = "Failed to send email. Check SMTP settings.";
            }

            return RedirectToAction("ComposeEmail");
        }
    }

    public class EmailService
    {
        public void SendAndArchive(string recipient, string subject, string body)
        {
            var smtpHost = ConfigurationManager.AppSettings["SmtpHost"];
            var smtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
            var smtpUser = ConfigurationManager.AppSettings["SmtpUser"];
            var smtpPass = ConfigurationManager.AppSettings["SmtpPass"];
            var smtpFrom = ConfigurationManager.AppSettings["SmtpFrom"];
            var smtpEnableSsl = bool.Parse(ConfigurationManager.AppSettings["SmtpEnableSsl"]);

            using (var message = new MailMessage())
            using (var smtp = new SmtpClient(smtpHost, smtpPort))
            {
                message.From = new MailAddress(smtpFrom);
                message.To.Add(new MailAddress(recipient));
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = false;

                smtp.Credentials = new NetworkCredential(smtpUser, smtpPass);
                smtp.EnableSsl = smtpEnableSsl;

                smtp.Send(message);
            }

            // Archive logic if needed
            // ...
        }
    }
}