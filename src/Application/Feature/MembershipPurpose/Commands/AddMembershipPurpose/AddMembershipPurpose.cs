using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.MembershipPurpose.Commands.AddMembershipPurpose;
public class AddMembershipPurposeCommand : IRequest<SuccessResponse<string>>
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}
public class AddMembershipPurposeCommandHandler : IRequestHandler<AddMembershipPurposeCommand, SuccessResponse<string>>
{
    private readonly IApplicationDbContext _context;

    public AddMembershipPurposeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(AddMembershipPurposeCommand request, CancellationToken cancellationToken)
    {
        var entity = new Domain.Entities.MembershipPurpose
        {
            Title = request.Title,
            Description = request.Description
        };

        _context.MembershipPurposes.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>("Membership purpose created successfully.");
    }
}

