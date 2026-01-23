using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.FemigationDT.Command;

public static class SettingRule
{
    public static bool IsDiscount(Domain.Enums.Settings setting)
    {
        return setting == Domain.Enums.Settings.Member
            || setting == Domain.Enums.Settings.MemStaff
            || setting == Domain.Enums.Settings.NonMember
            || setting == Domain.Enums.Settings.Employee;
    }
}
