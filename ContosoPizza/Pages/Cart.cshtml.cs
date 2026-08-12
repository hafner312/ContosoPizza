using ContosoPizza.Models;
using ContosoPizza.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContosoPizza.Pages;

public class CartModel : PageModel
{
    private readonly CartService _cartService;

    public List<CartLine> Lines { get; set; } = new();
    public decimal Total { get; set; }

    public CartModel(CartService cartService)
    {
        _cartService = cartService;
    }

    public void OnGet()
    {
        Load();
    }

    public IActionResult OnPostUpdateQuantity(int pizzaId, int quantity)
    {
        _cartService.SetQuantity(pizzaId, quantity);
        return RedirectToPage();
    }

    public IActionResult OnPostRemove(int pizzaId)
    {
        _cartService.RemoveFromCart(pizzaId);
        return RedirectToPage();
    }

    private void Load()
    {
        Lines = _cartService.GetLines();
        Total = _cartService.Total();
    }
}
