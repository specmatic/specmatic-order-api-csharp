using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using specmatic_order_api_csharp.exceptions;

namespace specmatic_order_api_csharp.models;

[ExcludeFromCodeCoverage]
public static class DB
{
    private static Dictionary<int, Product> PRODUCTS = new()
    {
        { 10, new Product { Id = 10, Name = "XYZ Phone", Type = ProductType.gadget, Inventory = 10 } },
        { 20, new Product { Id = 20, Name = "Gemini", Type = ProductType.gadget, Inventory = 10 } }
    };

    private static Dictionary<int, string> PRODUCT_IMAGE = new()
    {
        { 10, "https://example.com/image.jpg" },
        { 20, "https://example.com/image.jpg" }
    };

    private static Dictionary<int, Order> ORDERS = new()
    {
        { 10, new Order { Id = 10, Productid = 10, Count = 2, Status = OrderStatus.pending } },
        { 20, new Order { Id = 20, Productid = 10, Count = 1, Status = OrderStatus.pending } }
    };

    private static readonly Dictionary<string, User> USERS = new()
    {
        { "API-TOKEN-SPEC", new User (name: "Hari") }
    };

    public static int UserCount() => USERS.Count;

    public static void ResetDB()
    {
        PRODUCTS = new Dictionary<int, Product>
        {
            { 10, new Product { Id = 10, Name = "XYZ Phone", Type = ProductType.gadget, Inventory = 10 } },
            { 20, new Product { Id = 20, Name = "Gemini", Type = ProductType.gadget, Inventory = 10 } }
        };

        ORDERS = new Dictionary<int, Order>
        {
            { 10, new Order { Id = 10, Productid = 10, Count = 2, Status =  OrderStatus.pending } },
            { 20, new Order { Id = 20, Productid = 10, Count = 1, Status =  OrderStatus.pending } }
        };
    }

    public static void AddProduct(Product product)
    {
        PRODUCTS[product.Id] = product;
    }

    public static Product FindProduct(int id) => PRODUCTS[id];

    public static void UpdateProduct(Product updatedProduct)
    {
        if (PRODUCTS.ContainsKey(updatedProduct.Id))
        {
            PRODUCTS[updatedProduct.Id] = new Product
            {
                Id = updatedProduct.Id,
                Name = updatedProduct.Name,
                Type = updatedProduct.Type,
                Inventory = updatedProduct.Inventory
            };
        }
    }

    public static void DeleteProduct(int id)
    {
        PRODUCTS.Remove(id);
    }

    public static List<Product> FindProducts(string name = null, ProductType? type = null, string status = null)
    {
        return PRODUCTS.Values
            .Where(product =>
                (name == null || product.Name == name) &&
                (type == null || product.Type == type) &&
                (status == null || InventoryStatus(product.Id) == status))
            .ToList();
    }

    private static string InventoryStatus(int productId) => PRODUCTS[productId].Inventory == 0 ? "sold" : "available";

    public static void AddOrder(Order order)
    {
        ORDERS[order.Id] = order;
    }
    
    public static Order GetOrder(int id)
    {
        if (!ORDERS.ContainsKey(id))
        {
            // Handle the case where the order ID is not found
            throw new KeyNotFoundException($"Order with ID {id} not found.");
        }
        return ORDERS[id];
    }

    public static void DeleteOrder(int id)
    {
        ORDERS.Remove(id);
    }

    public static List<Order> FindOrders(OrderStatus? status = null, int? productId = null)
    {
        return ORDERS.Values
            .Where(order => 
                    (status == null || order.Status == status) &&   // Check if status has value
                    (!productId.HasValue || order.Productid == productId)   // Check if productId has value
            )
            .ToList();
    }

    public static void UpdateOrder(Order updatedOrder)
    {
        ORDERS[updatedOrder.Id] = updatedOrder;
    }

    public static void ReserveProductInventory(int productId, int count)
    {
        if(count <= 0)
        {
            throw new ValidationException("Invalid count. Quantity must be greater than zero.");
        }

        // Check if the product exists
        if (!PRODUCTS.ContainsKey(productId))
        {
            throw new ValidationException($"Product Id {productId} does not exist.");
        }

        var updatedProduct = PRODUCTS[productId];

        // Check if the product has enough inventory
        if (updatedProduct.Inventory < count)
        {
            throw new ValidationException($"Not enough inventory for Product Id {productId}. Available: {updatedProduct.Inventory}, Requested: {count}.");
        }

        // Deduct inventory after passing validation
        updatedProduct.Inventory -= count;
        PRODUCTS[productId] = updatedProduct;
    }

    public static void UpdateProductImage(int id, string imageFileName)
    {
        PRODUCT_IMAGE[id] = imageFileName;
    }
}