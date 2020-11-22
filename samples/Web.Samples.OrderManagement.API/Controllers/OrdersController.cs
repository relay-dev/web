using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.Swagger.Annotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Web.Samples.OrderManagement.Domain.Commands.Create;
using Web.Samples.OrderManagement.Domain.Commands.Get;
using Web.Samples.OrderManagement.Domain.DTO;

namespace Web.Samples.OrderManagement.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Gets the order that match on the provided orderId
        /// </summary>
        /// <param name="orderId">The AccountGlobalId to search with</param>
        /// <param name="cancellationToken">The current cancellation token</param>
        /// <returns>The collection of account that match on AccountGlobalId</returns>
        [HttpGet]
        [Route("{orderId}")]
        [Produces(typeof(Order))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Order))]
        public async Task<ActionResult<Order>> Get(long orderId, CancellationToken cancellationToken)
        {
            if (orderId < 1)
            {
                return BadRequest();
            }

            var request = new GetOrderByIdRequest
            {
                OrderId = orderId
            };

            GetOrderByIdResponse response = await _mediator.Send(request, cancellationToken);

            return new OkObjectResult(response.Order);
        }

        /// <summary>
        /// Creates a new order record
        /// </summary>
        /// <param name="order">The order to save</param>
        /// <param name="cancellationToken">The current cancellation token</param>
        /// <returns>The orderId that was created</returns>
        [HttpPost]
        [Produces(typeof(long))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(long))]
        public async Task<ActionResult<long>> Post(Order order, CancellationToken cancellationToken)
        {
            var request = new CreateOrderRequest
            {
                Order = order
            };

            CreateOrderResponse response = await _mediator.Send(request, cancellationToken);

            return CreatedAtAction("Post", response.OrderId);
        }
    }
}
