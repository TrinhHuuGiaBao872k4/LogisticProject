using System.Text.Json;

public static class JwtHelper
{
    /// <summary>
    /// Trả về true nếu token hết hạn (exp) tính theo UTC. 
    /// leewaySeconds cho phép lệch đồng hồ (mặc định 0).
    /// </summary>
    public static bool IsExpired(string jwt, int leewaySeconds = 0)
    {
        if (!TryReadPayload(jwt, out var root)) return true;

        if (root.TryGetProperty("exp", out var expEl) &&
            expEl.ValueKind is JsonValueKind.Number &&
            expEl.TryGetInt64(out var exp))
        {
            var expUtc = DateTimeOffset.FromUnixTimeSeconds(exp);
            // Hết hạn nếu "bây giờ" >= exp + leeway
            return DateTimeOffset.UtcNow >= expUtc.AddSeconds(leewaySeconds);
        }
        // Không có exp -> coi như hết hạn
        return true;
    }

    /// <summary>
    /// Trích roles từ các biến thể: "role" (string/array), "roles" (string/array),
    /// và claim chuẩn MS: "http://schemas.microsoft.com/ws/2008/06/identity/claims/role".
    /// </summary>
    public static List<string> GetRoles(string jwt)
    {
        if (!TryReadPayload(jwt, out var root)) return new();

        var roles = new List<string>();

        // role: string hoặc array
        if (root.TryGetProperty("role", out var roleEl))
        {
            if (roleEl.ValueKind == JsonValueKind.Array)
                roles.AddRange(roleEl.EnumerateArray()
                                     .Select(x => x.GetString())
                                     .Where(s => !string.IsNullOrWhiteSpace(s))!);
            else if (roleEl.ValueKind == JsonValueKind.String)
                roles.Add(roleEl.GetString()!);
        }

        // roles: string hoặc array
        if (root.TryGetProperty("roles", out var rolesEl))
        {
            if (rolesEl.ValueKind == JsonValueKind.Array)
                roles.AddRange(rolesEl.EnumerateArray()
                                      .Select(x => x.GetString())
                                      .Where(s => !string.IsNullOrWhiteSpace(s))!);
            else if (rolesEl.ValueKind == JsonValueKind.String)
                roles.Add(rolesEl.GetString()!);
        }

        // Claim chuẩn MS
        const string msRole = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
        if (root.TryGetProperty(msRole, out var msRoleEl) && msRoleEl.ValueKind == JsonValueKind.String)
            roles.Add(msRoleEl.GetString()!);

        // Chuẩn hoá
        return roles.Select(r => r.Trim())
                    .Where(r => !string.IsNullOrWhiteSpace(r))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
    }

    /// <summary>
    /// Lấy claim string bất kỳ.
    /// </summary>
    public static string? GetClaim(string jwt, string claim)
    {
        if (!TryReadPayload(jwt, out var root)) return null;
        if (root.TryGetProperty(claim, out var el) && el.ValueKind == JsonValueKind.String)
            return el.GetString();
        return null;
    }

    // ===== Helpers =====

    private static bool TryReadPayload(string jwt, out JsonElement root)
    {
        root = default;

        if (string.IsNullOrWhiteSpace(jwt)) return false;
        var parts = jwt.Split('.');
        if (parts.Length < 2) return false;

        // base64url -> base64
        var payload = parts[1].Replace('-', '+').Replace('_', '/');
        switch (payload.Length % 4)
        {
            case 2: payload += "=="; break;
            case 3: payload += "="; break;
        }

        try
        {
            var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(payload));
            using var doc = JsonDocument.Parse(json);
            root = doc.RootElement.Clone(); // Clone để dùng sau khi doc Dispose
            return true;
        }
        catch
        {
            return false;
        }
    }
}
