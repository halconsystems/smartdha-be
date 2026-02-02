using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Common.Interfaces;
public interface ISecureKeyProtector
{
    string Encrypt(string plain);
    string Decrypt(string encrypted);
}

