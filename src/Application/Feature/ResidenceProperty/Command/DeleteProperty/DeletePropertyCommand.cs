using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.ResidenceProperty.Command.DeleteProperty;

namespace DHAFacilitationAPIs.Application.Feature.Property.Command;

public class DeletePropertyCommand : IRequest<Result<DeletepropertyResponse>>
{
    public Guid Id { get; set; }
}

