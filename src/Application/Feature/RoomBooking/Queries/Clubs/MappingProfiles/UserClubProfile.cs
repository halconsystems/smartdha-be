using Application.Feature.RoomBooking.Queries.Clubs.Dtos;
using AutoMapper;
using DHAFacilitationAPIs.Domain.Entities;

namespace Application.Feature.RoomBooking.Queries.Clubs.MappingProfiles;

public class UserClubProfile : Profile
{
    public UserClubProfile()
    {
        CreateMap<UserClubMembership, UserClubDto>()
            .ForMember(dest => dest.ClubId, opt => opt.MapFrom(src => src.ClubId));
    }
}
