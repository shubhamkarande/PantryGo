using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PantryGo.Api.Models.DTOs;
using PantryGo.Api.Services;

namespace PantryGo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AddressesController : ControllerBase
{
    private readonly IAddressService _addressService;
    
    public AddressesController(IAddressService addressService)
    {
        _addressService = addressService;
    }
    
    /// <summary>
    /// Get current user's addresses
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<AddressDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAddresses()
    {
        var userId = User.GetUserId();
        var addresses = await _addressService.GetUserAddressesAsync(userId);
        return Ok(ApiResponse<List<AddressDto>>.Ok(addresses));
    }
    
    /// <summary>
    /// Create a new address
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AddressDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAddress([FromBody] CreateAddressRequest request)
    {
        var userId = User.GetUserId();
        var address = await _addressService.CreateAddressAsync(userId, request);
        return CreatedAtAction(nameof(GetAddress), new { id = address.Id }, 
            ApiResponse<AddressDto>.Ok(address, "Address created"));
    }
    
    /// <summary>
    /// Get address by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<AddressDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAddress(Guid id)
    {
        var address = await _addressService.GetAddressByIdAsync(id);
        
        if (address == null)
        {
            return NotFound(ApiResponse<object>.Fail("Address not found"));
        }
        
        return Ok(ApiResponse<AddressDto>.Ok(address));
    }
    
    /// <summary>
    /// Update an address
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<AddressDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAddress(Guid id, [FromBody] CreateAddressRequest request)
    {
        var userId = User.GetUserId();
        var address = await _addressService.UpdateAddressAsync(userId, id, request);
        
        if (address == null)
        {
            return NotFound(ApiResponse<object>.Fail("Address not found"));
        }
        
        return Ok(ApiResponse<AddressDto>.Ok(address, "Address updated"));
    }
    
    /// <summary>
    /// Delete an address
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAddress(Guid id)
    {
        var userId = User.GetUserId();
        var result = await _addressService.DeleteAddressAsync(userId, id);
        
        if (!result)
        {
            return NotFound(ApiResponse<object>.Fail("Address not found"));
        }
        
        return NoContent();
    }
    
    /// <summary>
    /// Set address as default
    /// </summary>
    [HttpPut("{id}/default")]
    [ProducesResponseType(typeof(ApiResponse<AddressDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetDefaultAddress(Guid id)
    {
        var userId = User.GetUserId();
        var address = await _addressService.SetDefaultAddressAsync(userId, id);
        
        if (address == null)
        {
            return NotFound(ApiResponse<object>.Fail("Address not found"));
        }
        
        return Ok(ApiResponse<AddressDto>.Ok(address, "Default address updated"));
    }
}
