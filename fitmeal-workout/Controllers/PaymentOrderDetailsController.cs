using fitmeal_workout.DatabaseContext;
using fitmeal_workout.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace fitmeal_workout.Controllers
{
    [ApiController]
    [Route("/api/orderDetails")]
    public class PaymentOrderDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentOrderDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("FetchPaymentDetails")]
        public async Task<IActionResult> FetchPaymentDetails()
        {
            try
            {
                var paymentDetails = await _context.ordersDetails
                    .AsNoTracking()
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(payment => new PaymentDetails()
                    {
                        RazorpayOrderId = payment.RazorpayOrderId,
                        Amount = payment.Amount,
                        paymentStatus = payment.paymentStatus,
                        Phone = payment.Phone,
                        Name = payment.Name,
                        Email = payment.Email,
                        PlanCode = payment.PlanCode,
                        CreatedAt = payment.CreatedAt

                    }).ToListAsync();
                return Ok(paymentDetails);
            }
            catch (Exception ex) {
                return StatusCode(500,ex.Message);
            }
        }
    }
}
