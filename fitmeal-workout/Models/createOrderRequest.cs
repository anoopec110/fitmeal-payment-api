using System.ComponentModel.DataAnnotations;

namespace fitmeal_workout.Models
{
    public class createOrderRequest
    {
        [Key]
        public int Id { get; set; }
        public decimal amount { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phoneNumber { get; set; }
        public string planCode { get; set; }
    }
}
