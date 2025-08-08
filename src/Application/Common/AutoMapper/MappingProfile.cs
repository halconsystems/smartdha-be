using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Feature.Announcements.Commands.AddAnnouncement;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Common.AutoMapper;
internal class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Announcement, AnnouncementDto>();
    }
}
