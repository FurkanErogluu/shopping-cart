using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


[ApiController]
[Route("api/[controller]")]
[Authorize]

public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("id")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetProductById(int id)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(id);
            return Ok(ApiResponse<ProductDto>.Ok(product));
        }
        catch (BusinessException ex)
        {
            return NotFound(ApiResponse<ProductDto>.Fail(ex.Code, ex.Message, 404));
        }
    }

    [HttpGet("by-name")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetProductByName([FromQuery] string name)
    {
        try
        {
            var product = await _productService.GetProductByNameAsync(name);
            return Ok(ApiResponse<ProductDto>.Ok(product));
        }
        catch (BusinessException ex)
        {
            return NotFound(ApiResponse<ProductDto>.Fail(ex.Code, ex.Message, 404));
        }
    }

    [HttpGet("all")]
    public async Task<ActionResult<ApiResponse<List<ProductDto>>>> GetAllProducts()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(ApiResponse<List<ProductDto>>.Ok(products));
    }

}