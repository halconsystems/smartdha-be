using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Common.Contracts.Mobile;
public sealed record PhaseDto(Guid Id, string Name);
public sealed record CapacityDto(Guid CapacityId, string Label, decimal? BaseRate);

public sealed record QuoteRequestDto(Guid PhaseId, Guid CapacityId, DateTime RequestedDeliveryDate);
public sealed record QuoteResponseDto(decimal Amount, string Currency, bool IsAllowed, int? MinLeadTimeMinutes);

public sealed record CreateRequestDto(
    Guid PhaseId,
    Guid CapacityId,
    DateTime RequestedDeliveryDate,
    string Street,
    string Address,
    decimal Latitude,
    decimal Longitude,
    string? Ext = null,
    string? Notes = null
);

public sealed record CreateRequestResultDto(Guid RequestId, string RequestNo, string Status, string PaymentStatus);

public sealed record RequestCardDto(
    Guid RequestId,
    string RequestNo,
    string Phase,
    string CapacityLabel,
    string Status,
    string PaymentStatus,
    decimal Amount,
    string Currency,
    string Street,
    string Address,
    decimal Latitude,
    decimal Longitude,
    string? DriverName,
    string? VehiclePlate,
    int? EtaMinutes,
    string? SignalRToken
);

public sealed record CancelRequestDto(string Reason);
public sealed record PaymentIntentDto(string Provider);

public sealed record PaymentIntentResultDto(
    Guid PaymentId,
    string Status,
    string? ProviderRedirectUrl,
    string? ProviderIntentId
);
public sealed record MyRequestHistoryCardDto(
    Guid RequestId,
    string RequestNo,
    DateTime RequestDate,
    DateTime RequestedDeliveryDate,
    string Phase,
    string CapacityLabel,
    string Status,          // enum → string, e.g., "Completed"
    string PaymentStatus,   // enum → string, e.g., "Paid"
    decimal Amount,
    string Currency,
    string? PaymentId,      // 12-digit you created; may be null
    string? Provider        // e.g., "COD", "Easypaisa"
);
