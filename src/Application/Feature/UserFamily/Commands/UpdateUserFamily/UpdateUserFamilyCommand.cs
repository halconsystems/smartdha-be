using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace DHAFacilitationAPIs.Application.Feature.UserFamily.Commands.UpdateUserFamilyCommandHandler;

using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

public class UpdateUserFamilyCommand : IRequest<Result<UpdateUserFamilyResponse>>
    {
        public Guid Id { get; set; }  
        public string? Name { get; set; } 
        public string? PhoneNo { get; set; } 
        public string? CNIC { get; set; } 
        public RelationUserFamily? Relation { get; set; }
        public DateTime? DOB { get; set; }
        public bool? IsActive { get; set; }
        public IFormFile? ProfilePicture { get; set; }
    
    }

