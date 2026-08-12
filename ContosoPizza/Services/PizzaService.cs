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
    // Alle Operationen sind auf den aktuellen Besucher (siehe VisitorService)
    // skopiert: jeder Besucher der Live-Demo bekommt beim ersten Aufruf eine
    // eigene Kopie der Speisekarte und sieht/veraendert nur diese - so kann
    // niemand die Karte eines anderen gleichzeitigen Besuchers leerraeumen.
    //
    // Warum ein Service?
    //  - PageModel soll KEINE DB-Zugriffe direkt machen
    //  - Wir haben eine saubere Layer-Trennung
    //  - Wiederverwendbarkeit: andere Seiten können denselben Service nutzen
    // -------------------------------------------------------------------------
    public class PizzaService
    {
        private readonly PizzaContext _context;
        private readonly VisitorService _visitor;

        public PizzaService(PizzaContext context, VisitorService visitor)
        {
            _context = context;
            _visitor = visitor;
        }

        /// <summary>
        /// Gibt alle Pizzen des aktuellen Besuchers zurück. Legt beim allerersten
        /// Aufruf automatisch das Standardsortiment fuer diesen Besucher an.
        /// </summary>
        public IList<Pizza> GetPizzas()
        {
            if (_context.Pizzas == null) return new List<Pizza>();

            var ownerId = _visitor.GetOwnerId();
            SeedIfNew(ownerId);
            return _context.Pizzas.Where(p => p.OwnerId == ownerId).ToList();
        }

        /// <summary>
        /// Fügt eine neue Pizza in die Speisekarte des aktuellen Besuchers ein.
        /// </summary>
        public void AddPizza(Pizza pizza)
        {
            if (_context.Pizzas != null)
            {
                pizza.OwnerId = _visitor.GetOwnerId();
                _context.Pizzas.Add(pizza);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Löscht eine Pizza aus der Speisekarte des aktuellen Besuchers, sofern
        /// sie ihm tatsächlich gehört.
        /// </summary>
        public void DeletePizza(int id)
        {
            if (_context.Pizzas != null)
            {
                var ownerId = _visitor.GetOwnerId();
                var pizza = _context.Pizzas.Find(id);
                if (pizza != null && pizza.OwnerId == ownerId)
                {
                    _context.Pizzas.Remove(pizza);
                    _context.SaveChanges();
                }
            }
        }

        // Befüllt die Speisekarte eines neuen Besuchers einmalig mit einem
        // realistischen Sortiment, damit die Demo sofort etwas zu bestellen
        // anbietet statt einer leeren Liste.
        private void SeedIfNew(string ownerId)
        {
            if (_context.Pizzas!.Any(p => p.OwnerId == ownerId)) return;

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
                pizzas.Add(new Pizza { Name = item.Name, Description = item.Description, Size = PizzaSize.Small, IsGlutenFree = item.GlutenFree, Price = item.Small, OwnerId = ownerId });
                pizzas.Add(new Pizza { Name = item.Name, Description = item.Description, Size = PizzaSize.Medium, IsGlutenFree = item.GlutenFree, Price = item.Medium, OwnerId = ownerId });
                pizzas.Add(new Pizza { Name = item.Name, Description = item.Description, Size = PizzaSize.Large, IsGlutenFree = item.GlutenFree, Price = item.Large, OwnerId = ownerId });
            }

            _context.Pizzas!.AddRange(pizzas);
            _context.SaveChanges();
        }
    }
}
