using MediatR;
using Microsoft.EntityFrameworkCore;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Property.Queries;
using DHAFacilitationAPIs.Domain.Entities.Smartdha;

public class GetAllPropertiesQueryHandler
    : IRequestHandler<GetAllPropertiesQuery, List<ResidentProperty>>
{
    private readonly ISmartdhaDbContext _smartdhaDbContext;

    public GetAllPropertiesQueryHandler(ISmartdhaDbContext smartdhaDbContext)
    {
        _smartdhaDbContext = smartdhaDbContext;
    }

    public async Task<List<ResidentProperty>> Handle(
        GetAllPropertiesQuery request,
        CancellationToken cancellationToken)
    {
        return await _smartdhaDbContext.ResidentProperties
            .AsNoTracking()   
            .ToListAsync(cancellationToken);
    }
}
