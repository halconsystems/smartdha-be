using AutoMapper;
using DHAFacilitationAPIs.Application.Feature.Modules.Commands.UpdateUserModulePermissions.Dtos;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.Modules.Commands.UpdateUserModulePermissions.MappingProfiles;

public class ModulePermissionMappingProfile : Profile
{
    public ModulePermissionMappingProfile()
    {
        CreateMap<ModulePermissionDto, UserModuleAssignment>()
            .ForMember(dest => dest.ModuleId, opt => opt.MapFrom(src => src.ModuleId));
    }
}
