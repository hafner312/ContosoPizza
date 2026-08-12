using System.ComponentModel.DataAnnotations;

namespace ContosoPizza.Models;

public class Pizza
{
    public int Id { get; set; }

    [Required]
    public string? Name { get; set; }

    [StringLength(300)]
    public string? Description { get; set; }

    public PizzaSize Size { get; set; }
    public bool IsGlutenFree { get; set; }

    [Range(0.01, 9999.99)]
    public decimal Price { get; set; }

    // Anonyme Besucher-Kennung (siehe VisitorService) - jeder Besucher
    // bekommt seine eigene Speisekarte, damit niemand die Karte eines
    // anderen gleichzeitigen Besuchers leerraeumen kann.
    public string OwnerId { get; set; } = string.Empty;
}

public enum PizzaSize
{ Small, Medium, Large }