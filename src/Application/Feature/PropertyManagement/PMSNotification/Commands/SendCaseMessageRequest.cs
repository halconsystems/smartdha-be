using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSNotification.Commands;
public record SendCaseMessageRequest
(
    Guid CaseId,
    MessageChannel Channel,

    // Template
    string TemplateCode,

    // Optional override
    string? CustomTitle,
    string? CustomMessage,

    // Optional variables
    Dictionary<string, string>? Placeholders
);

