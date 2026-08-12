namespace ContosoPizza.Models;

public class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order? Order { get; set; }

    public int PizzaId { get; set; }
    public Pizza? Pizza { get; set; }

    // Snapshot von Name/Preis zum Bestellzeitpunkt, damit spätere Änderungen
    // an der Speisekarte alte Bestellungen nicht rückwirkend verfälschen.
    public string PizzaName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}
