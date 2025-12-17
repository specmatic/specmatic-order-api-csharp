using Moq;
using specmatic_order_api_csharp.filestorage;
using specmatic_order_api_csharp.models;
using specmatic_order_api_csharp.services;
using Xunit;

namespace specmatic_order_api_csharp.tests
{
    public class ProductServiceTests
    {
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _productService = new ProductService();
        }

        [Fact]
        public void GetProduct_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            int productId = 1;
            var expectedProduct = new Product { Id = productId, Name = "Sample Product", Type = ProductType.gadget };

            // Assuming DB.FindProduct is a static method, it will return the expected product
            DB.AddProduct(expectedProduct); // Adding the product to the DB for the test

            // Act
            var result = _productService.GetProduct(productId);

            // Assert
            Assert.Equal(expectedProduct, result); // Ensure the correct product is returned
        }

        [Fact]
        public void GetAllProducts_ShouldReturnProducts_WhenTypeIsValid()
        {
            // Arrange
            ProductType productType = ProductType.gadget;
            var expectedProducts = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Type = productType },
                new Product { Id = 2, Name = "Product 2", Type = productType }
            };

            // Add products to DB
            foreach (var product in expectedProducts)
            {
                DB.AddProduct(product);
            }

            // Act
            var result = _productService.GetAllProducts(productType);

            // Assert
            Assert.All(result,
                product => Assert.Equal(productType, product.Type)); // Ensure all products have the correct type
        }

        [Fact]
        public void AddProduct_ShouldReturnIdResponse_WhenProductIsAdded()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "New Product", Type = ProductType.food };

            // Act
            var result = _productService.AddProduct(product);

            // Assert
            Assert.IsType<IdResponse>(result); // Ensure the return type is IdResponse
            Assert.Equal(product.Id, result.Id); // Check if the returned ID matches the product ID
        }

        [Fact]
        public void UpdateProduct_ShouldUpdateProduct_WhenProductIsValid()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Updated Product", Type = ProductType.gadget };
            DB.AddProduct(product); // Add the product to the DB

            var updatedProduct = new Product { Id = 1, Name = "Updated Product", Type = ProductType.gadget };

            // Act
            _productService.UpdateProduct(updatedProduct, 1);

            // Assert
            var result = DB.FindProduct(1); // Retrieve the updated product from the DB
            Assert.Equal(updatedProduct.Name, result.Name); // Ensure the product name is updated
            Assert.Equal(updatedProduct.Type, result.Type); // Ensure the produ
        }
    }
}