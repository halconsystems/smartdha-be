using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Options;

namespace DHAFacilitationAPIs.Infrastructure.Notifications;
public class FirebaseTokenProvider : IFirebaseTokenProvider
{
    private readonly FirebaseSettings _settings;

    public FirebaseTokenProvider(IOptions<FirebaseSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task<string> GetAccessTokenAsync(CancellationToken ct)
    {
        GoogleCredential credential;

        using (var stream = new FileStream(_settings.ServiceAccountJsonPath, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream)
                .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");
        }

        var token = await credential.UnderlyingCredential
            .GetAccessTokenForRequestAsync(cancellationToken: ct);

        return token;
    }
    public async Task<string> GetAccessPMSTokenAsync(CancellationToken ct)
    {
        GoogleCredential credential;

        using (var stream = new FileStream(_settings.DHAServiceAccountJsonPath, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream)
                .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");
        }

        var token = await credential.UnderlyingCredential
            .GetAccessTokenForRequestAsync(cancellationToken: ct);

        return token;
    }
}
