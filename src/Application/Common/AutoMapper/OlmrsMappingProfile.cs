using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Feature.Clubs.Queries;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Common.AutoMapper;
public class OlmrsMappingProfile : Profile
{
    public OlmrsMappingProfile()
    {
        CreateMap<Club, ClubDto>();
        //CreateMap<RoomCategory, RoomCategoryDto>();
        //CreateMap<ResidenceType, ResidenceTypeDto>();
        //CreateMap<Services, ServiceDto>();
    }
}

