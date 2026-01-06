using fitmeal_workout.DatabaseContext;
using fitmeal_workout.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace fitmeal_workout.Controllers
{
    [ApiController]
    [Route("/api/webhook")]
    public class WebhookController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public WebhookController(ApplicationDbContext context,IConfiguration configuration) { _context = context;

            _configuration = configuration;
        }

        [HttpPost("razorpay")]
        public async Task<IActionResult> RazorPayWebhook()
        {
            string Id = string.Empty;
            try
            {
                Request.EnableBuffering();

                string body;
                using (var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true))
                {
                    body = await reader.ReadToEndAsync();
                    Request.Body.Position = 0;
                }

                var razorpaySignature = Request.Headers["X-Razorpay-Signature"].ToString();
                var secret = _configuration["RazorPay:WebhookSceret"];

                using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(body));
                var generatedSignaure = BitConverter.ToString(hash).Replace("-", "").ToLower();

                if (generatedSignaure != razorpaySignature)
                    return Unauthorized();

                var webhook = JsonConvert.DeserializeObject<RazorpayWebhook>(body);

                if (webhook.Event == "payment.captured")
                {
                    var entity = webhook.Payload.Payment.Entity;

                    Id = entity.Id;

                    var order = await _context.ordersDetails
                        .FirstOrDefaultAsync(o => o.RazorpayOrderId == entity.OrderId);

                    if (order != null)
                    {
                        order.paymentStatus = "CAPTURED";
                        order.RazorpayPaymentId = entity.Id;
                        order.RazorpaySignature = razorpaySignature;
                        await _context.SaveChangesAsync();
                    }
                }

                if (webhook.Event == "payment.failed")
                {
                    var entity = webhook.Payload.Payment.Entity;

                    var order = await _context.ordersDetails
                        .FirstOrDefaultAsync(o => o.RazorpayOrderId == entity.OrderId);

                    if (order != null)
                    {
                        order.paymentStatus = "FAILED";
                        await _context.SaveChangesAsync();
                    }
                }

                return Ok();

            }catch(Exception ex)
            {
                ExceptionDetails exceptionDetails = new ExceptionDetails()
                {
                    ExceptionDetailsId = Id,    
                    ExceptionMessage = ex.Message
                };
                await _context.ExceptionDetails.AddAsync(exceptionDetails);
                await _context.SaveChangesAsync();
                return BadRequest(exceptionDetails);    
            }
           
        }

    }
}
