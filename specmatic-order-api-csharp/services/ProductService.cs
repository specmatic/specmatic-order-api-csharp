using specmatic_order_api_csharp.exceptions;
using specmatic_order_api_csharp.filestorage;
using specmatic_order_api_csharp.models;

namespace specmatic_order_api_csharp.services
{
    public class ProductService
    {
        public Product GetProduct(int id)
        {
            return DB.FindProduct(id);
        }
        
        public List<Product> GetAllProducts(ProductType? type)
        {
            return DB.FindProducts(type:type);
        }

        public IdResponse AddProduct(Product product)
        {
            DB.AddProduct(product);
            return new IdResponse(product.Id);
        }
        public void UpdateProduct(Product updatedProduct, int id)
        {
            DB.UpdateProduct(updatedProduct);
        }

        public void DeleteProduct(int id)
        {
            DB.DeleteProduct(id);
        }

        public void AddImage(int id, string imageFileName, byte[] bytes)
        {
            string canonicalImageFilePath = LocalFileSystem.SaveImage(imageFileName, bytes);
            DB.UpdateProductImage(id, canonicalImageFilePath);
        }
    }
}