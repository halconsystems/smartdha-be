using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Common.Interfaces;
public interface ISmartPayService
{
    /// <summary>
    /// Consumer Inquiry API
    /// GET: /api/ConsumerInq/{cellNo}/{reserved}
    /// </summary>
    Task<SmartPayConsumerInquiryResponse> ConsumerInquiryAsync(
        string cellNo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Bill Inquiry by Consumer Number
    /// GET: /api/BillInq/{consumerNo}
    /// </summary>
    Task<SmartPayBillInquiryResponse> BillInquiryAsync(
        string consumerNo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Consumer Bills History
    /// GET: /api/ConsumerHistory/{consumerNo}
    /// </summary>
    Task<SmartPayConsumerHistoryResponse> ConsumerHistoryAsync(
        string consumerNo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Upload / Add Bill to SmartPay
    /// POST: /api/AddUpdateBill
    /// </summary>
    Task<SmartPayUploadBillResponse> UploadBillAsync(
        SmartPayUploadBillRequest request,
        CancellationToken cancellationToken = default);

    Task<SmartPayStatsBillResponse> IStats(
       string Prefix,string Since,
       CancellationToken cancellationToken = default);

    Task<SmartPayPayAtInsResponse> PayAtInsAsync(
        SmartPayPayAtInsRequest request,
        CancellationToken cancellationToken = default);
}
