using System;

namespace SmartSolutions.WebApp.Config
{
    public class PowerBIOptions
    {
        public string ApiUrl { get; set; }
        public Guid DashboardId { get; set; }
        public Guid WorkspaceId { get; set; }
        public string DatasetId { get; set; }
    }
}