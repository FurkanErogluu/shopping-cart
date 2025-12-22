using ShoppingCart.Entities;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);
    Task<Product?> GetByNameAsync(string name);
    Task<List<Product>> GetAllAsync();
}