using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums.GBMS;
using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;

namespace DHAFacilitationAPIs.Application.Feature.Grounds.Command;

internal class UpdateGround
{
}

public class UpdateGroundCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public Guid ClubId { get; set; }
    public GroundCategory GroundCategory { get; set; }
    public GroundType GroundType { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? ContactNumber { get; set; }
    public string? AccountNo { get; set; }
    [MaxLength(4)] public string? AccountNoAccronym { get; set; }
}

public class UpdateGroundCommandHandler : IRequestHandler<UpdateGroundCommand, bool>
{
    private readonly IOLMRSApplicationDbContext _context;

    public UpdateGroundCommandHandler(IOLMRSApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateGroundCommand request, CancellationToken cancellationToken)
    {
        var ground = await _context.Grounds.FindAsync(new object[] { request.Id }, cancellationToken);

        if (ground == null)
            return false;

        ground.ClubId = request.ClubId;
        ground.GroundCategory = request.GroundCategory;
        ground.GroundType = request.GroundType;
        ground.Name = request.Name;
        ground.Description = request.Description;
        ground.Location = request.Location;
        ground.ContactNumber = request.ContactNumber;
        ground.AccountNo = request.AccountNo;
        ground.AccountNoAccronym = request.AccountNoAccronym;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

