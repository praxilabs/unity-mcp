using System;

public static class GetExpNameFromURL
{
    public static string GetExpFromURL(string url)
    {
        Uri uri = new Uri(url);
        return uri.Query.TrimStart('?'); // Remove the '?' at the start
    }
}
