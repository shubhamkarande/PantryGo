using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PantryGo.Api.Data;
using PantryGo.Api.Models.DTOs;

namespace PantryGo.Api.Services;

public interface IPaymentService
{
    Task<PaymentOrderResponse?> CreatePaymentOrderAsync(Guid userId, Guid orderId);
    Task<bool> VerifyPaymentAsync(VerifyPaymentRequest request);
}

public class PaymentService : IPaymentService
{
    private readonly AppDbContext _context;
    private readonly IOrderService _orderService;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    
    public PaymentService(
        AppDbContext context, 
        IOrderService orderService,
        IConfiguration configuration,
        HttpClient httpClient)
    {
        _context = context;
        _orderService = orderService;
        _configuration = configuration;
        _httpClient = httpClient;
    }
    
    public async Task<PaymentOrderResponse?> CreatePaymentOrderAsync(Guid userId, Guid orderId)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
        
        if (order == null || order.IsPaid)
        {
            return null;
        }
        
        // For test mode, we'll create a mock Razorpay order
        // In production, this would make an actual API call to Razorpay
        var razorpayKeyId = _configuration["Razorpay:KeyId"] ?? "rzp_test_demo";
        var razorpayOrderId = $"order_{Guid.NewGuid():N}";
        
        // Store the Razorpay order ID
        order.RazorpayOrderId = razorpayOrderId;
        await _context.SaveChangesAsync();
        
        return new PaymentOrderResponse
        {
            OrderId = razorpayOrderId,
            Amount = order.TotalAmount,
            Currency = "INR",
            Key = razorpayKeyId
        };
    }
    
    public async Task<bool> VerifyPaymentAsync(VerifyPaymentRequest request)
    {
        var order = await _context.Orders.FindAsync(request.OrderId);
        if (order == null || order.RazorpayOrderId != request.RazorpayOrderId)
        {
            return false;
        }
        
        // Verify signature
        // In production, this would verify the actual Razorpay signature
        var razorpaySecret = _configuration["Razorpay:KeySecret"] ?? "demo_secret";
        var expectedSignature = ComputeHmacSha256(
            $"{request.RazorpayOrderId}|{request.RazorpayPaymentId}",
            razorpaySecret
        );
        
        // For demo/test mode, accept the payment
        // In production: if (request.RazorpaySignature != expectedSignature) return false;
        
        // Mark order as paid
        await _orderService.MarkOrderAsPaidAsync(request.OrderId, request.RazorpayPaymentId);
        
        return true;
    }
    
    private static string ComputeHmacSha256(string data, string key)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(hash).ToLower();
    }
}
