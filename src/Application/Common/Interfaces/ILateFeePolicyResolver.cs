using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities.BillsPayment;

namespace DHAFacilitationAPIs.Application.Common.Interfaces;
public interface ILateFeePolicyResolver
{
    Task<PayLateFeePolicy> ResolveAsync(string sourceSystem, CancellationToken ct);
}

