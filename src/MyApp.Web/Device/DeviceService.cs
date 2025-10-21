using System.Text.Json;

public class DeviceService
{
    private readonly TTLockClient _ttLock;

    public DeviceService(TTLockClient ttLock)
    {
        _ttLock = ttLock;
    }

    public async Task AuthAsync()
    {
        await _ttLock.AuthAsync();
    }

    // ==============================
    // Get Lock Devices
    // ==============================
    public async Task<List<DeviceDto>> GetDevicesAsync(int pageNo = 1, int pageSize = 200)
    {
        var doc = await _ttLock.GetLockListAsync(pageNo, pageSize);
        var list = new List<DeviceDto>();

        if (doc.RootElement.TryGetProperty("list", out var devices))
        {
            foreach (var item in devices.EnumerateArray())
            {
                list.Add(new DeviceDto
                {
                    LockId = item.GetPropertyOrDefault<long>("lockId", 0L),
                    LockAlias = item.GetPropertyOrDefault<string>("lockAlias", string.Empty),
                    ElectricQuantity = item.GetPropertyOrDefault<int>("electricQuantity", 0),
                    NoKeyPwd = item.GetPropertyOrDefault<int>("noKeyPwd", 0),
                    LockMac = item.GetPropertyOrDefault<string>("lockMac", string.Empty),
                    Date = item.GetDateTimeFromUnix("date")
                });
            }
        }

        return list;
    }

    // ==============================
    // Rename Lock (Update Device)
    // ==============================
    public async Task<bool> UpdateDeviceAsync(long lockId, string newAlias)
    {
        try
        {
            var response = await _ttLock.RenameLockAsync(lockId, newAlias);
            return response.ErrCode == 0;
        }
        catch
        {
            return false;
        }
    }

    // ==============================
    // Lock / Unlock
    // ==============================
    public async Task LockDeviceAsync(long lockId) => await _ttLock.LockAsync(lockId);
    public async Task UnlockDeviceAsync(long lockId) => await _ttLock.UnlockAsync(lockId);

