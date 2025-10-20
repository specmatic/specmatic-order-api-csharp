using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using specmatic_order_api_csharp.models;
using specmatic_order_api_csharp.services;
using System.Diagnostics.CodeAnalysis;
namespace specmatic_order_api_csharp.controllers // Replace with your actual namespace
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    [Route("orders")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrdersController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public ActionResult<IdResponse> Create([FromBody] Order order)
        {
            var orderId = _orderService.CreateOrder(order);
            return Ok(orderId);
        }
        
        [HttpPost("{id}")]
        public ActionResult Update(int id, [FromBody] Order order)
        {
            _orderService.UpdateOrder(order,id);
            return Ok();
        }

        [HttpGet("{id}")]
        public ActionResult<Order> Get(int id)
        {
            try
            {
                return _orderService.GetOrder(id);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Order with ID {id} not found." });
            }
            catch (InvalidOperationException e) 
            {
                return NotFound(new { message =e.Message});
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _orderService.DeleteOrder(id);
            return Ok();
        }

        [HttpGet]
        public ActionResult<List<Order>> Search([FromQuery] string? status, [FromQuery] int? productId)
        {
            var orders = _orderService.FindOrders(status, productId);
            return Ok(orders);
        }
    }
}
