using Microsoft.EntityFrameworkCore;
// Importiert Entity Framework Core.
// DbContext und DbSet kommen beide aus diesem Namespace.

namespace ContosoPizza.Data
{
    // -------------------------------------------------------------------------
    // PizzaContext
    // ------------
    // Diese Klasse repräsentiert den *Datenbank-Kontext* für EF Core.
    //
    // Bedeutung:
    //  - Sie stellt die Verbindung zur Datenbank her.
    //  - Sie definiert, welche Tabellen in der Datenbank existieren sollen.
    //  - Sie ermöglicht Abfragen (SELECT), Einfügen (INSERT), Löschen (DELETE)
    //    und Ändern (UPDATE) von Daten.
    //
    // EF Core benutzt diese Klasse, um:
    //  - Datenbanktabellen zu erstellen (Migrationen)
    //  - Datenbankzugriffe auszuführen
    // -------------------------------------------------------------------------
    public class PizzaContext : DbContext
    {
        // ---------------------------------------------------------------------
        // KONSTRUKTOR
        //
        // Der Konstruktor nimmt DbContextOptions<PizzaContext> entgegen.
        //
        // Warum?
        //  - Die Optionen enthalten die Konfigurationsdaten (z. B. SQLite, ConnectionString)
        //  - Diese Konfiguration wird in Program.cs erstellt:
        //
        //      builder.Services.AddDbContext<PizzaContext>(options =>
        //          options.UseSqlite("Data Source=ContosoPizza.db"));
        //
        // Durch das ": base(options)" werden diese Einstellungen an die
        // Basisklasse DbContext übergeben, damit EF Core weiß:
        //  - welche Datenbank genutzt werden soll
        //  - wie die Verbindung hergestellt wird
        // ---------------------------------------------------------------------
        public PizzaContext(DbContextOptions<PizzaContext> options)
            : base(options)
        {
        }

        // ---------------------------------------------------------------------
        // PROPERTY: Pizzas
        //
        // DbSet<Pizza> repräsentiert eine Tabelle in der Datenbank.
        //
        // Bedeutung eines DbSet:
        //  - Jeder Eintrag im DbSet entspricht einer Tabellenzeile.
        //  - Der Typ Pizza bestimmt die Struktur der Spalten (Entity).
        //  - EF Core erstellt automatisch eine Tabelle "Pizzas".
        //
        // Nullable (?), weil EF Core interne Dinge zur Initialisierung macht.
        //
        // Beispiel:
        //  _context.Pizzas.ToList()          => SELECT * FROM Pizzas
        //  _context.Pizzas.Add(pizza)        => INSERT INTO Pizzas (...)
        //  _context.Pizzas.Remove(pizza)     => DELETE FROM Pizzas WHERE Id=...
        //
        // Das ist die einzige Tabelle in diesem Projekt.
        // ---------------------------------------------------------------------
        public DbSet<ContosoPizza.Models.Pizza>? Pizzas { get; set; }

        // Bestellungen und ihre Positionen (siehe Models/Order.cs, Models/OrderItem.cs).
        public DbSet<ContosoPizza.Models.Order> Orders { get; set; } = default!;
        public DbSet<ContosoPizza.Models.OrderItem> OrderItems { get; set; } = default!;
    }
}
