using System;

namespace MyApp.Core.Entities
{
    public class GateDevice
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public int? CompanyId { get; set; }
        public int? AreaId { get; set; }
        public Area? Area { get; set; }
        public string? DeviceName { get; set; }
        public string? DeviceAlias { get; set; }
        public string DeviceSn { get; set; } = null!;
        public string? DeviceIp { get; set; }
        public bool IsRegistered { get; set; }
        public bool IsAttendance { get; set; }
        public int? Heartbeat { get; set; }
        public DateTime? LastHeartbeat { get; set; }
        public string? TransferMode { get; set; }
        public string? Timezone { get; set; }
        public string? DeviceOption { get; set; }
        public string? FirmwareVersion { get; set; }
        public string TotalEnrolledUser { get; set; } = "0";
        public string TotalEnrolledFingerprint { get; set; } = "0";
        public string TotalEnrolledFace { get; set; } = "0";
        public string TotalAttendancesRecord { get; set; } = "0";
        public string? FingerprintAlgorithmVersion { get; set; }
        public string? FaceAlgorithmVersion { get; set; }
        public string? RequiredFaceEnrollment { get; set; }
        public string? Value10 { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
