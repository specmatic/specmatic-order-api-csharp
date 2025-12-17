using specmatic_order_api_csharp.exceptions;
using specmatic_order_api_csharp.models;
using specmatic_order_api_csharp.services;
using Xunit;
using System.Collections.Generic;

namespace specmatic_order_api_csharp.tests
{
    public class OrderServiceTests
    {
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _orderService = new OrderService();
        }

        [Fact]
        public void CreateOrder_ShouldReturnIdResponse_WhenOrderIsValid()
        {
            // Arrange
            var order = new Order { Id = 1, Productid = 10, Count = 2 }; // Sample valid order

            // Reserve product inventory in DB (actual DB call)
            DB.ReserveProductInventory(order.Productid, order.Count);
            DB.AddOrder(order); // Add order to DB

            // Act
            var result = _orderService.CreateOrder(order);

            // Assert
            Assert.IsType<IdResponse>(result); // Ensure return type is IdResponse
            Assert.Equal(order.Id, result.Id); // Check if the order id is returned correctly
        }

        [Fact]
        public void UpdateOrder_ShouldThrowValidationException_WhenOrderIdIsZero()
        {
            // Arrange
            var order = new Order { Id = 0 }; // Invalid order (id cannot be zero)

            // Act & Assert
            Assert.Throws<ValidationException>(() => _orderService.UpdateOrder(order, order.Id));
          
        }

        [Fact]
        public void GetOrder_ShouldReturnOrder_WhenOrderExists()
        {
            // Arrange
            int orderId = 1;
            var expectedOrder = new Order { Id = orderId, Productid = 100, Count = 2 };

            // Add order to DB (actual DB call)
            DB.AddOrder(expectedOrder);

            // Act
            var result = _orderService.GetOrder(orderId);

            // Assert
            Assert.Equal(expectedOrder, result); // Check if the correct order is returned
        }

        [Fact]
        public void DeleteOrder_ShouldThrowException_WhenOrderIsDeleted()
        {
            // Arrange
            int orderId = 10;

            // Add order to DB (actual DB call)
            var order = new Order { Id = orderId, Productid = 100, Count = 2 };
            DB.AddOrder(order);

            // Act
            _orderService.DeleteOrder(orderId);

            // Assert
            // Check if the order was deleted in DB by trying to retrieve it
            var exception = Assert.Throws<KeyNotFoundException>(() => DB.GetOrder(orderId));
            Assert.Equal($"Order with ID {orderId} not found.", exception.Message); // Assuming OrderNotFoundException is thrown
        }


        [Fact]
        public void FindOrders_ShouldReturnListOfOrders_WhenStatusAndProductIdAreValid()
        {
            // Arrange
            OrderStatus status = OrderStatus.pending;
            int productId = 100;

            var expectedOrders = new List<Order>
            {
                new Order { Id = 1, Productid = productId, Status = status },
                new Order { Id = 2, Productid = productId, Status = status }
            };

            // Add orders to DB (actual DB call)
            DB.AddOrder(expectedOrders[0]);
            DB.AddOrder(expectedOrders[1]);

            // Act
            var result = _orderService.FindOrders(status, productId);

            // Assert
            Assert.Equal(expectedOrders.Count, result.Count); // Ensure the number of orders returned is correct
            Assert.Contains(result, order => order.Id == 1); // Ensure the orders match expected ones
            Assert.Contains(result, order => order.Id == 2);
        }
    }
}
