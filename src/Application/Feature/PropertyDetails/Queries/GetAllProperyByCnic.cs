using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.PropertyDetails.Queries;

public record GetAllProperyByCnicQuery(string Cnic) : IRequest<List<PropertyDetailDTO>>;
public class GetAllProperyByCnicQueryHandler
    : IRequestHandler<GetAllProperyByCnicQuery, List<PropertyDetailDTO>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IProcedureService _sp;

    public GetAllProperyByCnicQueryHandler(UserManager<ApplicationUser> userManager, IProcedureService sp)
    {
        _userManager = userManager;
        _sp = sp;
    }

    public async Task<List<PropertyDetailDTO>> Handle(GetAllProperyByCnicQuery request, CancellationToken ct)
    {

        var p = new DynamicParameters();
        p.Add("@memCNIC", request.Cnic, DbType.String, size: 150);
        p.Add("@msg", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);


        var (_, steps) = await _sp.ExecuteWithListAsync<PropertyDetailDTO>(
            "USP_SelectPropertyByCNIC", p, ct);

        var result = steps.Select(x => new PropertyDetailDTO
        {
            PLOT_NO = x.PLOT_NO,
            STNAME = x.STNAME,
            PLTNO = x.PLTNO,
            SUBDIV = x.SUBDIV,
            PTYPE = x.PTYPE,
            PHASE = x.PHASE,
            EXT = x.EXT,
            NOMEAEA = x.NOMEAEA,
            ACTUAL_SIZE = x.ACTUAL_SIZE,
            STREET1COD = x.STREET1COD,
            PLOTPK = x.PLOTPK,
            MEMPK = x.MEMPK,
            MEMNO = x.MEMNO,
            CAT = x.CAT,
            NAME = x.NAME,
            APPLIDATE = x.APPLIDATE,
            NIC = x.NIC,
            CELLNO = x.CELLNO,
            ALLRESPLOT = x.ALLRESPLOT,
        }).ToList();

        return result;
    }
}

