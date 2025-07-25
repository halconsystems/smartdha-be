using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Interface.Repository;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.Modules.Commands.AddModule;
public record AddModuleCommand : IRequest<Guid>
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Remarks { get; set; } = default!;
}

public class AddModuleCommandHandler : IRequestHandler<AddModuleCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public AddModuleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(AddModuleCommand request, CancellationToken cancellationToken)
    {
        var module = new Module
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Remarks = request.Remarks
        };

        _context.Modules.Add(module);
        await _context.SaveChangesAsync(cancellationToken);

        return module.Id;
    }
}


