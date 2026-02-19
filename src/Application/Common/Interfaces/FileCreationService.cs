using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Feature.VisitorPass.Queries.GetVisitorPassbyId;
using DHAFacilitationAPIs.Domain.Entities.Smartdha;

namespace DHAFacilitationAPIs.Application.Common.Interfaces;
public interface IFileCreationService
{
    byte[] GenerateVisitorPassPdf(VisitorPass visitorPass);
}
