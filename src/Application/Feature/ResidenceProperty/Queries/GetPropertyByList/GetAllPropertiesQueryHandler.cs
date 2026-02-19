using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.Property.Queries;
using DHAFacilitationAPIs.Application.Feature.ResidenceProperty.Queries.GetPropertyByList;
using DHAFacilitationAPIs.Domain.Entities.Smartdha;
using DHAFacilitationAPIs.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Application.Feature.Property.Queries
{
    public class GetAllPropertiesQueryHandler
        : IRequestHandler<GetAllPropertiesQuery, Result<List<GetAllPropertiesResponse>>>
    {
        private readonly ISmartdhaDbContext _smartdhaDbContext;

        public GetAllPropertiesQueryHandler(ISmartdhaDbContext smartdhaDbContext)
        {
            _smartdhaDbContext = smartdhaDbContext;
        }

        public async Task<Result<List<GetAllPropertiesResponse>>> Handle(
            GetAllPropertiesQuery request,
            CancellationToken cancellationToken)
        {
            // Check if CreatedByUserId exists in ResidentProperty
            var propertiesQuery = _smartdhaDbContext.ResidentProperties.AsQueryable();
            if (request.IsActive) 
            {
                if (!string.IsNullOrEmpty(request.Id))
                {
                    propertiesQuery = propertiesQuery.Where(p => p.CreatedBy == request.Id && p.IsActive == true);
                }

                var properties = await propertiesQuery.AsNoTracking().ToListAsync(cancellationToken);

                if (!properties.Any())
                    return Result<List<GetAllPropertiesResponse>>.Failure(new[] { "No properties found" });

                var response = properties.Select(p => new GetAllPropertiesResponse
                {
                    Category = (CategoryType)p.Category,
                    Type = (PropertyType)p.Type,
                    Phase = (Phase)p.Phase,
                    Zone = (Zone)p.Zone,
                    StreetNo = p.StreetNo,
                    Khayaban = p.Khayaban,
                    Floor = p.Floor,
                    PlotNo = p.PlotNo,
                    Plot = p.Plot,
                    PossessionType = (ResidenceStatusDha)p.PossessionType
                }).ToList();
                return Result<List<GetAllPropertiesResponse>>.Success(response);
            }
            else
            {
                if (!string.IsNullOrEmpty(request.Id))
                {
                    propertiesQuery = propertiesQuery.Where(p => p.CreatedBy == request.Id && p.IsActive == false);
                }

                var properties = await propertiesQuery.AsNoTracking().ToListAsync(cancellationToken);

                if (!properties.Any())
                    return Result<List<GetAllPropertiesResponse>>.Failure(new[] { "No properties found" });

                var response = properties.Select(p => new GetAllPropertiesResponse
                {
                    Category = (CategoryType)p.Category,
                    Type = (PropertyType)p.Type,
                    Phase = (Phase)p.Phase,
                    Zone = (Zone)p.Zone,
                    StreetNo = p.StreetNo,
                    Khayaban = p.Khayaban,
                    Floor = p.Floor,
                    PlotNo = p.PlotNo,
                    Plot = p.Plot,
                    PossessionType = (ResidenceStatusDha)p.PossessionType
                }).ToList();
                return Result<List<GetAllPropertiesResponse>>.Success(response);
            }
        }
    }
}
