using Core.Plugins.Azure.EventGrid.Extensions;
using MediatR;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Web.AzureFunctions.Framework;
using Web.Samples.OrderManagement.Domain.Commands.Get;
using Web.Samples.OrderManagement.Domain.Events.Payload;

namespace Web.Samples.OrderManagement.EventHandlers.Handlers
{
    public class OnOrderCreated : EventHandlerBase
    {
        private readonly IMediator _mediator;

        public OnOrderCreated(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName("OnOrderCreated")]
        public async Task Run([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log, CancellationToken cancellationToken)
        {
            var payload = eventGridEvent.GetPayload<OrderCreatedPayload>();

            var request = new GetOrderByIdRequest
            {
                OrderId = payload.OrderId
            };

            GetOrderByIdResponse response = await _mediator.Send(request, cancellationToken);

            // Handle the event

            log.LogInformation($"Handled OnOrderCreated for OrderId {response.Order.OrderId}");
        }
    }
}
