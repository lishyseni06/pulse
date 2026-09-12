namespace Pulse.Services;

/// <summary>Kthen klasën CSS të badge-it sipas emrit të kategorisë.</summary>
public static class CategoryBadge
{
    public static string Class(string? name) => name switch
    {
        "Bota" => "badge-bota",
        "Rajoni" => "badge-rajoni",
        "Vendi" => "badge-vendi",
        "Sport" => "badge-sport",
        "ShowBiz" => "badge-showbiz",
        _ => "badge-default"
    };
}
