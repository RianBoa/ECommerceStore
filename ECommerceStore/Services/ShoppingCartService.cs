namespace ECommerceStore.Services;
using ECommerceStore.Models;
using System.Collections.Generic;
using System.Linq;

    public class ShoppingCartService
    {
    private readonly ApplicationDbContext _context; // Контекст базы данных
    private List<ShoppingCartItem> _cartItems = new List<ShoppingCartItem>(); // Локальная корзина

    public ShoppingCartService(ApplicationDbContext context)
    {
        _context = context;
    }

    // Метод для добавления товара в корзину
    public void AddToCart(Product product)
    {
        try
        {
            var cartItem = _context.ShoppingCartItems
                .SingleOrDefault(item => item.ProductId == product.Id);

            if (cartItem == null)
            {
                cartItem = new ShoppingCartItem
                {
                    ProductId = product.Id,
                    Product = product,
                    Quantity = 1
                };
                _context.ShoppingCartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity++;
            }

            _context.SaveChanges();
            Console.WriteLine("Changes saved to the database");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving changes: {ex.Message}");
        }

    }
   
    

    // Метод для получения всех товаров в корзине
    public IEnumerable<ShoppingCartItem> GetCartItems()
    {
        return _cartItems;
    }

    // Метод для очистки корзины
    public void ClearCart()
    {
        _cartItems.Clear();
    }

    // Метод для удаления товара из корзины
    public void RemoveFromCart(Product product)
    {
        var cartItem = _cartItems.SingleOrDefault(item => item.Product?.Id == product.Id);

        if (cartItem != null)
        {
            _cartItems.Remove(cartItem);
        }
    }
}



