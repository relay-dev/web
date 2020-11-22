using Core.Plugins.Azure.EventGrid.Extensions;
using Core.Utilities;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Web.AzureFunctions.Framework;
using Web.Samples.OrderManagement.Domain.Events.Payload;

namespace Web.Samples.OrderManagement.EventHandlers.Handlers
{
    public class OnOrderCreated : EventHandlerBase
    {
        private readonly IJsonSerializer _jsonSerializer;

        public OnOrderCreated(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }

        [FunctionName("OnOrderCreated")]
        public async Task Run([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log, CancellationToken cancellationToken)
        {
            var payload = eventGridEvent.GetPayload<OrderCreatedPayload>();

            // Handle the event

            string payloadJson = await _jsonSerializer.SerializeAsync(payload, cancellationToken);

            log.LogInformation(payloadJson);
        }
    }
}
