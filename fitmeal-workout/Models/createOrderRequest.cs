using System.ComponentModel.DataAnnotations;

namespace fitmeal_workout.Models
{
    public class createOrderRequest
    {
        [Required]
        public decimal amount { get; set; }
    }
}
