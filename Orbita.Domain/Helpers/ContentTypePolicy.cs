namespace Orbita.Domain.Helpers;

internal static class ContentTypePolicy
{
    private static readonly HashSet<string> Allowed = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

    public static bool IsAllowed(string contentType) => Allowed.Contains(contentType);
}
