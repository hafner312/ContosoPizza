using ContosoPizza.Data;
using ContosoPizza.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoPizza.Services;

// Bestellungen (inkl. Name/Adresse) sind auf den aktuellen Besucher (siehe
// VisitorService) skopiert, damit niemand die Bestelldaten anderer
// gleichzeitiger Besucher der Live-Demo einsehen kann.
public class OrderService
{
    private readonly PizzaContext _context;
    private readonly VisitorService _visitor;

    public OrderService(PizzaContext context, VisitorService visitor)
    {
        _context = context;
        _visitor = visitor;
    }

    public Order PlaceOrder(string customerName, string address, PaymentMethod paymentMethod, List<CartLine> lines)
    {
        var order = new Order
        {
            CustomerName = customerName,
            Address = address,
            PaymentMethod = paymentMethod,
            PlacedAt = DateTime.Now,
            OwnerId = _visitor.GetOwnerId(),
            Items = lines.Select(l => new OrderItem
            {
                PizzaId = l.Pizza.Id,
                PizzaName = $"{l.Pizza.Name} ({l.Pizza.Size})",
                UnitPrice = l.Pizza.Price,
                Quantity = l.Quantity,
            }).ToList(),
        };

        _context.Orders.Add(order);
        _context.SaveChanges();
        return order;
    }

    public Order? GetOrder(int id) =>
        _context.Orders.Include(o => o.Items)
            .FirstOrDefault(o => o.Id == id && o.OwnerId == _visitor.GetOwnerId());

    public List<Order> GetRecentOrders(int take = 20)
    {
        var ownerId = _visitor.GetOwnerId();
        return _context.Orders
            .Include(o => o.Items)
            .Where(o => o.OwnerId == ownerId)
            .OrderByDescending(o => o.PlacedAt)
            .Take(take)
            .ToList();
    }
}
