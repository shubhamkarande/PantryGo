using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PantryGo.Api.Models.DTOs;
using PantryGo.Api.Services;

namespace PantryGo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    
    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }
    
    /// <summary>
    /// Get products with optional filtering and pagination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<ProductDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts([FromQuery] ProductFilter filter)
    {
        var result = await _productService.GetProductsAsync(filter);
        return Ok(ApiResponse<PagedResponse<ProductDto>>.Ok(result));
    }
    
    /// <summary>
    /// Get product categories
    /// </summary>
    [HttpGet("categories")]
    [ProducesResponseType(typeof(ApiResponse<List<string>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _productService.GetCategoriesAsync();
        return Ok(ApiResponse<List<string>>.Ok(categories));
    }
    
    /// <summary>
    /// Get product by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        
        if (product == null)
        {
            return NotFound(ApiResponse<object>.Fail("Product not found"));
        }
        
        return Ok(ApiResponse<ProductDto>.Ok(product));
    }
    
    /// <summary>
    /// Create a new product (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
    {
        var product = await _productService.CreateProductAsync(request);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, 
            ApiResponse<ProductDto>.Ok(product, "Product created"));
    }
    
    /// <summary>
    /// Update a product (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductRequest request)
    {
        var product = await _productService.UpdateProductAsync(id, request);
        
        if (product == null)
        {
            return NotFound(ApiResponse<object>.Fail("Product not found"));
        }
        
        return Ok(ApiResponse<ProductDto>.Ok(product, "Product updated"));
    }
    
    /// <summary>
    /// Delete a product (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        var result = await _productService.DeleteProductAsync(id);
        
        if (!result)
        {
            return NotFound(ApiResponse<object>.Fail("Product not found"));
        }
        
        return NoContent();
    }
}
