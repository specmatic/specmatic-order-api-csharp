using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using specmatic_order_api_csharp.models;
using specmatic_order_api_csharp.services;
using System.Diagnostics.CodeAnalysis;
namespace specmatic_order_api_csharp.controllers // Replace with your actual namespace
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    [Route("products")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }


        [HttpGet]
        public ActionResult<List<Product>> GetAllProducts([FromQuery] ProductType? type)
        {
            return _productService.GetAllProducts(type);
        }

        [HttpGet("{id}")]
        public ActionResult<Product> Get(int id)
        {
            return _productService.GetProduct(id);
        }

        [HttpPost]
        public ActionResult<IdResponse> Create([FromBody] Product newProduct)
        {
            var productId = _productService.AddProduct(newProduct);
            return Ok(productId);
        }
        
        [HttpPost("{id}")]
        public ActionResult<IdResponse> Update([FromBody] Product updatedProduct,int id)
        {
            _productService.UpdateProduct(updatedProduct,id);
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _productService.DeleteProduct(id);
            return Ok();
        }
        

        [HttpPut("{id}/image")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        public IActionResult UploadImage(IFormFile image, int id)
        {
            if (image != null)
            {
                using (var stream = new System.IO.MemoryStream())
                {
                    image.CopyTo(stream);
                    var imageBytes = stream.ToArray();
                    _productService.AddImage(id, image.FileName, imageBytes);
                }
            }
        
            var response = new Dictionary<string, object>
            {
                { "message", "Product image updated successfully" },
                { "productId", id }
            };
        
            return Ok(response);
        }
    }
}
