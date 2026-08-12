using ContosoPizza.Data;
using ContosoPizza.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<PizzaService>();
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

app.UseAuthorization();

app.MapRazorPages();

app.Run();