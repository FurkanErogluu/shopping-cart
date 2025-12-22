public interface IProductService
{
    Task<ProductDto> GetProductByIdAsync(int id);
    Task<ProductDto> GetProductByNameAsync(string name);
    Task<List<ProductDto>> GetAllProductsAsync();
}