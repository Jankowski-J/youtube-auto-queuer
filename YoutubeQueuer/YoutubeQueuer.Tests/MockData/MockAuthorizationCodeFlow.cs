using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Util;
using Google.Apis.Util.Store;

namespace YoutubeQueuer.Tests.MockData
{
    public class MockAuthorizationCodeFlow : IAuthorizationCodeFlow
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<TokenResponse> LoadTokenAsync(string userId, CancellationToken taskCancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteTokenAsync(string userId, CancellationToken taskCancellationToken)
        {
            throw new NotImplementedException();
        }

        public AuthorizationCodeRequestUrl CreateAuthorizationCodeRequest(string redirectUri)
        {
            throw new NotImplementedException();
        }

        public Task<TokenResponse> ExchangeCodeForTokenAsync(string userId, string code, string redirectUri, CancellationToken taskCancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TokenResponse> RefreshTokenAsync(string userId, string refreshToken, CancellationToken taskCancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RevokeTokenAsync(string userId, string token, CancellationToken taskCancellationToken)
        {
            throw new NotImplementedException();
        }

        public bool ShouldForceTokenRetrieval()
        {
            throw new NotImplementedException();
        }

        public IAccessMethod AccessMethod { get; }
        public IClock Clock { get; }
        public IDataStore DataStore { get; }
    }
}