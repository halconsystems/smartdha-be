using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Common.Interfaces;

public interface IEnumService
{
    object GetEnums(EnumType? enumType);
}
