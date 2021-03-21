using System.Collections.Generic;

namespace SmartSolutions.WebApp.Config
{
    public class AzureADOptions
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public IEnumerable<string> Scope { get; set; }
        public string TenantUrl { get; set; }
        public string AuthenticationMode { get; set; }
        public string AuthorityUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}