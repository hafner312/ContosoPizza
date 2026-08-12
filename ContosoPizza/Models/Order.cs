using System.ComponentModel.DataAnnotations;

namespace ContosoPizza.Models;

public class Order
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Address { get; set; } = string.Empty;

    public PaymentMethod PaymentMethod { get; set; }

    public DateTime PlacedAt { get; set; } = DateTime.Now;

    public List<OrderItem> Items { get; set; } = new();

    public decimal Total => Items.Sum(i => i.UnitPrice * i.Quantity);
}

public enum PaymentMethod
{
    Barzahlung,
    Karte,
    Twint
}
