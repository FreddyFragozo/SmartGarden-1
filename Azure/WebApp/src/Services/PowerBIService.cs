namespace SmartSolutions.WebApp.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.PowerBI.Api;
    using Microsoft.PowerBI.Api.Models;
    using Microsoft.Rest;
    using SmartSolutions.WebApp.Config;

    public interface IPowerBIService
    {
        Task<EmbedDashboard> GetEmbedDashboard(CancellationToken cancellationToken = default);
    }

    public class PowerBIService : IPowerBIService
    {
        private readonly IAadService _aadService;
        private readonly PowerBIOptions _pbiOptions;
        private readonly ILogger<PowerBIService> _logger;
        public PowerBIService(IAadService aadService, IOptions<PowerBIOptions> pbiOptions, ILogger<PowerBIService> logger)
        {
            _aadService = aadService;
            _pbiOptions = pbiOptions.Value;
            _logger = logger;
        }

        private async Task<PowerBIClient> GetPowerBIClient()
        {
            var token = await _aadService.GetAccessToken();
            var tokenCredentials = new TokenCredentials(token, "Bearer");
            return new PowerBIClient(new Uri(_pbiOptions.ApiUrl), tokenCredentials);
        }

        public async Task<EmbedDashboard> GetEmbedDashboard(CancellationToken cancellationToken = default)
        {
            using (var pbiClient = await GetPowerBIClient())
            {
                var dashboard = await pbiClient.Dashboards.GetDashboardInGroupAsync(_pbiOptions.WorkspaceId, _pbiOptions.DashboardId);

                var embedDashboard = new EmbedDashboard
                {
                    DashboardId = dashboard.Id,
                    EmbedUrl = dashboard.EmbedUrl
                };

                embedDashboard.EmbedToken = await GetEmbedToken(dashboard, cancellationToken);

                return embedDashboard;
            }
        }

        /// <summary>
        /// Get Embed token for single dashboard, multiple datasets, and an optional target workspace
        /// </summary>
        /// <returns>Embed token</returns>
        private async Task<EmbedToken> GetEmbedToken(Dashboard Dashboard, CancellationToken cancellationToken = default)
        {
            using (PowerBIClient pbiClient = await GetPowerBIClient())
            {
                var tokenRequest = new GenerateTokenRequest(accessLevel: "view", _pbiOptions.DatasetId);

                // Generate Embed token
                return await pbiClient.Dashboards.GenerateTokenInGroupAsync(_pbiOptions.WorkspaceId, 
                    _pbiOptions.DashboardId, 
                    tokenRequest, 
                    cancellationToken);
            }
        }
    }
}