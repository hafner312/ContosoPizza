using System.ComponentModel.DataAnnotations;
using ContosoPizza.Models;
using ContosoPizza.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContosoPizza.Pages;

public class CheckoutModel : PageModel
{
    private readonly CartService _cartService;
    private readonly OrderService _orderService;

    public List<CartLine> Lines { get; set; } = new();
    public decimal Total { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Bitte gib deinen Namen an.")]
    [StringLength(100)]
    public string CustomerName { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Bitte gib deine Lieferadresse an.")]
    [StringLength(200)]
    public string Address { get; set; } = string.Empty;

    [BindProperty]
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Karte;

    public CheckoutModel(CartService cartService, OrderService orderService)
    {
        _cartService = cartService;
        _orderService = orderService;
    }

    public IActionResult OnGet()
    {
        Lines = _cartService.GetLines();
        Total = _cartService.Total();
        if (Lines.Count == 0)
        {
            return RedirectToPage("/Cart");
        }
        return Page();
    }

    public IActionResult OnPost()
    {
        Lines = _cartService.GetLines();
        Total = _cartService.Total();

        if (Lines.Count == 0)
        {
            return RedirectToPage("/Cart");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var order = _orderService.PlaceOrder(CustomerName, Address, PaymentMethod, Lines);
        _cartService.Clear();

        return RedirectToPage("/OrderConfirmation", new { id = order.Id });
    }
}
