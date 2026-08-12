using ContosoPizza.Models;
using ContosoPizza.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContosoPizza.Pages;

public class OrderConfirmationModel : PageModel
{
    private readonly OrderService _orderService;

    public Order? Order { get; set; }
    public int EstimatedMinutes { get; set; }

    public OrderConfirmationModel(OrderService orderService)
    {
        _orderService = orderService;
    }

    public IActionResult OnGet(int id)
    {
        Order = _orderService.GetOrder(id);
        if (Order == null)
        {
            return RedirectToPage("/Menu");
        }

        // Deterministisch aus der Bestellnummer abgeleitet, damit die
        // Schätzung bei jedem Seitenaufruf gleich bleibt.
        EstimatedMinutes = 25 + (Order.Id * 7 % 20);
        return Page();
    }
}
