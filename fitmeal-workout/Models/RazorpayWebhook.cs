using Newtonsoft.Json;

namespace fitmeal_workout.Models
{
    public class RazorpayWebhook
    {
        [JsonProperty("event")]
        public string Event { get; set; }

        [JsonProperty("payload")]
        public Payload Payload { get; set; }
    }

    public class Payload
    {
        [JsonProperty("payment")]
        public Payment Payment { get; set; }
    }

    public class Payment
    {
        [JsonProperty("entity")]
        public PaymentEntity Entity { get; set; }
    }

    public class PaymentEntity
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("order_id")]
        public string OrderId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

}
