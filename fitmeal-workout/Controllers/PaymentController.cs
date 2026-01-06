using fitmeal_workout.DatabaseContext;
using fitmeal_workout.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> createOrder([FromBody] createOrderRequest request)
        {
            try
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

                OrderModel model = new OrderModel()
                {
                    RazorpayOrderId = order["id"].ToString(),
                    Amount = request.amount,
                    paymentStatus = "PENDING",
                    Name = request.name,
                    Phone = request.phoneNumber,
                    Email = request.email,
                    PlanCode = request.planCode

                };

                    await _context.ordersDetails.AddAsync(model);
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    orderId = order["id"].ToString(),
                    amount = request.amount,
                    key = key
                });

            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
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

                var existingOrder = await _context.ordersDetails.FirstOrDefaultAsync(data => 
                data.RazorpayOrderId == request.RazorpayOrderId);

                if(existingOrder != null)
                {
                    existingOrder.paymentStatus = "PENDING";
                    existingOrder.RazorpayPaymentId = request.RazorpayPaymentId;
                    existingOrder.RazorpaySignature = request.RazorpaySignature;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return NotFound(new
                    {
                        message = "Order not found during payment verification"
                    });
                }


                    return Ok(new { status = "Verified" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }

        }

        [HttpGet("order-status")]
        public async Task<IActionResult> GetOrderStatus(string orderId)
        {
            var orderDetails = await _context.ordersDetails.FirstOrDefaultAsync(data => data.RazorpayOrderId == orderId);

            if(orderDetails == null)
            {
                return NotFound("orderId not found");
            }
            else
            {
                return Ok(new { status = orderDetails.paymentStatus });
            }
        }
    }
}
