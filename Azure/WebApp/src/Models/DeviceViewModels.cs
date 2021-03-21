namespace SmartSolutions.WebApp.Models 
{
    using System;
    using Microsoft.Azure.Devices;

    public class GetDeviceViewModel 
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public DateTime? LastActivityTime { get; set; }
        public DeviceConnectionState? ConnectionState { get; set; }
    }
}