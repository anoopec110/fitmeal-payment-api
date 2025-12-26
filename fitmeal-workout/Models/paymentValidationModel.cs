using System.ComponentModel.DataAnnotations;

namespace fitmeal_workout.Models
{
    public class paymentValidationModel
    {
        [Required]
        public string razorpay_order_id { get; set; }
        [Required]
        public string razorpay_payment_id { get; set; }
        [Required]
        public string razorpay_signature { get; set; }

    }
}
