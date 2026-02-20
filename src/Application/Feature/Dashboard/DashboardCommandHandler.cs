using System;
using System.Collections.Generic;
using System.Linq;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Dashboard;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class DashboardCommandHandler
    : IRequestHandler<DashboardCommand, DashboardResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ISmartdhaDbContext _smartdhaDbContext;

    public DashboardCommandHandler(IApplicationDbContext context, ISmartdhaDbContext smartdhaDbContext)
    {
        _context = context;
        _smartdhaDbContext = smartdhaDbContext;
    }

    public async Task<DashboardResponse> Handle(
        DashboardCommand request,
        CancellationToken cancellationToken)
    {
        return new DashboardResponse
        {
            TotalWorkers = await _smartdhaDbContext.Workers.CountAsync(cancellationToken),
            TotalResidents = await _smartdhaDbContext.ResidentProperties.CountAsync(cancellationToken),
            TotalProperties = await _smartdhaDbContext.UserFamilies.CountAsync(cancellationToken),
            TotalVehicles = await _smartdhaDbContext.Vehicles.CountAsync(cancellationToken)
        };
    }
}

