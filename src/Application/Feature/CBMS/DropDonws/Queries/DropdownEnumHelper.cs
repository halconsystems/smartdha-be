using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.DropDonws.Queries;
public static class DropdownEnumHelper
{
    public static List<DropdownDto> GetBookingModes()
        => Enum.GetValues<BookingMode>()
            .Select(x => new DropdownDto(
                Guid.Empty,
                x.ToString()))
            .ToList();
}

