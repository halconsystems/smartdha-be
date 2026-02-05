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

public class MemberDetailForRenewalProcedureRepository : IMemberRenewalProcedurerespository
{
    private readonly IProcedureService _procedureService;

    public MemberDetailForRenewalProcedureRepository(IProcedureService procedureService)
    {
        _procedureService = procedureService;
    }

    public async Task<List<MemberShipDTO>> GetMemberShipRenewalAsync(
        string MemPk,
        CancellationToken ct)
    {
        MemPk = "4220125641462";
        var param = new DynamicParameters();
        param.Add("@memCNIC", MemPk, DbType.String, size: 150);

        var (_, list) =
            await _procedureService.ExecuteWithListAsync<MemberShipDTO>(
                "USP_SelectPropertyByCNIC",
                param,
                ct
            );

        return list.ToList();
    }
}


