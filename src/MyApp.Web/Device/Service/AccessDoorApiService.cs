using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using MyApp.Web.Device.Model;
using Microsoft.Extensions.Configuration;

namespace MyApp.Web.Device.Service
{
    public class AccessDoorApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUri;

        public AccessDoorApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUri = configuration["ApiSettings:BaseUrl"] ?? "http://192.168.91.195:8000";
        }

        private record CardIdPayload(int id);

        // ============================
        // DEVICE LIST / DETAIL
        // ============================
        public async Task<List<DeviceSummaryDto>> GetDevicesAsync()
        {
            try
            {
                Console.WriteLine($"🌐 Fetching devices from: {_baseUri}/devices");
                var response = await _httpClient.GetFromJsonAsync<DeviceListResponse>($"{_baseUri}/devices");
                Console.WriteLine(response?.Devices ?? new List<DeviceSummaryDto>());
                //return response?.Devices ?? new List<DeviceSummaryDto>();
                
                if (response == null)
                    Console.WriteLine("⚠️ Response is null!");
                else
                    Console.WriteLine($"✅ Loaded {response.Count} devices");
                
                var filtered = response.Devices
                    .Where(d => !(
                        (d.Alias?.Contains("Device", StringComparison.OrdinalIgnoreCase) ?? false)))
                    .ToList();

                Console.WriteLine($"✅ Filtered devices: {filtered.Count} remaining");

                return filtered;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error GetDevicesAsync: {ex.Message}");
                return new();
            }
        }


        public async Task<DeviceDetailDto?> GetDeviceDetailAsync(int deviceId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<DeviceDetailDto>($"{_baseUri}/devices/{deviceId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error GetDeviceDetailAsync({deviceId}): {ex.Message}");
                return null;
            }
        }

        // ============================
        // CARD MANAGEMENT
        // ============================
        public async Task<bool> AddCardAsync(int deviceId, int cardId)
        {
            try
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(new CardIdPayload(cardId)),
                    Encoding.UTF8,
                    "application/json");

                var res = await _httpClient.PostAsync($"{_baseUri}/device/{deviceId}/add", content);
                return res.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error AddCardAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RemoveCardAsync(int deviceId, int cardId)
        {
            try
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(new CardIdPayload(cardId)),
                    Encoding.UTF8,
                    "application/json");

                var res = await _httpClient.PostAsync($"{_baseUri}/device/{deviceId}/remove", content);
                return res.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error RemoveCardAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RefreshCardsAsync(int deviceId)
        {
            try
            {
                var res = await _httpClient.GetAsync($"{_baseUri}/device/{deviceId}/refresh_cards");
                return res.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error RefreshCardsAsync: {ex.Message}");
                return false;
            }
        }

        // ============================
        // DOOR CONTROLS
        // ============================
        public async Task<bool> OpenDoorAsync(int deviceId)
        {
            try
            {
                var res = await _httpClient.GetAsync($"{_baseUri}/device/{deviceId}/open");
                return res.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<bool> BypassDoorAsync(int deviceId, string action)
        {
            try
            {
                var res = await _httpClient.GetAsync($"{_baseUri}/device/{deviceId}/bypass/{action}");
                return res.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<bool> BypassLockAsync(int deviceId, string action)
        {
            try
            {
                var res = await _httpClient.GetAsync($"{_baseUri}/device/{deviceId}/bypassLock/{action}");
                return res.IsSuccessStatusCode;
            }
            catch { return false; }
        }
    }
}
