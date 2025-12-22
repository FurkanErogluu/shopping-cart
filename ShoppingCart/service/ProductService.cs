public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto> GetProductByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            throw new BusinessException("PRODUCT_NOT_FOUND", "Product not found");
        }

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            DefaultUnit = product.DefaultUnit,
            DefaultUnitName = product.DefaultUnitName
        };
    }

    public async Task<ProductDto> GetProductByNameAsync(string name)
    {
        var product = await _productRepository.GetByNameAsync(name);
        if (product == null)
        {
            throw new BusinessException("PRODUCT_NOT_FOUND", "Product not found");
        }

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            DefaultUnit = product.DefaultUnit,
            DefaultUnitName = product.DefaultUnitName
        };
    }

    public async Task<List<ProductDto>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(product => new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            DefaultUnit = product.DefaultUnit,
            DefaultUnitName = product.DefaultUnitName
        }).ToList();
    }
}