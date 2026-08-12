using ContosoPizza.Models;
using ContosoPizza.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContosoPizza.Pages;

public class MenuModel : PageModel
{
    private readonly PizzaService _pizzaService;
    private readonly CartService _cartService;

    public List<MenuItem> MenuItems { get; set; } = new();

    [TempData]
    public string? Confirmation { get; set; }

    public MenuModel(PizzaService pizzaService, CartService cartService)
    {
        _pizzaService = pizzaService;
        _cartService = cartService;
    }

    public void OnGet()
    {
        LoadMenu();
    }

    public IActionResult OnPostAddToCart(int pizzaId, int quantity = 1)
    {
        _cartService.AddToCart(pizzaId, quantity);
        Confirmation = "In den Warenkorb gelegt.";
        return RedirectToPage();
    }

    private void LoadMenu()
    {
        MenuItems = _pizzaService.GetPizzas()
            .GroupBy(p => p.Name ?? "")
            .Select(g => new MenuItem
            {
                Name = g.Key,
                Description = g.First().Description,
                IsGlutenFree = g.First().IsGlutenFree,
                Variants = g.OrderBy(p => p.Size).ToList(),
            })
            .OrderBy(m => m.Name)
            .ToList();
    }
}
