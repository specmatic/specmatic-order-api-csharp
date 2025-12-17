using specmatic_order_api_csharp.exceptions;
using specmatic_order_api_csharp.models;

namespace specmatic_order_api_csharp.services
{
    public class OrderService
    {
        public IdResponse CreateOrder(Order order)
        {
            DB.ReserveProductInventory(order.Productid, order.Count);
            DB.AddOrder(order);
            return new IdResponse(order.Id);
        }
        
        public void  UpdateOrder(Order order,int id)
        {
            if (order.Id == 0)
            {
                throw new ValidationException("Product id cannot be null");
            }

            DB.UpdateOrder(order);
        }
        public Order GetOrder(int id)
        {
            return DB.GetOrder(id);
        }

        public void DeleteOrder(int id)
        {
            DB.DeleteOrder(id);
        }

        public List<Order> FindOrders(OrderStatus? status, int? productId)
        {
            return DB.FindOrders(status, productId);
        }
    }
}