    // ==============================
    // IC Cards
    // ==============================
    public async Task<bool> AddCardAsync(long lockId, string cardName, string cardNumber, int validDays)
    {
        try
        {
            var response = await _ttLock.AddCardAsync(lockId, cardName, cardNumber, validDays);
            return response.ErrCode == 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task DeleteCardAsync(long lockId, long cardId)
        => await _ttLock.DeleteCardAsync(lockId, cardId);

    public async Task<List<ICCardDto>> GetCardsAsync(long lockId)
    {
        var doc = await _ttLock.GetCardsAsync(lockId);
        var list = new List<ICCardDto>();

        if (doc.RootElement.TryGetProperty("list", out var cards))
        {
            foreach (var item in cards.EnumerateArray())
            {
                list.Add(new ICCardDto
                {
                    CardId = item.GetPropertyOrDefault("cardId", 0L),
                    LockId = item.GetPropertyOrDefault("lockId", 0L),
                    CardNumber = item.GetPropertyOrDefault("cardNumber", string.Empty),
                    CardName = item.GetPropertyOrDefault("cardName", string.Empty),
                    CardType = item.GetPropertyOrDefault("cardType", 0),
                    SenderUsername = item.GetPropertyOrDefault("senderUsername", string.Empty),
                    StartDate = item.GetDateTimeFromUnix("startDate"),
                    EndDate = item.GetDateTimeFromUnix("endDate"),
                    CreateDate = item.GetDateTimeFromUnix("createDate")
                });
            }
        }

        return list;
    }

    // ==============================
    // Fingerprints
    // ==============================
    public async Task<List<FingerprintDto>> GetFingerprintsAsync(long lockId)
    {
        var doc = await _ttLock.GetFingerprintsAsync(lockId);
        var list = new List<FingerprintDto>();

        if (doc.RootElement.TryGetProperty("list", out var fps))
        {
            foreach (var item in fps.EnumerateArray())
            {
                list.Add(new FingerprintDto
                {
                    FingerprintId = item.GetPropertyOrDefault("fingerprintId", 0L),
                    LockId = item.GetPropertyOrDefault("lockId", 0L),
                    FingerprintName = item.GetPropertyOrDefault("fingerprintName", string.Empty),
                    FingerprintNumber = item.GetPropertyOrDefault("fingerprintNumber", string.Empty),
                    FingerprintType = item.GetPropertyOrDefault("fingerprintType", 0),
                    StartDate = item.GetDateTimeFromUnix("startDate"),
                    EndDate = item.GetDateTimeFromUnix("endDate"),
                    CreateDate = item.GetDateTimeFromUnix("createDate")
                });
            }
        }

        return list;
    }

    public async Task DeleteFingerprintAsync(long lockId, long fpId)
        => await _ttLock.DeleteFingerprintAsync(lockId, fpId);

    // ==============================
    // eKeys, Passcodes, Records
    // ==============================
    public async Task<List<EKeyDto>> GetEKeysAsync(long lockId, int pageNo = 1, int pageSize = 20)
    {
        var doc = await _ttLock.GetEKeysAsync(lockId, pageNo, pageSize);
        var list = new List<EKeyDto>();

        if (doc.RootElement.TryGetProperty("list", out var ekeys))
        {
            foreach (var item in ekeys.EnumerateArray())
            {
                list.Add(new EKeyDto
                {
                    EKeyId = item.GetPropertyOrDefault("keyId", 0L),
                    LockId = item.GetPropertyOrDefault("lockId", 0L),
                    Username = item.GetPropertyOrDefault("username", string.Empty),
                    KeyStatus = item.GetPropertyOrDefault("keyStatus", 0),
                    StartDate = item.GetDateTimeFromUnix("startDate"),
                    EndDate = item.GetDateTimeFromUnix("endDate"),
                    CreateDate = item.GetDateTimeFromUnix("createDate")
                });
            }
        }

        return list;
    }

    public async Task<TTLockResponse> RenameLockAsync(long lockId, string newAlias)
    {
        return await _ttLock.RenameLockAsync(lockId, newAlias);
    }

    public async Task<List<PasscodeDto>> GetPasscodesAsync(long lockId, int pageNo = 1, int pageSize = 20)
    {
        var doc = await _ttLock.GetPasscodesAsync(lockId, pageNo, pageSize);
        var list = new List<PasscodeDto>();

        if (doc.RootElement.TryGetProperty("list", out var passcodes))
        {
            foreach (var item in passcodes.EnumerateArray())
            {
                list.Add(new PasscodeDto
                {
                    PasscodeId = item.GetPropertyOrDefault("keyboardPwdId", 0L),
                    LockId = item.GetPropertyOrDefault("lockId", 0L),
                    KeyboardPwd = item.GetPropertyOrDefault("keyboardPwd", string.Empty),
                    PwdName = item.GetPropertyOrDefault("pwdName", string.Empty),
                    SenderUsername = item.GetPropertyOrDefault("senderUsername", string.Empty),
                    StartDate = item.GetDateTimeFromUnix("startDate"),
                    EndDate = item.GetDateTimeFromUnix("endDate"),
                    CreateDate = item.GetDateTimeFromUnix("createDate")
                });
            }
        }

        return list;
    }

    public async Task<List<RecordDto>> GetRecordsAsync(long lockId, int pageNo = 1, int pageSize = 50)
    {
        var doc = await _ttLock.GetRecordsAsync(lockId, pageNo, pageSize);
        var list = new List<RecordDto>();

        if (doc.RootElement.TryGetProperty("list", out var records))
        {
            foreach (var item in records.EnumerateArray())
            {
                list.Add(new RecordDto
                {
                    LockId = item.GetPropertyOrDefault("lockId", 0L),
                    RecordType = item.GetPropertyOrDefault("recordType", string.Empty),
                    Username = item.GetPropertyOrDefault("username", string.Empty),
                    ClientName = item.GetPropertyOrDefault("clientName", string.Empty),
                    LockDate = item.GetDateTimeFromUnix("lockDate"),
                    ServerDate = item.GetDateTimeFromUnix("serverDate")
                });
            }
        }

        return list;
    }
}
