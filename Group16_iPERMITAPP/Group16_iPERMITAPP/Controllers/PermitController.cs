using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Web.Mvc;
using Group16_iPERMITAPP.Database;
using Group16_iPERMITAPP.Models;

namespace Group16_iPERMITAPP.Controllers
{
    public class PermitController : Controller
    {
        private PermitRequestRepository _repo = new PermitRequestRepository();
        private RERepository _reRepo = new RERepository();

        // GET: Permit/Index — show RE's own applications
        public ActionResult Index()
        {
            if (Session["UserType"] == null || Session["UserType"].ToString() != "RE")
                return RedirectToAction("Login", "Account");

            var reId = Session["UserId"].ToString();
            var applications = _repo.GetByREId(reId);
            return View(applications);
        }

        // GET: Permit/Create — show the application form
        public ActionResult Create()
        {
            if (Session["UserType"] == null || Session["UserType"].ToString() != "RE")
                return RedirectToAction("Login", "Account");

            ViewBag.PermitTypes = GetPermitTypes();
            return View();
        }

        // POST: Permit/Create — store form data in session and redirect to payment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PermitRequestViewModel model)
        {
            if (Session["UserType"] == null || Session["UserType"].ToString() != "RE")
                return RedirectToAction("Login", "Account");

            model.REId = Session["UserId"].ToString();
            model.PermitFee = GetPermitFee(model.PermitID);

            // Store application in session temporarily until payment is confirmed
            Session["PendingApplication"] = model;

            // Redirect to payment page
            return RedirectToAction("Payment");
        }

        // GET: Permit/Payment — show mock payment page
        public ActionResult Payment()
        {
            if (Session["UserType"] == null || Session["UserType"].ToString() != "RE")
                return RedirectToAction("Login", "Account");

            var model = Session["PendingApplication"] as PermitRequestViewModel;
            if (model == null)
                return RedirectToAction("Create");

            return View(model);
        }

        // POST: Permit/Payment — process mock payment and save application
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Payment(string cardName, string cardNumber, string cardExpiry, string cardCvv)
        {
            if (Session["UserType"] == null || Session["UserType"].ToString() != "RE")
                return RedirectToAction("Login", "Account");

            var model = Session["PendingApplication"] as PermitRequestViewModel;
            if (model == null)
                return RedirectToAction("Create");

            // Save the permit application
            var success = _repo.SubmitApplication(model);

            if (success)
            {
                // Get the request number that was just created
                var reId = Session["UserId"].ToString();
                var latest = _repo.GetByREId(reId);
                var requestNo = latest.Count > 0 ? latest[0].RequestNo : null;

                // Record payment in Payment table
                if (requestNo != null)
                {
                    RecordPayment(requestNo, model.PermitFee);
                }

                // Clear pending application from session
                Session["PendingApplication"] = null;

                // Send confirmation email
                try
                {
                    var re = _reRepo.GetById(reId);
                    if (re != null && !string.IsNullOrEmpty(re.REEmail))
                    {
                        var emailService = new EmailService();
                        var subject = "iPERMIT - Permit Application & Payment Received";
                        var body = $"Dear {re.ContactPersonName},\n\n" +
                                   $"Your environmental permit application has been successfully submitted and payment received.\n\n" +
                                   $"Activity: {model.ActivityDescription}\n" +
                                   $"Start Date: {model.ActivityStartDate}\n" +
                                   $"Duration: {model.ActivityDuration} days\n" +
                                   $"Amount Paid: ${model.PermitFee:F2}\n" +
                                   $"Status: Pending Review\n\n" +
                                   $"You will be notified once your application has been reviewed.\n\n" +
                                   $"Regards,\niPERMIT System";
                        emailService.SendAndArchive(re.REEmail, subject, body);
                    }
                }
                catch
                {
                    // Don't block on email failure
                }

                TempData["Success"] = "Payment successful! Your permit application has been submitted.";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Something went wrong. Please try again.";
            return RedirectToAction("Create");
        }

        // Helper: record payment in Payment table
        private void RecordPayment(string requestNo, double amount)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                var paymentId = Guid.NewGuid().ToString();
                var sql = @"INSERT INTO Payment (paymentID, requestNo, amountPaid) 
                            VALUES (@paymentID, @requestNo, @amountPaid);";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@paymentID", paymentId);
                    cmd.Parameters.AddWithValue("@requestNo", requestNo);
                    cmd.Parameters.AddWithValue("@amountPaid", amount);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Helper: get all permit types from DB
        private List<SelectListItem> GetPermitTypes()
        {
            var items = new List<SelectListItem>();
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                var sql = "SELECT permitID, permitName, permitFee FROM ENVIRONMENTALPERMIT;";
                using (var cmd = new SQLiteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new SelectListItem
                        {
                            Value = reader.GetInt32(0).ToString(),
                            Text = $"{reader.GetString(1)} (${reader.GetDouble(2):F2})"
                        });
                    }
                }
            }
            return items;
        }

        // Helper: get fee for selected permit
        private double GetPermitFee(int permitId)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                var sql = "SELECT permitFee FROM ENVIRONMENTALPERMIT WHERE permitID = @id;";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", permitId);
                    var result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToDouble(result) : 0;
                }
            }
        }
    }
}