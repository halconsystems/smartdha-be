using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Dto;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Common.Interfaces;
public interface IPropertyInfoService
{
    Task<List<PropertyDetailDTO>> GetPropertiesByCnicAsync(
        string cnic,
        CancellationToken cancellationToken
    );

    Task<List<SmartPayBillData>> GetPendingBillsByUserAsync(
       string userId,
       CancellationToken cancellationToken
   );

    Task<PropertyDetailDTO?> GetPropertyByPlotPkAsync(
        string cnic,
        string plotPk,
        CancellationToken ct
    );

}
