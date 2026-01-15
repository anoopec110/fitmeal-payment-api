namespace fitmeal_workout.Models
{
    public class PaymentDetails
    {
        public string RazorpayOrderId { get; set; }

        public string PlanCode { get; set; }
        public decimal Amount { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string paymentStatus { get; set; }



        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
