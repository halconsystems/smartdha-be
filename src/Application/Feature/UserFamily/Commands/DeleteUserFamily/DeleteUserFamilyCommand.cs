using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediatR;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.UserFamily.UserFamilyCommands.DeleteUserFamilyCommand;


public class DeleteUserFamilyCommand : IRequest<Result<Guid>>
{
    public Guid Id { get; set; }
}

