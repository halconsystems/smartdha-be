using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Dto;

namespace DHAFacilitationAPIs.Application.Common.Interfaces;
public interface IPaymentBillService
{
    Task<Guid> CreatePaymentBillAsync(CreatePaymentBillRequest request, CancellationToken ct);
}

