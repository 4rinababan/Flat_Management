using System.Text.Json;

public static class JsonElementExtensions
{
    public static T GetPropertyOrDefault<T>(this JsonElement element, string name, T defaultValue)
{
    if (!element.TryGetProperty(name, out var prop))
        return defaultValue;

    try
    {
        object? value = defaultValue;

        switch (Type.GetTypeCode(typeof(T)))
        {
            case TypeCode.String:
                value = prop.ValueKind switch
                {
                    JsonValueKind.String => prop.GetString(),
                    JsonValueKind.Number => prop.GetRawText(), // raw text number tetap string
                    JsonValueKind.True => "true",
                    JsonValueKind.False => "false",
                    _ => defaultValue
                };
                break;

            case TypeCode.Int32:
                if (prop.ValueKind == JsonValueKind.Number && prop.TryGetInt32(out var i))
                    value = i;
                else if (prop.ValueKind == JsonValueKind.String && int.TryParse(prop.GetString(), out i))
                    value = i;
                break;

            case TypeCode.Int64:
                if (prop.ValueKind == JsonValueKind.Number && prop.TryGetInt64(out var l))
                    value = l;
                else if (prop.ValueKind == JsonValueKind.String && long.TryParse(prop.GetString(), out l))
                    value = l;
                break;

            case TypeCode.Boolean:
                if (prop.ValueKind == JsonValueKind.True || prop.ValueKind == JsonValueKind.False)
                    value = prop.GetBoolean();
                else if (prop.ValueKind == JsonValueKind.String && bool.TryParse(prop.GetString(), out var b))
                    value = b;
                break;
        }

        return (T)value!;
    }
    catch
    {
        return defaultValue;
    }
}


    public static DateTime? GetDateTimeFromUnix(this JsonElement element, string name)
    {
        if (element.TryGetProperty(name, out var prop))
        {
            long unixMs = 0;

            if (prop.ValueKind == JsonValueKind.Number)
                unixMs = prop.GetInt64();
            else if (prop.ValueKind == JsonValueKind.String && long.TryParse(prop.GetString(), out var parsed))
                unixMs = parsed;

            if (unixMs > 0)
            {
                return DateTimeOffset
                    .FromUnixTimeMilliseconds(unixMs)
                    .ToLocalTime() // <-- sesuai timezone lokal
                    .DateTime;
            }
        }

        return null;
    }


}
