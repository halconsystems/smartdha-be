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
public class MemberLookupService : IMemberLookupService
{
    private readonly IProcedureService _procedureService;

    public MemberLookupService(IProcedureService procedureService)
    {
        _procedureService = procedureService;
    }

    public async Task<(MemberLookupResult Member, List<ClubMembershipDto> Clubs)>
        GetMemberByCnicAsync(string cnic, CancellationToken ct)
    {
        var parameters = BuildParameters(cnic);

        var (member, clubs) =
            await _procedureService.ExecuteWithOutputAndListAsync<ClubMembershipDto>(
                "USP_SelectMemberByCNIC_NEW",
                parameters,
                ct);

        return (member, clubs);
    }

    private static DynamicParameters BuildParameters(string cnic)
    {
        var p = new DynamicParameters();

        p.Add("@CNICNO", cnic);

        p.Add("@MEMID", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
        p.Add("@MemNo", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
        p.Add("@STAFFNO", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
        p.Add("@Cat", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
        p.Add("@Name", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
        p.Add("@ApplicationDate", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
        p.Add("@NIC", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
        p.Add("@CellNo", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
        p.Add("@ALLREPLOT", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
        p.Add("@MEMPK", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
        p.Add("@Email", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
        p.Add("@DOB", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
        p.Add("@msg", dbType: DbType.String, size: 150, direction: ParameterDirection.Output);

        return p;
    }
}

