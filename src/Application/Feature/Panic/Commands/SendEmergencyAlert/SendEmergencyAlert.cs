using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Commands.SendEmergencyAlert;

using DHAFacilitationAPIs.Application.Common.Interfaces;
using MediatR;

public class SendEmergencyAlertCommand : IRequest<Unit>
{
    public string DeviceToken { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Body { get; set; } = default!;
    public Dictionary<string, string>? Data { get; set; }
}
public class SendEmergencyAlertCommandHandler: IRequestHandler<SendEmergencyAlertCommand, Unit>
{
    private readonly INotificationService _notificationService;

    public SendEmergencyAlertCommandHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<Unit> Handle(
        SendEmergencyAlertCommand request,
        CancellationToken cancellationToken)
    {
        await _notificationService.SendFirebaseNotificationAsync(
            request.DeviceToken,
            request.Title,
            request.Body,
            request.Data,
            cancellationToken);

        return Unit.Value;
    }
}
