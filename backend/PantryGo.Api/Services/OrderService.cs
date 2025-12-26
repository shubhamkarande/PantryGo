using Microsoft.EntityFrameworkCore;
using PantryGo.Api.Data;
using PantryGo.Api.Models.DTOs;
using PantryGo.Api.Models.Entities;
using PantryGo.Api.Models.Enums;

namespace PantryGo.Api.Services;

public interface IOrderService
{
    Task<OrderDto?> CreateOrderAsync(Guid userId, CreateOrderRequest request);
    Task<OrderDto?> GetOrderByIdAsync(Guid orderId);
    Task<PagedResponse<OrderDto>> GetUserOrdersAsync(Guid userId, int page, int pageSize);
    Task<PagedResponse<OrderDto>> GetDeliveryOrdersAsync(Guid deliveryPartnerId, int page, int pageSize);
    Task<PagedResponse<OrderDto>> GetAllOrdersAsync(int page, int pageSize, OrderStatus? status);
    Task<OrderDto?> UpdateOrderStatusAsync(Guid orderId, OrderStatus status);
    Task<OrderDto?> AssignDeliveryPartnerAsync(Guid orderId, Guid deliveryPartnerId);
    Task<bool> MarkOrderAsPaidAsync(Guid orderId, string paymentId);
}

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;
    private readonly INotificationService _notificationService;
    
    public OrderService(AppDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }
    
    public async Task<OrderDto?> CreateOrderAsync(Guid userId, CreateOrderRequest request)
    {
        // Validate address belongs to user
        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.Id == request.AddressId && a.UserId == userId);
        
        if (address == null) return null;
        
        // Get products and validate stock
        var productIds = request.Items.Select(i => i.ProductId).ToList();
        var products = await _context.Products
            .Where(p => productIds.Contains(p.Id) && p.IsActive)
            .ToDictionaryAsync(p => p.Id);
        
        if (products.Count != request.Items.Count) return null;
        
        // Calculate total and validate stock
        decimal totalAmount = 0;
        var orderItems = new List<OrderItem>();
        
        foreach (var item in request.Items)
        {
            var product = products[item.ProductId];
            if (product.Stock < item.Quantity) return null;
            
            totalAmount += product.Price * item.Quantity;
            orderItems.Add(new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = product.Price
            });
        }
        
        // Create order
        var order = new Order
        {
            UserId = userId,
            AddressId = request.AddressId,
            Status = OrderStatus.Pending,
            TotalAmount = totalAmount,
            OrderItems = orderItems
        };
        
        // Reduce stock
        foreach (var item in request.Items)
        {
            products[item.ProductId].Stock -= item.Quantity;
        }
        
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        
        return await GetOrderByIdAsync(order.Id);
    }
    
    public async Task<OrderDto?> GetOrderByIdAsync(Guid orderId)
    {
        var order = await _context.Orders
            .Include(o => o.Address)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId);
        
        return order == null ? null : MapToDto(order);
    }
    
    public async Task<PagedResponse<OrderDto>> GetUserOrdersAsync(Guid userId, int page, int pageSize)
    {
        var query = _context.Orders
            .Include(o => o.Address)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .Where(o => o.UserId == userId);
        
        return await GetPagedOrdersAsync(query, page, pageSize);
    }
    
    public async Task<PagedResponse<OrderDto>> GetDeliveryOrdersAsync(Guid deliveryPartnerId, int page, int pageSize)
    {
        var query = _context.Orders
            .Include(o => o.Address)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .Where(o => o.DeliveryPartnerId == deliveryPartnerId);
        
        return await GetPagedOrdersAsync(query, page, pageSize);
    }
    
    public async Task<PagedResponse<OrderDto>> GetAllOrdersAsync(int page, int pageSize, OrderStatus? status)
    {
        var query = _context.Orders
            .Include(o => o.Address)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .AsQueryable();
        
        if (status.HasValue)
        {
            query = query.Where(o => o.Status == status.Value);
        }
        
        return await GetPagedOrdersAsync(query, page, pageSize);
    }
    
    public async Task<OrderDto?> UpdateOrderStatusAsync(Guid orderId, OrderStatus status)
    {
        var order = await _context.Orders
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == orderId);
        
        if (order == null) return null;
        
        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        
        // Send push notification
        await _notificationService.SendOrderStatusNotificationAsync(
            order.UserId, 
            orderId, 
            status
        );
        
        return await GetOrderByIdAsync(orderId);
    }
    
    public async Task<OrderDto?> AssignDeliveryPartnerAsync(Guid orderId, Guid deliveryPartnerId)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null) return null;
        
        var deliveryPartner = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == deliveryPartnerId && u.Role == UserRole.Delivery);
        
        if (deliveryPartner == null) return null;
        
        order.DeliveryPartnerId = deliveryPartnerId;
        order.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        
        return await GetOrderByIdAsync(orderId);
    }
    
    public async Task<bool> MarkOrderAsPaidAsync(Guid orderId, string paymentId)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null) return false;
        
        order.IsPaid = true;
        order.PaymentId = paymentId;
        order.Status = OrderStatus.Confirmed;
        order.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        
        // Send notification
        await _notificationService.SendOrderStatusNotificationAsync(
            order.UserId,
            orderId,
            OrderStatus.Confirmed
        );
        
        return true;
    }
    
    private async Task<PagedResponse<OrderDto>> GetPagedOrdersAsync(IQueryable<Order> query, int page, int pageSize)
    {
        var totalCount = await query.CountAsync();
        
        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return new PagedResponse<OrderDto>
        {
            Items = items.Select(MapToDto).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
    
    private static OrderDto MapToDto(Order order) => new()
    {
        Id = order.Id,
        UserId = order.UserId,
        Address = order.Address == null ? null : new AddressDto
        {
            Id = order.Address.Id,
            Label = order.Address.Label,
            AddressLine = order.Address.AddressLine,
            City = order.Address.City,
            Pincode = order.Address.Pincode,
            IsDefault = order.Address.IsDefault
        },
        Status = order.Status,
        TotalAmount = order.TotalAmount,
        IsPaid = order.IsPaid,
        CreatedAt = order.CreatedAt,
        UpdatedAt = order.UpdatedAt,
        Items = order.OrderItems.Select(oi => new OrderItemDto
        {
            Id = oi.Id,
            ProductId = oi.ProductId,
            ProductName = oi.Product?.Name ?? "",
            ProductImageUrl = oi.Product?.ImageUrl,
            Quantity = oi.Quantity,
            Price = oi.Price
        }).ToList()
    };
}
