using fitmeal_workout.Models;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using Razorpay.Api;

namespace fitmeal_workout.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public PaymentController(IConfiguration configuration)
        {
            _configuration = configuration;
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
        public IActionResult paymentVerification([FromBody] paymentValidationModel request)
        {
            try
            {


                string secret = _configuration["RazorPay:RazorPaySceret"];

                Dictionary<string, string> attributes = new Dictionary<string, string>();
                attributes.Add("razorpay_order_id", request.razorpay_order_id);
                attributes.Add("razorpay_payment_id", request.razorpay_payment_id);
                attributes.Add("razorpay_signature", request.razorpay_signature);

                Utils.verifyPaymentSignature(attributes);


                return Ok(new { status = "success" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }

        }
    }
}
