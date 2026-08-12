using ContosoPizza.Models;
using ContosoPizza.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContosoPizza.Pages;

public class OrdersModel : PageModel
{
    private readonly OrderService _orderService;

    public List<Order> Orders { get; set; } = new();

    public OrdersModel(OrderService orderService)
    {
        _orderService = orderService;
    }

    public void OnGet()
    {
        Orders = _orderService.GetRecentOrders();
    }
}
