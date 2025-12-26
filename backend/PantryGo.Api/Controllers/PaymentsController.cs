using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PantryGo.Api.Models.DTOs;
using PantryGo.Api.Services;

namespace PantryGo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    
    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }
    
    /// <summary>
    /// Create a Razorpay payment order
    /// </summary>
    [HttpPost("create")]
    [ProducesResponseType(typeof(ApiResponse<PaymentOrderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePaymentOrder([FromBody] CreatePaymentRequest request)
    {
        var userId = User.GetUserId();
        var result = await _paymentService.CreatePaymentOrderAsync(userId, request.OrderId);
        
        if (result == null)
        {
            return BadRequest(ApiResponse<object>.Fail("Unable to create payment order. Order may not exist or already paid."));
        }
        
        return Ok(ApiResponse<PaymentOrderResponse>.Ok(result, "Payment order created"));
    }
    
    /// <summary>
    /// Verify Razorpay payment
    /// </summary>
    [HttpPost("verify")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyPayment([FromBody] VerifyPaymentRequest request)
    {
        var result = await _paymentService.VerifyPaymentAsync(request);
        
        if (!result)
        {
            return BadRequest(ApiResponse<object>.Fail("Payment verification failed"));
        }
        
        return Ok(ApiResponse<object>.Ok(new { verified = true }, "Payment verified successfully"));
    }
}
