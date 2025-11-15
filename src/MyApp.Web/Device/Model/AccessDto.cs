using System.Text.Json.Serialization;

namespace MyApp.Web.Device.Model
{
    // ======== LAST COMMAND ========
    public class DeviceLastCommand
    {
        [JsonPropertyName("cmd")]
        public string? Cmd { get; set; }

        [JsonPropertyName("action")]
        public string? Action { get; set; }
    }

    // ======== SETTINGS ========
    public class DeviceSettings
    {
        [JsonPropertyName("alias")]
        public string? Alias { get; set; }

        [JsonPropertyName("bypass")]
        public bool? Bypass { get; set; }

        [JsonPropertyName("relay_ttl")]
        public int? RelayTtl { get; set; }

        [JsonPropertyName("bypass_lock")]
        public bool? BypassLock { get; set; }
    }

    // ======== DETAIL DEVICE ========
    public class DeviceDetailDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("alias")]
        public string? Alias { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("ip")]
        public string? Ip { get; set; }

        [JsonPropertyName("last_seen")]
        public DateTime? LastSeen { get; set; }

        [JsonPropertyName("last_cmd")]
        public DeviceLastCommand? LastCmd { get; set; }

        [JsonPropertyName("cards")]
        public List<int>? Cards { get; set; }

        [JsonPropertyName("settings")]
        public DeviceSettings? Settings { get; set; }

        // Helper property buat styling status di UI
        public string StatusClass =>
            Status?.ToLower() switch
            {
                "online" => "text-green-600",
                "offline" => "text-red-600",
                _ => "text-yellow-600"
            };
    }

    // ======== SUMMARY DEVICE ========
    public class DeviceSummaryDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("alias")]
        public string? Alias { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("ip")]
        public string? Ip { get; set; }

        [JsonPropertyName("last_seen")]
        public DateTime? LastSeen { get; set; }

        [JsonPropertyName("last_cmd")]
        public DeviceLastCommand? LastCmd { get; set; }

        [JsonPropertyName("cards_count")]
        public int CardsCount { get; set; }

        [JsonPropertyName("settings")]
        public DeviceSettings? Settings { get; set; }

        public string StatusClass =>
            Status?.ToLower() switch
            {
                "online" => "text-green-600",
                "offline" => "text-red-600",
                _ => "text-yellow-600"
            };

        public bool Bypass => Settings?.Bypass.GetValueOrDefault() ?? false;
        public bool BypassLock => Settings?.BypassLock.GetValueOrDefault() ?? false;
    }

    // ======== LIST RESPONSE ========
    public class DeviceListResponse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("devices")]
        public List<DeviceSummaryDto>? Devices { get; set; }
    }
}
