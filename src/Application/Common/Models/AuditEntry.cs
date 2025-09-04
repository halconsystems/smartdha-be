using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DHAFacilitationAPIs.Application.Common.Models;
public class AuditEntry
{
    public AuditEntry(EntityEntry entry)
    {
        Entry = entry;
    }

    public EntityEntry Entry { get; }
    public string TableName { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public Dictionary<string, object?> KeyValues { get; } = new();
    public Dictionary<string, object?> OldValues { get; } = new();
    public Dictionary<string, object?> NewValues { get; } = new();

    public AuditLog ToAuditLog()
    {
        return new AuditLog
        {
            UserId = UserId,
            Action = Entry.State.ToString(), // Added, Modified, Deleted
            EntityName = TableName,
            EntityId = KeyValues.FirstOrDefault().Value?.ToString() ?? "",
            OldValues = OldValues.Any() ? JsonSerializer.Serialize(OldValues) : null,
            NewValues = NewValues.Any() ? JsonSerializer.Serialize(NewValues) : null,
            CreatedAt = DateTime.UtcNow
        };
    }
}

