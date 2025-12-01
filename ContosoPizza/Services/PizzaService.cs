using ContosoPizza.Data;      // Importiert den PizzaContext (DB-Kontext von EF Core)
using ContosoPizza.Models;    // Importiert das Pizza-Model (Entity-Klasse)

namespace ContosoPizza.Services
{
    // -------------------------------------------------------------------------
    // PizzaService
    // ----------
    // Diese Klasse ist die "Business-Logik-Schicht" zwischen Datenbank und UI.
    //
    // Aufgaben des Services:
    //  - Holt Pizza-Daten aus der Datenbank (GetPizzas)
    //  - Fügt neue Pizzen in die Datenbank ein (AddPizza)
    //  - Löscht Pizzen aus der Datenbank (DeletePizza)
    //
    // Warum ein Service?
    //  - PageModel soll KEINE DB-Zugriffe direkt machen
    //  - Wir haben eine saubere Layer-Trennung
    //  - Wiederverwendbarkeit: andere Seiten können denselben Service nutzen
    // -------------------------------------------------------------------------
    public class PizzaService
    {
        // ---------------------------------------------------------------------
        // PRIVATES FELD: _context
        //
        // Der PizzaContext ist der EF-Core-Datenbankkontext.
        // Er repräsentiert die Verbindung zur Datenbank und enthält Zugriff
        // auf die Tabellen (DbSet<Pizza> Pizzas).
        //
        // readonly bedeutet:
        //   - der Wert kann nur im Konstruktor gesetzt werden
        //   - danach darf er NICHT mehr verändert werden
        // ---------------------------------------------------------------------
        private readonly PizzaContext _context = default!;

        // ---------------------------------------------------------------------
        // KONSTRUKTOR
        //
        // Der DB-Kontext wird durch Dependency Injection aus Program.cs
        // automatisch an diesen Service übergeben.
        //
        // Program.cs:
        //    builder.Services.AddDbContext<PizzaContext>(...);
        //    builder.Services.AddScoped<PizzaService>();
        //
        // Dadurch steht hier ein fertig konfigurierter PizzaContext bereit.
        // ---------------------------------------------------------------------
        public PizzaService(PizzaContext context)
        {
            _context = context;
        }

        // ---------------------------------------------------------------------
        // METHODE: GetPizzas
        //
        // Gibt eine Liste aller Pizzen zurück, die in der Datenbank gespeichert sind.
        //
        // Ablauf:
        //  1. Prüfen, ob die Tabelle Pizzas existiert
        //  2. Falls ja: .ToList() holt alle Datensätze aus der DB
        //  3. Falls nein: leere Liste zurückgeben
        //
        // .ToList() löst bei EF Core eine "SELECT * FROM Pizzas" Query aus.
        // ---------------------------------------------------------------------
        public IList<Pizza> GetPizzas()
        {
            if (_context.Pizzas != null)
            {
                return _context.Pizzas.ToList();  // Alle Pizzen laden
            }
            return new List<Pizza>();             // Falls Tabelle fehlt
        }

        // ---------------------------------------------------------------------
        // METHODE: AddPizza
        //
        // Fügt eine neue Pizza in die Datenbank ein.
        //
        // Ablauf:
        //  1. Pizza zur Tabelle hinzufügen
        //  2. Änderungen dauerhaft speichern (SaveChanges)
        //
        // SaveChanges() führt ein INSERT SQL aus.
        // ---------------------------------------------------------------------
        public void AddPizza(Pizza pizza)
        {
            if (_context.Pizzas != null)
            {
                _context.Pizzas.Add(pizza);   // Neue Pizza in den DB-Kontext
                _context.SaveChanges();       // INSERT in DB ausführen
            }
        }

        // ---------------------------------------------------------------------
        // METHODE: DeletePizza
        //
        // Löscht eine Pizza anhand ihrer Id aus der Datenbank.
        //
        // Ablauf:
        //  1. Pizza anhand der Id suchen   (Find)
        //  2. Falls gefunden: löschen (Remove)
        //  3. Änderungen in DB speichern (SaveChanges)
        //
        // Find(id) sucht zuerst im lokalen Speicher (Cache), dann in der DB.
        // Remove erzeugt einen DELETE SQL Befehl.
        // ---------------------------------------------------------------------
        public void DeletePizza(int id)
        {
            if (_context.Pizzas != null)
            {
                var pizza = _context.Pizzas.Find(id);  // Pizza suchen anhand Id
                if (pizza != null)
                {
                    _context.Pizzas.Remove(pizza);     // Zum Löschen markieren
                    _context.SaveChanges();            // DELETE in DB ausführen
                }
            }
        }
    }
}
