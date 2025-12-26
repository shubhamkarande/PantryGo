using Microsoft.EntityFrameworkCore;
using PantryGo.Api.Data;
using PantryGo.Api.Models.DTOs;
using PantryGo.Api.Models.Entities;

namespace PantryGo.Api.Services;

public interface IAddressService
{
    Task<List<AddressDto>> GetUserAddressesAsync(Guid userId);
    Task<AddressDto?> GetAddressByIdAsync(Guid addressId);
    Task<AddressDto> CreateAddressAsync(Guid userId, CreateAddressRequest request);
    Task<AddressDto?> UpdateAddressAsync(Guid userId, Guid addressId, CreateAddressRequest request);
    Task<bool> DeleteAddressAsync(Guid userId, Guid addressId);
    Task<AddressDto?> SetDefaultAddressAsync(Guid userId, Guid addressId);
}

public class AddressService : IAddressService
{
    private readonly AppDbContext _context;
    
    public AddressService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<AddressDto>> GetUserAddressesAsync(Guid userId)
    {
        return await _context.Addresses
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.IsDefault)
            .ThenBy(a => a.Label)
            .Select(a => MapToDto(a))
            .ToListAsync();
    }
    
    public async Task<AddressDto?> GetAddressByIdAsync(Guid addressId)
    {
        var address = await _context.Addresses.FindAsync(addressId);
        return address == null ? null : MapToDto(address);
    }
    
    public async Task<AddressDto> CreateAddressAsync(Guid userId, CreateAddressRequest request)
    {
        // If setting as default, unset other defaults
        if (request.IsDefault)
        {
            await UnsetDefaultAddresses(userId);
        }
        
        var address = new Address
        {
            UserId = userId,
            Label = request.Label,
            AddressLine = request.AddressLine,
            City = request.City,
            Pincode = request.Pincode,
            IsDefault = request.IsDefault
        };
        
        // If this is the first address, make it default
        var hasAddresses = await _context.Addresses.AnyAsync(a => a.UserId == userId);
        if (!hasAddresses)
        {
            address.IsDefault = true;
        }
        
        _context.Addresses.Add(address);
        await _context.SaveChangesAsync();
        
        return MapToDto(address);
    }
    
    public async Task<AddressDto?> UpdateAddressAsync(Guid userId, Guid addressId, CreateAddressRequest request)
    {
        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);
        
        if (address == null) return null;
        
        if (request.IsDefault && !address.IsDefault)
        {
            await UnsetDefaultAddresses(userId);
        }
        
        address.Label = request.Label;
        address.AddressLine = request.AddressLine;
        address.City = request.City;
        address.Pincode = request.Pincode;
        address.IsDefault = request.IsDefault;
        
        await _context.SaveChangesAsync();
        
        return MapToDto(address);
    }
    
    public async Task<bool> DeleteAddressAsync(Guid userId, Guid addressId)
    {
        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);
        
        if (address == null) return false;
        
        _context.Addresses.Remove(address);
        await _context.SaveChangesAsync();
        
        // If deleted address was default, set a new default
        if (address.IsDefault)
        {
            var newDefault = await _context.Addresses
                .FirstOrDefaultAsync(a => a.UserId == userId);
            
            if (newDefault != null)
            {
                newDefault.IsDefault = true;
                await _context.SaveChangesAsync();
            }
        }
        
        return true;
    }
    
    public async Task<AddressDto?> SetDefaultAddressAsync(Guid userId, Guid addressId)
    {
        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);
        
        if (address == null) return null;
        
        await UnsetDefaultAddresses(userId);
        
        address.IsDefault = true;
        await _context.SaveChangesAsync();
        
        return MapToDto(address);
    }
    
    private async Task UnsetDefaultAddresses(Guid userId)
    {
        var defaultAddresses = await _context.Addresses
            .Where(a => a.UserId == userId && a.IsDefault)
            .ToListAsync();
        
        foreach (var addr in defaultAddresses)
        {
            addr.IsDefault = false;
        }
        
        await _context.SaveChangesAsync();
    }
    
    private static AddressDto MapToDto(Address address) => new()
    {
        Id = address.Id,
        Label = address.Label,
        AddressLine = address.AddressLine,
        City = address.City,
        Pincode = address.Pincode,
        IsDefault = address.IsDefault
    };
}
