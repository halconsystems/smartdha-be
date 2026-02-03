using System.Text.Json;
using DHAFacilitationAPIs.Application.Feature.PaymentIpn.Commands.SavePaymentIpn;
using DHAFacilitationAPIs.Application.Feature.PaymentIpn.Commands.SaveWebhookCallback;
using DHAFacilitationAPIs.Application.Feature.PaymentIpn.Queries.InitiatePayFastCheckout;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "other")]
public class PaymentIpnController : BaseApiController
{
    private readonly IMediator _med;

    public PaymentIpnController(IMediator med)
    {
        _med = med;
    }

    [HttpPost("payments/payfast/initiate")]
    public async Task<IActionResult> InitiatePayFast(
    [FromBody] Guid billId,
    CancellationToken ct)
    {
        var result = await _med.Send(
            new InitiatePayFastCheckoutCommand(billId),
            ct);

        return Ok(result);
    }

    [HttpPost("checkout")]
    [AllowAnonymous] // IPN usually unauthenticated
    public async Task<IActionResult> ReceiveIpn(CancellationToken ct)
    {
        PaymentIpnRequestDto dto;
        string rawPayload;

        if (Request.HasFormContentType)
        {
            var form = Request.Form;
            dto = new PaymentIpnRequestDto
            {
                err_code = form["err_code"],
                err_msg = form["err_msg"],
                basket_id = form["basket_id"],
                transaction_id = form["transaction_id"],
                responseKey = form["responseKey"],
                Response_Key = form["Response_Key"],
                validation_hash = form["validation_hash"],
                order_date = form["order_date"],
                amount = form["amount"],
                transaction_amount = form["transaction_amount"],
                merchant_amount = form["merchant_amount"],
                discounted_amount = form["discounted_amount"],
                transaction_currency = form["transaction_currency"],
                PaymentName = form["PaymentName"],
                issuer_name = form["issuer_name"],
                masked_pan = form["masked_pan"],
                mobile_no = form["mobile_no"],
                email_address = form["email_address"],
                is_international = form["is_international"],
                recurring_txn = form["recurring_txn"],
                bill_number = form["bill_number"],
                customer_id = form["customer_id"],
                rdv_message_key = form["rdv_message_key"],
                additional_value = form["additional_value"]
            };

            rawPayload = JsonSerializer.Serialize(
                form.ToDictionary(x => x.Key, x => x.Value.ToString()));
        }
        else
        {
            using var reader = new StreamReader(Request.Body);
            rawPayload = await reader.ReadToEndAsync(ct);

            if (string.IsNullOrWhiteSpace(rawPayload))
                throw new InvalidOperationException("Empty JSON payload.");

            dto = JsonSerializer.Deserialize<PaymentIpnRequestDto>(
                rawPayload,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;
        }

        await _med.Send(
            new SavePaymentIpnCommand(dto, rawPayload),
            ct);
        if (dto.err_code == "000")
        {
            return Ok(new { status = "Success", message=dto.err_msg });
        }else
        {
            return Ok(new { status = "Failed", message = dto.err_msg });
        }
    }

    [HttpPost("success")]
    [AllowAnonymous]
    public async Task<IActionResult> SuccessReceiveCallback(CancellationToken ct)
    {
        string payload;

        // 🔹 HANDLE FORM (application/x-www-form-urlencoded)
        if (Request.HasFormContentType)
        {
            var form = await Request.ReadFormAsync(ct);

            payload = JsonSerializer.Serialize(
                form.ToDictionary(x => x.Key, x => x.Value.ToString())
            );
        }
        // 🔹 HANDLE JSON (application/json)
        else
        {
            using var reader = new StreamReader(Request.Body);
            payload = await reader.ReadToEndAsync(ct);

            if (string.IsNullOrWhiteSpace(payload))
                payload = "{}"; // avoid empty-body crash
        }

        // 🔹 Serialize headers
        var headers = JsonSerializer.Serialize(
            Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())
        );

        // 🔹 Persist callback
        var id = await _med.Send(new SaveWebhookCallbackCommand(
            Source: "IPN",                                // static (no param)
            Endpoint: Request.Path,
            HttpMethod: Request.Method,
            ContentType: Request.ContentType ?? "",
            Payload: payload,
            Headers: headers,
            ClientIp: HttpContext.Connection.RemoteIpAddress?.ToString()
        ), ct);

        return Ok(new
        {
            Status = "RECEIVED",
            CallbackId = id
        });
    }

    [HttpPost("failed")]
    [AllowAnonymous]
    public async Task<IActionResult> failedReceiveCallback(CancellationToken ct)
    {
        string payload;

        // 🔹 HANDLE FORM (application/x-www-form-urlencoded)
        if (Request.HasFormContentType)
        {
            var form = await Request.ReadFormAsync(ct);

            payload = JsonSerializer.Serialize(
                form.ToDictionary(x => x.Key, x => x.Value.ToString())
            );
        }
        // 🔹 HANDLE JSON (application/json)
        else
        {
            using var reader = new StreamReader(Request.Body);
            payload = await reader.ReadToEndAsync(ct);

            if (string.IsNullOrWhiteSpace(payload))
                payload = "{}"; // avoid empty-body crash
        }

        // 🔹 Serialize headers
        var headers = JsonSerializer.Serialize(
            Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())
        );

        // 🔹 Persist callback
        var id = await _med.Send(new SaveWebhookCallbackCommand(
            Source: "IPN",                                // static (no param)
            Endpoint: Request.Path,
            HttpMethod: Request.Method,
            ContentType: Request.ContentType ?? "",
            Payload: payload,
            Headers: headers,
            ClientIp: HttpContext.Connection.RemoteIpAddress?.ToString()
        ), ct);

        return Ok(new
        {
            Status = "RECEIVED",
            CallbackId = id
        });
    }
}

