using ContosoPizza.Data;
using ContosoPizza.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<VisitorService>();
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
// auf eine im Repo mitgelieferte .db-Datei zu verlassen. Die Speisekarte
// selbst wird nicht mehr hier global befuellt, sondern pro Besucher lazily
// durch PizzaService.GetPizzas() (siehe dort) - jeder Besucher bekommt seine
// eigene Kopie statt sich eine gemeinsame mit allen zu teilen.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PizzaContext>();
    db.Database.Migrate();
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
