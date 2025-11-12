
public class DeviceDto
{
    public long LockId { get; set; }
    public string LockAlias { get; set; } = "";
    public string LockMac { get; set; } = "";
    public int ElectricQuantity { get; set; } = 0;
    public string LockVersion { get; set; } = "";
    public string ModelNum { get; set; } = "";
    public string HardwareRevision { get; set; } = "";
    public string FirmwareRevision { get; set; } = "";
     public int NoKeyPwd { get; set; } 
    public DateTime? Date { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    // Di DeviceDto.cs
public string? ErrMsg { get; set; } // Opsi 1: Jadikan nullable
// ATAU
// public string ErrMsg { get; set; } = string.Empty; // Opsi 2: Inisialisasi
}


public class CardDto
{
    public long CardId { get; set; }
    public string CardName { get; set; } = "";
    public string CardNumber { get; set; } = "";
    public DateTime Date { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public int Status { get; set; }
    public long LockId { get; set; }
    public string NoKeyPwd { get; set; } = "";
}


public class FingerprintDto
{
    public long FingerprintId { get; set; }
    public string FingerprintName { get; set; } = "";
    public string FingerprintNumber { get; set; } = "";
    public int FingerprintType { get; set; }
    public long LockId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Status { get; set; }
    public DateTime? CreateDate { get; set; }
}



// DTO untuk IC card
public class ICCardDto
{
    public long CardId { get; set; }
    public long LockId { get; set; }
    public string CardNumber { get; set; } = "";
    public string CardName { get; set; } = "";
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? CreateDate { get; set; }
    public string SenderUsername { get; set; } = "";
    public int CardType { get; set; }
}

public class EKeyDto
{
    public long EKeyId { get; set; }
    public long LockId { get; set; }
    public string Username { get; set; } = "";
    public int KeyStatus { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? CreateDate { get; set; }
}

public class PasscodeDto
{
    public long PasscodeId { get; set; }
    public long LockId { get; set; }
    public string KeyboardPwd { get; set; } = "";
    public string PwdName { get; set; } = "";
    public string SenderUsername { get; set; } = "";
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? CreateDate { get; set; }
}

public class RecordDto
{
    public string RecordType { get; set; } = "";
    public string Username { get; set; } = "";
    public string ClientName { get; set; } = "";
    public long LockId { get; set; }
    // public DateTime RecordDate { get; set; }
    public DateTime? LockDate { get; set; }
    public DateTime? ServerDate { get; set; }

}

public class TTLockResponse
{
    public int ErrCode { get; set; }
    public string ErrMsg { get; set; }
    public bool Success => ErrCode == 0;
}
