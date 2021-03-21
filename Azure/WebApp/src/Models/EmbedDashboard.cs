using System;
using Microsoft.PowerBI.Api.Models;

public class EmbedDashboard
{
    public EmbedToken EmbedToken { get; set; }
    public Guid DashboardId { get; set; }
    public string EmbedUrl { get; set; }
}