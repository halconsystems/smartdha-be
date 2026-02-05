using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.PaymentIpn.Commands.SaveWebhookCallback;
public record SaveWebhookCallbackCommand(
    string Source,
    string Endpoint,
    string HttpMethod,
    string ContentType,
    string Payload,
    string Headers,
    string? ClientIp
) : IRequest<Guid>;
public class SaveWebhookCallbackCommandHandler
    : IRequestHandler<SaveWebhookCallbackCommand, Guid>
{
    private readonly IPaymentDbContext _ctx;

    public SaveWebhookCallbackCommandHandler(IPaymentDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<Guid> Handle(SaveWebhookCallbackCommand request, CancellationToken ct)
    {
        var entity = new WebhookCallbackLog
        {
            Source = request.Source,
            Endpoint = request.Endpoint,
            HttpMethod = request.HttpMethod,
            ContentType = request.ContentType,
            Payload = request.Payload,
            Headers = request.Headers,
            ClientIp = request.ClientIp,
            IsProcessed = false
        };

        _ctx.WebhookCallbackLogs.Add(entity);
        await _ctx.SaveChangesAsync(ct);

        return entity.Id;
    }
}

