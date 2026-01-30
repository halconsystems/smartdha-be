using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.OrderDispatch.Command.PickUp;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.FMS;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using static Dapper.SqlMapper;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.FumigationProcess.Command;

public class FemugationProcess : IRequest<string>
{
    public Guid FemugationId {  get; set; }
    public bool Acknowledged {  get; set; }
    public bool AssginedAt {  get; set; }
    public bool ResolvedAt {  get; set; }
    public bool CancelledAt {  get; set; }
    public string? AssignedTo {  get; set; }
    public string? Remarks {  get; set; }
    public IFormFile? VoiceFile { get; set; }   // optional mp3
    public List<IFormFile>? MediaFiles { get; set; }  // optional images / videos
}

public class FemugationProcessCommandHandler
    : IRequestHandler<FemugationProcess, string>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageService _fileStorage;

    public FemugationProcessCommandHandler(
        ICurrentUserService currentUser,
        IApplicationDbContext context,
        IFileStorageService fileStorage)
    {
        _currentUser = currentUser;
        _context = context;
        _fileStorage = fileStorage;
    }

    public async Task<string> Handle(FemugationProcess request, CancellationToken ct)
    {
        var UsedId = _currentUser.UserId;

        Domain.Entities.FMS.Fumigation? FumigationDispatch = null;

        if (request.ResolvedAt)
        {
            FumigationDispatch = await _context.Fumigations
               .Where(x => x.Id == request.FemugationId 
               && x.FemStatus != FemStatus.InProgress && x.FemStatus != FemStatus.Cancelled)
               .OrderByDescending(x => x.AssginedAt)
               .FirstOrDefaultAsync(ct);

        }
        else
        {
            FumigationDispatch = await _context.Fumigations
               .Where(x => x.Id == request.FemugationId &&
               x.AssigndTo != null &&
               x.FemStatus != FemStatus.Resolved && x.FemStatus != FemStatus.Cancelled)
               .OrderByDescending(x => x.AssginedAt)
               .FirstOrDefaultAsync(ct);

        }

        if (FumigationDispatch != null)
        {
            // Return existing dispatch (NO transaction, NO update)
            return FumigationDispatch.Id.ToString();
        }

        try
        {
            // Get Panic
            var Fumigations = await _context.Fumigations
                .FirstOrDefaultAsync(p => p.Id == request.FemugationId, ct)
                ?? throw new NotFoundException(nameof(Domain.Entities.FMS.Fumigation), request.FemugationId.ToString());


            if (request.Acknowledged == true)
            {
                Fumigations.AcknowledgedAt = DateTime.Now;
                Fumigations.FemStatus = FemStatus.Acknowledged;
            }
            if(request.AssginedAt == true)
            {

                Fumigations.AssginedAt = DateTime.Now;
                Fumigations.FemStatus = FemStatus.InProgress;
                Fumigations.AssigndTo = request.AssignedTo;
                Fumigations.AssignedBy = UsedId;
            }
            if(request.ResolvedAt == true)
            {
                if (request.VoiceFile != null)
                {
                    var allowedExtensions = new[] { ".mp3", ".aac" };

                    var relativePath = await _fileStorage.SaveAudioAsync(
                        request.VoiceFile,
                        folderName: "fumagation-user-remarks",
                        ct,
                        maxBytes: 10 * 1024 * 1024,
                        allowedExtensions: allowedExtensions
                    );

                    Fumigations.DriverRemarksAudioPath = relativePath;
                }

                Fumigations.ResolvedAt = DateTime.Now;
                Fumigations.FemStatus = FemStatus.Resolved;
                Fumigations.Remarks = request.Remarks;


                if (request.MediaFiles != null && request.MediaFiles.Any())
                {
                    foreach (var file in request.MediaFiles)
                    {
                        var result = await _fileStorage.FemugationSaveImageOrVideoAsync(
                            file,
                            folderName: "femugation-process-completion",
                            ct
                        );

                        _context.FumgationMedias.Add(new FumgationMedia
                        {
                            FemugationId = Fumigations.Id,
                            FilePath = result.Path,
                            MediaType = result.MediaType
                        });
                    }
                }


            }
            if (request.CancelledAt)
            {
                Fumigations.CancelledAt = DateTime.Now;
                Fumigations.FemStatus = FemStatus.Cancelled;
                Fumigations.Remarks = request.Remarks;
            }
            await _context.SaveChangesAsync(ct);

            return Fumigations.Id.ToString();
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(e.ToString());
        }

    }
}




