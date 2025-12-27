using fitmeal_workout.DatabaseContext;
using fitmeal_workout.Models;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using Razorpay.Api;
using System.Threading.Tasks;

namespace fitmeal_workout.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        private readonly ApplicationDbContext _context;

        public PaymentController(IConfiguration configuration, ApplicationDbContext cntext)
        {
            _configuration = configuration;
            _context = cntext;          
        }
        [HttpPost("create-order")]
        public IActionResult createOrder([FromBody] createOrderRequest request)
        {
            var key = _configuration["RazorPay:RazorPayKey"];
            var sceret = _configuration["RazorPay:RazorPaySceret"];

            RazorpayClient razorpayClient = new RazorpayClient(key, sceret);

            Dictionary<string, object> data = new Dictionary<string, object>()
            {

            { "amount", request.amount * 100 }, // paise
            { "currency", "INR" },
            { "receipt", Guid.NewGuid().ToString() },
            { "payment_capture", 1 }

            };

            Order order = razorpayClient.Order.Create(data);
            return Ok(new
            {
                orderId = order["id"].ToString(),
                amount = request.amount,
                key = key
            });
        }
        [HttpPost("payment-verification")]
        public async Task<IActionResult> paymentVerification([FromBody] OrderModel request)
        {
            try
            {


                string secret = _configuration["RazorPay:RazorPaySceret"];

                Dictionary<string, string> attributes = new Dictionary<string, string>();
                attributes.Add("razorpay_order_id", request.RazorpayOrderId);
                attributes.Add("razorpay_payment_id", request.RazorpayPaymentId);
                attributes.Add("razorpay_signature", request.RazorpaySignature);

                Utils.verifyPaymentSignature(attributes);

                await _context.ordersDetails.AddAsync(request);
                await _context.SaveChangesAsync();


                return Ok(new { status = "success" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }

        }
    }
}
