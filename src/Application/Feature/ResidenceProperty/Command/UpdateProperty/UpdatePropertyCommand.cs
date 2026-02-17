using System;
using MediatR;
using DHAFacilitationAPIs.Application.Feature.ResidenceProperty.Command.UpdateProperty;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Application.Feature.Property.Command
{
    public class UpdatePropertyCommand : IRequest<UpdatePropertyResponse>
    {
        public Guid Id { get; set; }

        public CategoryType Category { get; set; }
        public PropertyType Type { get; set; }
        public Phase Phase { get; set; }
        public Zone Zone { get; set; }
        public string Khayaban { get; set; } = string.Empty;
        public int Floor { get; set; }
        public string StreetNo { get; set; } = string.Empty;
        public int PlotNo { get; set; }
        public string Plot { get; set; } = string.Empty;
        public ResidenceStatusDha PossessionType { get; set; }

        public IFormFile? ProofOfPossessionImage { get; set; }
        public IFormFile? UtilityBillAttachment { get; set; }

    }
}
