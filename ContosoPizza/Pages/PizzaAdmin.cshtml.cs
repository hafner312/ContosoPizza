using ContosoPizza.Models;
using ContosoPizza.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContosoPizza.Pages;

public class PizzaAdminModel : PageModel
{
    private readonly PizzaService _service;

    public IList<Pizza> PizzaList { get; set; } = default!;

    [BindProperty]
    public Pizza NewPizza { get; set; } = default!;

    public PizzaAdminModel(PizzaService service)
    {
        _service = service;
    }

    public void OnGet()
    {
        PizzaList = _service.GetPizzas();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid || NewPizza == null)
        {
            PizzaList = _service.GetPizzas();
            return Page();
        }

        _service.AddPizza(NewPizza);

        return RedirectToPage();
    }

    public IActionResult OnPostDelete(int id)
    {
        _service.DeletePizza(id);
        return RedirectToPage();
    }
}
