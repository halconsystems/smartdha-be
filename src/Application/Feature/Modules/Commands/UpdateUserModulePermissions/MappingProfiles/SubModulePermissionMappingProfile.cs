using AutoMapper;
using DHAFacilitationAPIs.Application.Feature.Modules.Commands.UpdateUserModulePermissions.Dtos;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.Modules.Commands.UpdateUserModulePermissions.MappingProfiles;

public class SubModulePermissionMappingProfile : Profile
{
    public SubModulePermissionMappingProfile()
    {
        CreateMap<SubModulePermissionDto, RolePermission>()
            .ForMember(dest => dest.SubModuleId, opt => opt.MapFrom(src => src.SubModuleId))
            .ForMember(dest => dest.CanRead, opt => opt.MapFrom(src => src.CanRead))
            .ForMember(dest => dest.CanWrite, opt => opt.MapFrom(src => src.CanWrite))
            .ForMember(dest => dest.CanDelete, opt => opt.MapFrom(src => src.CanDelete));
    }
}
