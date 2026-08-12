using ContosoPizza.Data;
using ContosoPizza.Models;
using ContosoPizza.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<PizzaService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.IsEssential = true;
});
builder.Services.AddRazorPages();
builder.Services.AddDbContext<PizzaContext>(options =>
    options.UseSqlite("Data Source=ContosoPizza.db"));

var app = builder.Build();

// Datenbank beim Start anlegen/aktualisieren - der SQLite-Container hat bei
// jedem Deploy ein frisches, leeres Dateisystem, daher reicht es nicht, sich
// auf eine im Repo mitgelieferte .db-Datei zu verlassen.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PizzaContext>();
    db.Database.Migrate();
    SeedMenu(db);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Render (und aehnliche PaaS-Anbieter) terminieren TLS am Edge und leiten
// intern per HTTP weiter - ein zusaetzlicher Redirect hier wuerde eine
// Redirect-Schlaufe verursachen.
if (Environment.GetEnvironmentVariable("RENDER") != "true")
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapRazorPages();

app.Run();

// Befüllt eine leere Speisekarte einmalig mit einem realistischen Sortiment,
// damit die Demo sofort etwas zu bestellen anbietet statt einer leeren Liste.
static void SeedMenu(PizzaContext db)
{
    if (db.Pizzas == null || db.Pizzas.Any()) return;

    (string Name, string Description, bool GlutenFree, decimal Small, decimal Medium, decimal Large)[] menu =
    {
        ("Margherita", "Tomatensauce, Mozzarella, frisches Basilikum", false, 12.90m, 15.90m, 18.90m),
        ("Salami", "Tomatensauce, Mozzarella, würzige Salami", false, 14.90m, 17.90m, 20.90m),
        ("Prosciutto e Funghi", "Tomatensauce, Mozzarella, Schinken, frische Champignons", false, 15.90m, 18.90m, 21.90m),
        ("Diavola", "Tomatensauce, Mozzarella, scharfe Salami, Peperoncini", false, 15.90m, 18.90m, 21.90m),
        ("Quattro Formaggi", "Tomatensauce, vier Käsesorten, Oregano", false, 16.90m, 19.90m, 22.90m),
        ("Vegetaria", "Tomatensauce, Mozzarella, Zucchini, Peperoni, Auberginen, Cherrytomaten", true, 15.90m, 18.90m, 21.90m),
    };

    var pizzas = new List<Pizza>();
    foreach (var item in menu)
    {
        pizzas.Add(new Pizza { Name = item.Name, Description = item.Description, Size = PizzaSize.Small, IsGlutenFree = item.GlutenFree, Price = item.Small });
        pizzas.Add(new Pizza { Name = item.Name, Description = item.Description, Size = PizzaSize.Medium, IsGlutenFree = item.GlutenFree, Price = item.Medium });
        pizzas.Add(new Pizza { Name = item.Name, Description = item.Description, Size = PizzaSize.Large, IsGlutenFree = item.GlutenFree, Price = item.Large });
    }

    db.Pizzas!.AddRange(pizzas);
    db.SaveChanges();
}
