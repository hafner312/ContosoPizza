using ContosoPizza.Data;
using ContosoPizza.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoPizza.Services;

public class OrderService
{
    private readonly PizzaContext _context;

    public OrderService(PizzaContext context)
    {
        _context = context;
    }

    public Order PlaceOrder(string customerName, string address, PaymentMethod paymentMethod, List<CartLine> lines)
    {
        var order = new Order
        {
            CustomerName = customerName,
            Address = address,
            PaymentMethod = paymentMethod,
            PlacedAt = DateTime.Now,
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
        _context.Orders.Include(o => o.Items).FirstOrDefault(o => o.Id == id);

    public List<Order> GetRecentOrders(int take = 20) =>
        _context.Orders
            .Include(o => o.Items)
            .OrderByDescending(o => o.PlacedAt)
            .Take(take)
            .ToList();
}
