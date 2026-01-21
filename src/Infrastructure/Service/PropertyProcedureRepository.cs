using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DHAFacilitationAPIs.Application.Common.Dto;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Infrastructure.Service;
public class PropertyProcedureRepository : IPropertyProcedureRepository
{
    private readonly IProcedureService _procedureService;

    public PropertyProcedureRepository(IProcedureService procedureService)
    {
        _procedureService = procedureService;
    }

    public async Task<List<PropertyDetailDTO>> GetPropertiesByCnicAsync(
        string cnic,
        CancellationToken ct)
    {
        cnic = "4220125641462";
        var param = new DynamicParameters();
        param.Add("@memCNIC", cnic, DbType.String, size: 150);

        var (_, list) =
            await _procedureService.ExecuteWithListAsync<PropertyDetailDTO>(
                "USP_SelectPropertyByCNIC",
                param,
                ct
            );

        return list.ToList();
    }
}

