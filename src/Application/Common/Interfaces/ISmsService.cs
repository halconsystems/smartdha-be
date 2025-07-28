using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Common.Interfaces;
public interface ISmsService
{
    /// <summary>
    /// Sends an SMS and returns the provider’s raw status string.
    /// </summary>
    Task<string> SendSmsAsync(string cellnumber, string msg, CancellationToken cancellationToken);
}
