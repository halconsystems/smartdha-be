using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.Admin.Queries.AdminBookingDashboard;
public record AdminBookingDashboardDto(
    int TodayBookings,
    decimal TodayRevenue,

    int WeekBookings,
    decimal WeekRevenue,

    int MonthBookings,
    decimal MonthRevenue
);

