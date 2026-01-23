using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.FumationShops.Command;

public record UpdateFMShopsCommand(Guid ShopId, string name, string DisplayName, string Address, float Latitude, float Longitude, string OwnerName, string OwnerEmail, string OwnerPhone, string ShopPhoneNumber)
    : IRequest<SuccessResponse<string>>;

public class UpdateFMShopsCommandHandler
    : IRequestHandler<UpdateFMShopsCommand, SuccessResponse<string>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateFMShopsCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<SuccessResponse<string>> Handle(UpdateFMShopsCommand request, CancellationToken cancellationToken)
    {
        string UsedId = _currentUserService.UserId.ToString();
        var shop = await _context.FemgutionShops.FirstOrDefaultAsync(x => x.Id == request.ShopId);

        if (shop is null) throw new KeyNotFoundException("Shop not found.");

        shop.Name = request.name;
        shop.DisplayName = request.DisplayName;
        shop.Code = request.name?.Substring(0, request.name.Length / 2).ToUpper();
        shop.Address = request.Address;
        shop.Latitude = request.Latitude;
        shop.Longitude = request.Longitude;
        shop.OwnerName = request.OwnerName;
        shop.OwnerPhone = request.OwnerPhone;
        shop.OwnerEmail = request.OwnerEmail;
        shop.ShopPhoneNumber = request.ShopPhoneNumber;
        shop.LastModifiedBy = UsedId;
        shop.LastModified = DateTime.Now;

        await _context.SaveChangesAsync(cancellationToken);
        return Success.Update(shop.Id.ToString());
    }
}
