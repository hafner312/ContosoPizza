using Microsoft.AspNetCore.Http;

namespace ContosoPizza.Services;

// Gibt jedem Besucher ueber die bestehende Session (siehe CartService) eine
// eigene, anonyme Kennung, damit sich gleichzeitige Besucher der Live-Demo
// nicht gegenseitig Speisekarten-Eintraege oder Bestellungen anzeigen/
// veraendern koennen.
public class VisitorService
{
    private const string SessionKey = "ownerId";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public VisitorService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ISession Session => _httpContextAccessor.HttpContext!.Session;

    public string GetOwnerId()
    {
        var ownerId = Session.GetString(SessionKey);
        if (string.IsNullOrEmpty(ownerId))
        {
            ownerId = Guid.NewGuid().ToString();
            Session.SetString(SessionKey, ownerId);
        }
        return ownerId;
    }
}
