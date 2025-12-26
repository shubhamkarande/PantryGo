using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PantryGo.Api.Models.DTOs;
using PantryGo.Api.Models.Enums;
using PantryGo.Api.Services;

namespace PantryGo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    
    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }
    
    /// <summary>
    /// Create a new order
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<OrderDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var userId = User.GetUserId();
        var order = await _orderService.CreateOrderAsync(userId, request);
        
        if (order == null)
        {
            return BadRequest(ApiResponse<object>.Fail("Failed to create order. Check address and product availability."));
        }
        
        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, 
            ApiResponse<OrderDto>.Ok(order, "Order created"));
    }
    
    /// <summary>
    /// Get current user's orders
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<OrderDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var userId = User.GetUserId();
        var orders = await _orderService.GetUserOrdersAsync(userId, page, pageSize);
        return Ok(ApiResponse<PagedResponse<OrderDto>>.Ok(orders));
    }
    
    /// <summary>
    /// Get order by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrder(Guid id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        
        if (order == null)
        {
            return NotFound(ApiResponse<object>.Fail("Order not found"));
        }
        
        // Check authorization - user can only see their own orders (unless admin/delivery)
        var userId = User.GetUserId();
        var userRole = User.GetUserRole();
        
        if (order.UserId != userId && userRole != UserRole.Admin && userRole != UserRole.Delivery)
        {
            return Forbid();
        }
        
        return Ok(ApiResponse<OrderDto>.Ok(order));
    }
    
    /// <summary>
    /// Update order status (Admin/Delivery only)
    /// </summary>
    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin,Delivery")]
    [ProducesResponseType(typeof(ApiResponse<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] UpdateOrderStatusRequest request)
    {
        var order = await _orderService.UpdateOrderStatusAsync(id, request.Status);
        
        if (order == null)
        {
            return NotFound(ApiResponse<object>.Fail("Order not found"));
        }
        
        return Ok(ApiResponse<OrderDto>.Ok(order, $"Order status updated to {request.Status}"));
    }
    
    /// <summary>
    /// Get all orders (Admin only)
    /// </summary>
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<OrderDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllOrders(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10,
        [FromQuery] OrderStatus? status = null)
    {
        var orders = await _orderService.GetAllOrdersAsync(page, pageSize, status);
        return Ok(ApiResponse<PagedResponse<OrderDto>>.Ok(orders));
    }
    
    /// <summary>
    /// Get delivery partner's assigned orders
    /// </summary>
    [HttpGet("delivery")]
    [Authorize(Roles = "Delivery")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<OrderDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDeliveryOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var userId = User.GetUserId();
        var orders = await _orderService.GetDeliveryOrdersAsync(userId, page, pageSize);
        return Ok(ApiResponse<PagedResponse<OrderDto>>.Ok(orders));
    }
    
    /// <summary>
    /// Assign delivery partner to order (Admin only)
    /// </summary>
    [HttpPut("{id}/assign")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignDeliveryPartner(Guid id, [FromQuery] Guid deliveryPartnerId)
    {
        var order = await _orderService.AssignDeliveryPartnerAsync(id, deliveryPartnerId);
        
        if (order == null)
        {
            return NotFound(ApiResponse<object>.Fail("Order or delivery partner not found"));
        }
        
        return Ok(ApiResponse<OrderDto>.Ok(order, "Delivery partner assigned"));
    }
}
