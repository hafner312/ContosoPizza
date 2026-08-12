namespace ContosoPizza.Models;

// Eine einzelne Zeile im Warenkorb: eine Pizza (Grösse bereits Teil der
// Pizza-Zeile selbst) mit der gewünschten Menge.
public class CartLine
{
    public Pizza Pizza { get; set; } = default!;
    public int Quantity { get; set; }
    public decimal LineTotal => Pizza.Price * Quantity;
}
