using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Common.Contracts;
public class SmartPayBillId
{
    public static string GenerateOneBillId(string clubAccountNo)
    {
        string smartPayCode = "SMPY"; // Always 4 chars
        string yymm = DateTime.UtcNow.ToString("yyMM"); // e.g., 2508
        string invoiceNo = new Random().Next(1, 99).ToString("D2"); // 2-digit padded

        string final = $"{smartPayCode}{clubAccountNo}{yymm}{invoiceNo}"; // 14 chars

        if (final.Length != 14)
            throw new Exception($"Generated OneBillId must be 14 chars, got {final.Length}");

        return final;
    }
}
