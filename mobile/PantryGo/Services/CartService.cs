using System.Collections.ObjectModel;
using PantryGo.Models;

namespace PantryGo.Services;

public interface ICartService
{
    ObservableCollection<CartItem> Items { get; }
    int ItemCount { get; }
    decimal TotalPrice { get; }
    event Action? CartChanged;
    
    void AddItem(Product product, int quantity = 1);
    void UpdateQuantity(Guid productId, int quantity);
    void RemoveItem(Guid productId);
    void ClearCart();
    List<CreateOrderItemRequest> GetOrderItems();
}

public class CartService : ICartService
{
    private readonly ObservableCollection<CartItem> _items = new();
    
    public ObservableCollection<CartItem> Items => _items;
    
    public int ItemCount => _items.Sum(i => i.Quantity);
    
    public decimal TotalPrice => _items.Sum(i => i.TotalPrice);
    
    public event Action? CartChanged;
    
    public void AddItem(Product product, int quantity = 1)
    {
        var existingItem = _items.FirstOrDefault(i => i.ProductId == product.Id);
        
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
            if (existingItem.Quantity > existingItem.Stock)
            {
                existingItem.Quantity = existingItem.Stock;
            }
        }
        else
        {
            _items.Add(new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ProductImageUrl = product.ImageUrl,
                Price = product.Price,
                Unit = product.Unit,
                Quantity = Math.Min(quantity, product.Stock),
                Stock = product.Stock
            });
        }
        
        CartChanged?.Invoke();
    }
    
    public void UpdateQuantity(Guid productId, int quantity)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        
        if (item != null)
        {
            if (quantity <= 0)
            {
                RemoveItem(productId);
            }
            else
            {
                item.Quantity = Math.Min(quantity, item.Stock);
                CartChanged?.Invoke();
            }
        }
    }
    
    public void RemoveItem(Guid productId)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        
        if (item != null)
        {
            _items.Remove(item);
            CartChanged?.Invoke();
        }
    }
    
    public void ClearCart()
    {
        _items.Clear();
        CartChanged?.Invoke();
    }
    
    public List<CreateOrderItemRequest> GetOrderItems()
    {
        return _items.Select(i => new CreateOrderItemRequest
        {
            ProductId = i.ProductId,
            Quantity = i.Quantity
        }).ToList();
    }
}
