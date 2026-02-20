using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Dashboard;

    public class DashboardResponse
    {
        public int TotalWorkers { get; set; }
        public int TotalResidents { get; set; }
        public int TotalProperties { get; set; }
        public int TotalVehicles { get; set; }
    }
