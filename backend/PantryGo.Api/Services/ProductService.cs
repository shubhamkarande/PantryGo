using Microsoft.EntityFrameworkCore;
using PantryGo.Api.Data;
using PantryGo.Api.Models.DTOs;
using PantryGo.Api.Models.Entities;

namespace PantryGo.Api.Services;

public interface IProductService
{
    Task<PagedResponse<ProductDto>> GetProductsAsync(ProductFilter filter);
    Task<ProductDto?> GetProductByIdAsync(Guid id);
    Task<ProductDto> CreateProductAsync(CreateProductRequest request);
    Task<ProductDto?> UpdateProductAsync(Guid id, UpdateProductRequest request);
    Task<bool> DeleteProductAsync(Guid id);
    Task<List<string>> GetCategoriesAsync();
}

public class ProductService : IProductService
{
    private readonly AppDbContext _context;
    
    public ProductService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<PagedResponse<ProductDto>> GetProductsAsync(ProductFilter filter)
    {
        var query = _context.Products.Where(p => p.IsActive);
        
        // Apply filters
        if (!string.IsNullOrEmpty(filter.Category))
        {
            query = query.Where(p => p.Category.ToLower() == filter.Category.ToLower());
        }
        
        if (!string.IsNullOrEmpty(filter.Search))
        {
            var searchLower = filter.Search.ToLower();
            query = query.Where(p => 
                p.Name.ToLower().Contains(searchLower) || 
                (p.Description != null && p.Description.ToLower().Contains(searchLower)));
        }
        
        if (filter.MinPrice.HasValue)
        {
            query = query.Where(p => p.Price >= filter.MinPrice.Value);
        }
        
        if (filter.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= filter.MaxPrice.Value);
        }
        
        if (filter.InStock == true)
        {
            query = query.Where(p => p.Stock > 0);
        }
        
        var totalCount = await query.CountAsync();
        
        var items = await query
            .OrderBy(p => p.Category)
            .ThenBy(p => p.Name)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(p => MapToDto(p))
            .ToListAsync();
        
        return new PagedResponse<ProductDto>
        {
            Items = items,
            Page = filter.Page,
            PageSize = filter.PageSize,
            TotalCount = totalCount
        };
    }
    
    public async Task<ProductDto?> GetProductByIdAsync(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        return product == null ? null : MapToDto(product);
    }
    
    public async Task<ProductDto> CreateProductAsync(CreateProductRequest request)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Category = request.Category,
            Stock = request.Stock,
            ImageUrl = request.ImageUrl,
            Unit = request.Unit
        };
        
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        
        return MapToDto(product);
    }
    
    public async Task<ProductDto?> UpdateProductAsync(Guid id, UpdateProductRequest request)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return null;
        
        if (request.Name != null) product.Name = request.Name;
        if (request.Description != null) product.Description = request.Description;
        if (request.Price.HasValue) product.Price = request.Price.Value;
        if (request.Category != null) product.Category = request.Category;
        if (request.Stock.HasValue) product.Stock = request.Stock.Value;
        if (request.ImageUrl != null) product.ImageUrl = request.ImageUrl;
        if (request.Unit != null) product.Unit = request.Unit;
        if (request.IsActive.HasValue) product.IsActive = request.IsActive.Value;
        
        await _context.SaveChangesAsync();
        
        return MapToDto(product);
    }
    
    public async Task<bool> DeleteProductAsync(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return false;
        
        // Soft delete by marking as inactive
        product.IsActive = false;
        await _context.SaveChangesAsync();
        
        return true;
    }
    
    public async Task<List<string>> GetCategoriesAsync()
    {
        return await _context.Products
            .Where(p => p.IsActive)
            .Select(p => p.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
    }
    
    private static ProductDto MapToDto(Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Description = product.Description,
        Price = product.Price,
        Category = product.Category,
        Stock = product.Stock,
        ImageUrl = product.ImageUrl,
        Unit = product.Unit,
        IsActive = product.IsActive
    };
}
