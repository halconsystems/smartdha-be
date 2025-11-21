using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Common.Contracts;
public class SmartPayBillId
{
    private static readonly object _lock = new();
    private static int _sequence = 0;
    private static string _lastSecond = "";

    public static string GenerateOneBillId(string clubAccountNo)
    {
        if (string.IsNullOrWhiteSpace(clubAccountNo) || clubAccountNo.Length != 4 || !clubAccountNo.All(char.IsDigit))
            throw new ArgumentException("clubAccountNo must be a 4-digit numeric string.");

        var now = DateTime.UtcNow;

        // 8 digits (yyMMddHH)
        string ts = now.ToString("yyMMddHH");

        int seq;

        lock (_lock)
        {
            string secondKey = now.ToString("yyMMddHHmmss");

            if (_lastSecond != secondKey)
            {
                _sequence = 0;
                _lastSecond = secondKey;
            }

            _sequence++;
            if (_sequence > 99) _sequence = 1;

            seq = _sequence;
        }

        // 2-digit sequence
        string seqStr = seq.ToString("D2");

        // Final = 4 + 8 + 2 = 14 digits
        string final = $"{clubAccountNo}{ts}{seqStr}";

        if (final.Length != 14)
            throw new Exception($"OneBillId must be 14 digits, got {final.Length}");

        return final;
    }


}
