using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetPanicById;
public class PanicHistoryDetailDto
{
    // PANIC INFO
    public Guid PanicId { get; set; }
    public string CaseNo { get; set; } = default!;
    public double PanicLatitude { get; set; }
    public double PanicLongitude { get; set; }
    public PanicStatus PanicStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Address { get; set; } = "";
    public string Note { get; set; } = "";
    public string ControlRoomRemarks { get; set; } = "";
    public string DriverRemarks { get; set; } = "";
    public string FinalAudioNote { get; set; } = "";
    public string EmergencyName { get; set; } = "";

    // REQUESTED BY (PANIC OWNER)
    public string RequestedByUserId { get; set; } = default!;
    public string RequestedByName { get; set; } = "";
    public string? RequestedByEmail { get; set; }
    public string? RequestedByPhone { get; set; }
    public UserType RequestedByUserType { get; set; }

    // DISPATCH INFO
    public Guid? DispatchId { get; set; }
    public DateTime? AssignedAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? OnRouteAt { get; set; }
    public DateTime? ArrivedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    // DISPATCH TIME IN MINUTES
    public double? CreatedToAssignedMinutes { get; set; }
    public double? AssignedToAcceptedMinutes { get; set; }
    public double? AcceptedToArrivedMinutes { get; set; }
    public double? ArrivedToCompletedMinutes { get; set; }
    public double? TotalCompletionMinutes { get; set; }

    // DRIVER INFO
    public string DriverUserId { get; set; } = "";
    public string DriverName { get; set; } = "";
    public string? DriverEmail { get; set; }
    public string? DriverPhone { get; set; }
    public string? DriverCnic { get; set; }

    // VEHICLE INFO
    public Guid? VehicleId { get; set; }
    public string? VehicleName { get; set; } = "";
    public string? RegistrationNo { get; set; } = "";
    public string? VehicleType { get; set; } = "";
    public string? VehicleStatus { get; set; } = "";
    public string? MapIconKey { get; set; }

    // LIVE LOCATION OF VEHICLE (JSON override)
    public double? LastLatitude { get; set; }
    public double? LastLongitude { get; set; }
    public DateTime? LastLocationAt { get; set; }
    public List<PanicDispatchMediaDto> CompletionMedia { get; set; } = new();
    // GEO & MISC
    public double? AcceptedAtLatitude { get; set; }
    public double? AcceptedAtLongitude { get; set; }
    [MaxLength(300)] public string? AcceptedAtAddress { get; set; }
    public double? DistanceFromPanicKm { get; set; }
    public DateTime? LastLocationUpdateAt { get; set; }
    public PanicReviewDto? Review { get; set; }
}
public class PanicReviewDto
{
    public int Rating { get; set; }
    public string? ReviewText { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PanicDispatchMediaDto
{
    public PanicDispatchMediaType MediaType { get; set; }   // Image / Video
    public string Url { get; set; } = default!;
}
