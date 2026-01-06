using System.ComponentModel.DataAnnotations;

namespace fitmeal_workout.Models
{
    public class ExceptionDetails
    {
        [Key]
        public string ExceptionDetailsId {  get; set; }
        public string ExceptionMessage {  get; set; }
    }
}
