namespace SmartSolutions.WebApp.Services
{
    using Microsoft.Extensions.Options;
    using Microsoft.Identity.Client;
    using SmartSolutions.WebApp.Config;
    using System;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;

    public interface IAadService
    {
        Task<string> GetAccessToken();
    }

    public class AadService : IAadService
    {
        private readonly AzureADOptions _azureAdOptions;

        public AadService(IOptions<AzureADOptions> azureAdOptions)
        {
            _azureAdOptions = azureAdOptions.Value;
        }

        /// <summary>
        /// Generates and returns Access token
        /// </summary>
        /// <returns>AAD token</returns>
        public async Task<string> GetAccessToken()
        {
            AuthenticationResult authenticationResult = null;

            if (_azureAdOptions.AuthenticationMode.Equals("masteruser", StringComparison.InvariantCultureIgnoreCase))
            {
                // Create a public client to authorize the app with the AAD app
                IPublicClientApplication clientApp = PublicClientApplicationBuilder.Create(_azureAdOptions.ClientId)
                    .WithAuthority(_azureAdOptions.AuthorityUrl)
                    .Build();

                var userAccounts = clientApp.GetAccountsAsync().Result;

                if (!userAccounts.Any())
                {
                    authenticationResult = await AuthenticateByUsernamePassword(clientApp);
                }
                else
                {
                    authenticationResult = await clientApp.AcquireTokenSilent(_azureAdOptions.Scope, userAccounts.FirstOrDefault())
                        .ExecuteAsync();
                }
            }
            else
            {
                // Create a confidential client to authorize the app with the AAD app
                IConfidentialClientApplication clientApp = ConfidentialClientApplicationBuilder
                    .Create(_azureAdOptions.ClientId)
                    .WithClientSecret(_azureAdOptions.ClientSecret)
                    .WithAuthority(_azureAdOptions.TenantUrl)
                    .Build();

                authenticationResult = await clientApp.AcquireTokenForClient(_azureAdOptions.Scope).ExecuteAsync();
            }

            return authenticationResult.AccessToken;
        }

        private async Task<AuthenticationResult> AuthenticateByUsernamePassword(IPublicClientApplication clientApp)
        {
            SecureString password = new SecureString();
            foreach (var key in _azureAdOptions.Password)
            {
                password.AppendChar(key);
            }
            
            return await clientApp.AcquireTokenByUsernamePassword(_azureAdOptions.Scope, _azureAdOptions.Username, password)
                .ExecuteAsync();
        }
    }
}