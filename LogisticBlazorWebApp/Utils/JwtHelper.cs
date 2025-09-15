using System.Text.Json;

public static class JwtHelper
{
    public static List<string> GetRoles(string jwt)
    {
        var parts = jwt.Split('.');
        if (parts.Length < 2) return new();

        string payload = parts[1]
            .Replace('-', '+').Replace('_', '/');
        switch (payload.Length % 4)
        {
            case 2: payload += "=="; break;
            case 3: payload += "="; break;
        }

        try
        {
            var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(payload));
            using var doc = JsonDocument.Parse(json);
            var roles = new List<string>();

            // Role có thể ở claim "role" (string hoặc array) hoặc "roles"
            if (doc.RootElement.TryGetProperty("role", out var roleEl))
            {
                if (roleEl.ValueKind == JsonValueKind.Array)
                    roles.AddRange(roleEl.EnumerateArray().Select(x => x.GetString()!).Where(x => !string.IsNullOrWhiteSpace(x)));
                else if (roleEl.ValueKind == JsonValueKind.String)
                    roles.Add(roleEl.GetString()!);
            }
            if (doc.RootElement.TryGetProperty("roles", out var rolesEl) && rolesEl.ValueKind == JsonValueKind.Array)
            {
                roles.AddRange(rolesEl.EnumerateArray().Select(x => x.GetString()!).Where(x => !string.IsNullOrWhiteSpace(x)));
            }

            // Chuẩn hóa trùng/hoa-thường
            return roles.Select(r => r.Trim()).Where(r => !string.IsNullOrWhiteSpace(r))
                        .Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }
        catch { return new(); }
    }
    public static string? GetClaim(string jwt, string claim)
    {
        if (string.IsNullOrWhiteSpace(jwt)) return null;
        var parts = jwt.Split('.');
        if (parts.Length < 2) return null;

        string payload = parts[1].Replace('-', '+').Replace('_', '/');
        switch (payload.Length % 4) { case 2: payload += "=="; break; case 3: payload += "="; break; }

        var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(payload));
        using var doc = System.Text.Json.JsonDocument.Parse(json);
        return doc.RootElement.TryGetProperty(claim, out var el) && el.ValueKind == System.Text.Json.JsonValueKind.String
            ? el.GetString()
            : null;
    }
}
