using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.User.Queries.GetAccessTree;
using DHAFacilitationAPIs.Application.Interface.Repository;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.Modules.Commands.AddModule;
public record AddModuleCommand : IRequest<SuccessResponse<Guid>>
{
    public string Value { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Remarks { get; set; } = default!;
    public string Title { get; set; }   =default!;
    public string URL { get; set; } =default!;
    public AppType AppType { get; set; } = default!;
}

public class AddModuleCommandHandler : IRequestHandler<AddModuleCommand, SuccessResponse<Guid>>
{
    private readonly IApplicationDbContext _context;

    public AddModuleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<Guid>> Handle(AddModuleCommand request, CancellationToken cancellationToken)
    {
        var module = new Module
        {
            Id = Guid.NewGuid(),
            Value=request.Value,
            DisplayName=request.DisplayName,
            Name = request.Name,
            Description = request.Description,
            Remarks = request.Remarks,
            Title=request.Title,
            URL = request.URL,
            AppType= request.AppType
        };

        _context.Modules.Add(module);
        await _context.SaveChangesAsync(cancellationToken);

        
        return new SuccessResponse<Guid>(module.Id,"Module created successfully!");
    }
}


