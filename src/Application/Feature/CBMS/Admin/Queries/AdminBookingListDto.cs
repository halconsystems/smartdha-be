using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.Admin.Queries;
public record AdminBookingListDto(
    Guid BookingId,
    DateTime CreatedOn,

    string CategoryName,
    string FacilityName,
    string UnitName,

    string UserName,
    string UserContact,

    BookingMode BookingMode,
    BookingStatus BookingStatus,
    PaymentStatus PaymentStatus,

    decimal SubTotal,
    decimal Discount,
    decimal Tax,
    decimal TotalAmount
);

