using ContosoPizza.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContosoPizza.Components;

public class CartSummaryViewComponent : ViewComponent
{
    private readonly CartService _cartService;

    public CartSummaryViewComponent(CartService cartService)
    {
        _cartService = cartService;
    }

    public IViewComponentResult Invoke()
    {
        return View(_cartService.ItemCount());
    }
}
