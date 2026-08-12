using System.Text.Json;
using ContosoPizza.Data;
using ContosoPizza.Models;
using Microsoft.AspNetCore.Http;

namespace ContosoPizza.Services;

// Der Warenkorb wird in der Session gehalten (PizzaId -> Menge, als JSON),
// damit keine Anmeldung nötig ist und trotzdem jeder Besucher seinen
// eigenen Warenkorb hat.
public class CartService
{
    private const string SessionKey = "cart";

    private readonly PizzaContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CartService(PizzaContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    private ISession Session => _httpContextAccessor.HttpContext!.Session;

    private Dictionary<int, int> ReadCart()
    {
        var json = Session.GetString(SessionKey);
        if (string.IsNullOrEmpty(json))
        {
            return new Dictionary<int, int>();
        }
        return JsonSerializer.Deserialize<Dictionary<int, int>>(json) ?? new Dictionary<int, int>();
    }

    private void WriteCart(Dictionary<int, int> cart)
    {
        Session.SetString(SessionKey, JsonSerializer.Serialize(cart));
    }

    public void AddToCart(int pizzaId, int quantity = 1)
    {
        if (quantity <= 0) return;
        var cart = ReadCart();
        cart[pizzaId] = cart.GetValueOrDefault(pizzaId) + quantity;
        WriteCart(cart);
    }

    public void SetQuantity(int pizzaId, int quantity)
    {
        var cart = ReadCart();
        if (quantity <= 0)
        {
            cart.Remove(pizzaId);
        }
        else
        {
            cart[pizzaId] = quantity;
        }
        WriteCart(cart);
    }

    public void RemoveFromCart(int pizzaId)
    {
        var cart = ReadCart();
        cart.Remove(pizzaId);
        WriteCart(cart);
    }

    public void Clear()
    {
        Session.Remove(SessionKey);
    }

    public List<CartLine> GetLines()
    {
        var cart = ReadCart();
        if (cart.Count == 0 || _context.Pizzas == null) return new List<CartLine>();

        var pizzas = _context.Pizzas
            .Where(p => cart.Keys.Contains(p.Id))
            .ToDictionary(p => p.Id);

        return cart
            .Where(kv => pizzas.ContainsKey(kv.Key))
            .Select(kv => new CartLine { Pizza = pizzas[kv.Key], Quantity = kv.Value })
            .ToList();
    }

    public int ItemCount() => ReadCart().Values.Sum();

    public decimal Total() => GetLines().Sum(l => l.LineTotal);
}
