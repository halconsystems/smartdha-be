using AutoMapper;
using DHAFacilitationAPIs.Application.Feature.Modules.Commands.UpdateUserModulePermissions.Dtos;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.Modules.Commands.UpdateUserModulePermissions.MappingProfiles;

public class SubModulePermissionMappingProfile : Profile
{
    public SubModulePermissionMappingProfile()
    {
        CreateMap<SubModulePermissionDto, AppRolePermission>()
            .ForMember(dest => dest.SubModuleId, opt => opt.MapFrom(src => src.SubModuleId));
    }
}
