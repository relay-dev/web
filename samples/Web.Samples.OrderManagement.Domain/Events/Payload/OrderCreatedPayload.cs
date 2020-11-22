using System;

namespace Web.Samples.OrderManagement.Domain.Events.Payload
{
    public class OrderCreatedPayload
    {
        public long OrderId { get; set; }

        public DateTime OrderDate { get; set; }
    }
}
