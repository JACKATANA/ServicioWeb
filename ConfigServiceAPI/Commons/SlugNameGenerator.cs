using System.Text.RegularExpressions;

namespace ConfigServiceAPI.Commons
{
    public static class SlugNameGenerator
    {
        public static string GenerateSlug(string name)
        {
            name = name.ToLowerInvariant();
            name = Regex.Replace(name, @"[^a-z0-9\s-]", "");
            name = Regex.Replace(name, @"\s+", "-");
            name = Regex.Replace(name, @"-+", "-");
            return name.Trim('-');  
        }

    }
}
