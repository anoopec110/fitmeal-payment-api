using System.ComponentModel.DataAnnotations;

namespace fitmeal_workout.Models
{
    public class OrderModel
    {
            [Key]
            public int Id { get; set; }

            public string RazorpayOrderId { get; set; }
            public string RazorpayPaymentId { get; set; }
            public string RazorpaySignature { get; set; }

            public string PlanCode { get; set; }
            public int Amount { get; set; }

            public string Name { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }

            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


}
}
