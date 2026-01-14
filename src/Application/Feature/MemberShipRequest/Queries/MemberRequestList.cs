//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using DHAFacilitationAPIs.Application.Common.Interfaces;
//using DHAFacilitationAPIs.Application.Feature.MemberShipCategory.Queries;

//namespace DHAFacilitationAPIs.Application.Feature.MemberShipRequest.Queries;

//public record GetMemberRequestListQuery : IRequest<List<MemberRequestDTO>>;
//public class GetMemberRequestListQueryHandler : IRequestHandler<GetMemberRequestListQuery,List<MemberRequestDTO>>
//{
//    private readonly IApplicationDbContext _context;

//    public GetMemberRequestListQueryHandler(IApplicationDbContext context)
//    {
//        _context = context;
//    }

//    public async Task<List<MemberRequestDTO>> Handle(GetMemberRequestListQuery request, CancellationToken ct)
//    {
//        var memberRequest = _context.MemberRequests
//            .Include(x => x.)
//            .AsNoTracking().ToList();

//        var result = memberRequest.Select(x => new MemberRequestDTO
//        {
//            MemberShipCategory = x.MemberShipCategory,
//            Name = x.Name,
//            CNIC = x.CNIC,
//            MemberShipCatergories = x.MemberShipCatergories,
//            MemberShipName = _context.MemberShips.
//        })
//    }
//}
