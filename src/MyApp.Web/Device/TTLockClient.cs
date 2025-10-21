using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

public class TTLockClient
{
    private readonly HttpClient _http;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _username;
    private readonly string _password;
    private string _accessToken = "";

    private const string BaseUrl = "https://api.sciener.com/v3";

    public TTLockClient(string clientId, string clientSecret, string username, string password)
    {
        _clientId = clientId;
        _clientSecret = clientSecret;
        _username = username;
        _password = password;
        _http = new HttpClient();
    }

    private static string Md5Lower(string input)
    {
        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
        return string.Concat(hash.Select(b => b.ToString("x2")));
    }

    public async Task<string> AuthAsync()
    {
        var pwdMd5 = Md5Lower(_password);
        var data = new Dictionary<string, string>
        {
            {"grant_type", "password"},
            {"client_id", _clientId},
            {"client_secret", _clientSecret},
            {"username", _username},
            {"password", pwdMd5}
        };

        var doc = await PostAsync("https://api.sciener.com/oauth2/token", data);
        _accessToken = doc.RootElement.GetProperty("access_token").GetString()!;
        return _accessToken;
    }

    // -------------------- GET Helper --------------------
    private async Task<JsonDocument> GetV3Async(string path, Dictionary<string, string> parameters)
    {
        parameters ??= new();
        if (!parameters.ContainsKey("clientId")) parameters["clientId"] = _clientId;
        if (!parameters.ContainsKey("accessToken")) parameters["accessToken"] = _accessToken;
        if (!parameters.ContainsKey("date")) parameters["date"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

        var query = string.Join("&", parameters.Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"));
        var url = $"https://euapi.ttlock.com/v3/{path}?{query}";

        using var resp = await _http.GetAsync(url);
        var content = await resp.Content.ReadAsStringAsync();

        if (!resp.IsSuccessStatusCode)
            throw new Exception($"TTLock API error: {resp.StatusCode} - {content}");

        return JsonDocument.Parse(content);
    }

    // -------------------- POST Helper --------------------
    private async Task<JsonDocument> PostAsync(string url, Dictionary<string, string> data)
    {
        using var content = new FormUrlEncodedContent(data);
        var response = await _http.PostAsync(url, content);
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"HTTP {response.StatusCode}: {body}");

        return JsonDocument.Parse(body);
    }

    private async Task<JsonDocument> PostV3Async(string path, Dictionary<string, string>? extra = null)
    {
        if (string.IsNullOrEmpty(_accessToken))
            throw new Exception("Belum ada access token! Jalankan AuthAsync dulu.");

        var data = new Dictionary<string, string>
        {
            {"clientId", _clientId},
            {"accessToken", _accessToken},
            {"date", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()}
        };

        if (extra != null)
        {
            foreach (var kv in extra)
                data[kv.Key] = kv.Value;
        }

        return await PostAsync($"{BaseUrl}/{path}", data);
    }

    // -------------------- API: Locks, Cards, Fingerprints --------------------
    // public async Task<JsonDocument> AddCardAsync(long lockId, string cardName, string cardNumber, int validDays)
    // {
    //     var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    //     var end = DateTimeOffset.UtcNow.AddDays(validDays).ToUnixTimeMilliseconds();

    //     // Gunakan endpoint untuk cloud/gateway method
    //     string endpoint = "identityCard/addForReversedCardNumber";

    //     return await PostV3Async(endpoint, new()
    //     {
    //         {"lockId", lockId.ToString()},
    //         {"cardNumber", cardNumber},
    //         {"cardName", cardName},
    //         {"startDate", now.ToString()},
    //         {"endDate", end.ToString()},
    //         {"addType", "2"},
    //         {"date", now.ToString()}
    //     });
    // }

    public async Task<JsonDocument> GetEKeysAsync(long lockId, int pageNo = 1, int pageSize = 100)
        => await GetV3Async("key/list", new()
        {
            ["lockId"] = lockId.ToString(),
            ["pageNo"] = pageNo.ToString(),
            ["pageSize"] = pageSize.ToString(),
            ["orderBy"] = "1"
        });

    public async Task<JsonDocument> GetPasscodesAsync(long lockId, int pageNo = 1, int pageSize = 100)
        => await GetV3Async("keyboardPwd/list", new()
        {
            ["lockId"] = lockId.ToString(),
            ["pageNo"] = pageNo.ToString(),
            ["pageSize"] = pageSize.ToString(),
            ["orderBy"] = "1"
        });

    public async Task<JsonDocument> GetRecordsAsync(long lockId, int pageNo = 1, int pageSize = 100)
        => await GetV3Async("lockRecord/list", new()
        {
            ["lockId"] = lockId.ToString(),
            ["pageNo"] = pageNo.ToString(),
            ["pageSize"] = pageSize.ToString(),
            ["orderBy"] = "1"
        });

    public async Task<JsonDocument> GetGatewayListAsync(int pageNo = 1, int pageSize = 16)
        => await PostV3Async("gateway/list", new()
        {
            { "pageNo", pageNo.ToString() },
            { "pageSize", pageSize.ToString() }
        });

    public async Task<JsonDocument> GetLockListAsync(int pageNo = 1, int pageSize = 16)
        => await GetV3Async("lock/list", new()
        {
            { "pageNo", pageNo.ToString() },
            { "pageSize", pageSize.ToString() }
        });

    public async Task<JsonDocument> LockAsync(long lockId)
        => await PostV3Async("lock/lock", new() { { "lockId", lockId.ToString() } });

    public async Task<JsonDocument> UnlockAsync(long lockId)
        => await PostV3Async("lock/unlock", new() { { "lockId", lockId.ToString() } });

    public async Task<TTLockResponse> RenameLockAsync(long lockId, string newAlias)
    {
        if (string.IsNullOrEmpty(_accessToken))
            throw new Exception("Belum ada access token! Jalankan AuthAsync dulu.");

        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        var json = await PostV3Async("lock/rename", new()
        {
            { "lockId", lockId.ToString() },
            { "lockAlias", newAlias },
            { "date", timestamp.ToString() }
        });

        var root = json.RootElement;

        return new TTLockResponse
        {
            ErrCode = root.GetProperty("errcode").GetInt32(),
            ErrMsg = root.GetProperty("errmsg").GetString()
        };
    }


    public async Task<JsonDocument> GetCardsAsync(long lockId)
        => await PostV3Async("identityCard/list", new()
        {
            { "lockId", lockId.ToString() },
            { "pageNo", "1" },
            { "pageSize", "20" }
        });

    public async Task<TTLockResponse> AddCardAsync(long lockId, string cardName, string cardNumber, int validDays)
{
    var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    var end = DateTimeOffset.UtcNow.AddDays(validDays).ToUnixTimeMilliseconds();

    var json = await PostV3Async("identityCard/addForReversedCardNumber", new()
    {
        {"lockId", lockId.ToString()},
        {"cardNumber", cardNumber},
        {"cardName", cardName},
        {"startDate", now.ToString()},
        {"endDate", end.ToString()},
        {"addType", "2"},
        {"date", now.ToString()}
    });

    var root = json.RootElement;

    return new TTLockResponse
    {
        ErrCode = root.GetProperty("errcode").GetInt32(),
        ErrMsg = root.GetProperty("errmsg").GetString()
    };
}

    public async Task<JsonDocument> DeleteCardAsync(long lockId, long cardId)
        => await PostV3Async("identityCard/delete", new()
        {
            { "lockId", lockId.ToString() },
            { "cardId", cardId.ToString() },
            { "deleteType", "2" }
        });

    public async Task<JsonDocument> GetFingerprintsAsync(long lockId)
        => await PostV3Async("fingerprint/list", new() { { "lockId", lockId.ToString() } });

    public async Task<JsonDocument> AddFingerprintAsync(long lockId, string fingerprintNumber, int fingerprintType,
        string fingerprintName, long startDate, long endDate)
        => await PostV3Async("fingerprint/add", new()
        {
            {"lockId", lockId.ToString()},
            {"fingerprintNumber", fingerprintNumber},
            {"fingerprintType", fingerprintType.ToString()},
            {"fingerprintName", fingerprintName},
            {"startDate", startDate.ToString()},
            {"endDate", endDate.ToString()},
            {"date", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()}
        });

    public async Task<JsonDocument> DeleteFingerprintAsync(long lockId, long fingerprintId)
    {
        return await PostV3Async("fingerprint/delete", new()
        {
            { "lockId", lockId.ToString() },
            { "fingerprintId", fingerprintId.ToString() },
            { "deleteType", "2" }
        });
    }


    public async Task<JsonDocument> QueryLockSettingAsync(long lockId, int type)
        => await PostV3Async("lock/querySetting", new()
        {
            {"lockId", lockId.ToString()},
            {"type", type.ToString()}
        });

    public static void Print(JsonDocument doc)
        => Console.WriteLine(JsonSerializer.Serialize(doc.RootElement, new JsonSerializerOptions { WriteIndented = true }));
}
