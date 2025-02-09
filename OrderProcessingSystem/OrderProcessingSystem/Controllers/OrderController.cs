using Microsoft.AspNetCore.Mvc;
using ORS.Data.Models;
using ORS.Service.Contracts;
using ORS.Service.Dtos;

namespace ORS.API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderAsync(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }

            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrderAsync([FromBody] CustomerOrdersDto order)
        {
            try
            {
                await _orderService.CreateOrderAsync(order);
                return Ok("Order Created successfully...");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/fulfill")]
        public async Task<IActionResult> FulfillOrderAsync(int id)
        {
            try
            {
                await _orderService.FulfillOrderAsync(id);
                return Ok($"Order with ID {id} has been fulfilled.");
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
