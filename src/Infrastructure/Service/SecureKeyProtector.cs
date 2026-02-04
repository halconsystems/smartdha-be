using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using Microsoft.AspNetCore.DataProtection;

namespace DHAFacilitationAPIs.Infrastructure.Service;
public class SecureKeyProtector : ISecureKeyProtector
{
    private readonly IDataProtector _protector;

    public SecureKeyProtector(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector("MerchantSecureKey");
    }

    public string Encrypt(string plain) => _protector.Protect(plain);
    //public string Decrypt(string encrypted) => _protector.Unprotect(encrypted);

    public string Decrypt(string encrypted)
    {
        try
        {
            return _protector.Unprotect(encrypted);
        }
        catch (CryptographicException ex)
        {
            Console.WriteLine($"Decryption failed: {ex.Message}");
            return ""; // or log + handle gracefully
        }
    }

}

