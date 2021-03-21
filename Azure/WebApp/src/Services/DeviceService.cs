using System.Threading.Tasks;
using System.Linq;
using Microsoft.Azure.Devices;
using Microsoft.Extensions.Options;
using SmartSolutions.WebApp.Models;
using System.Collections.Generic;
using System.Text;

namespace SmartSolutions.WebApp.Services 
{
    public interface IDeviceService
    {
        Task<IEnumerable<GetDeviceViewModel>> GetDevicesAsync();
        Task OpenValve(string deviceId);
    }

    public class DeviceService : IDeviceService
    {
        private readonly RegistryManager _registryManager;
        private readonly IotHubOptions _iotHubConfig;
        private readonly ServiceClient _serviceClient;

        public DeviceService(IOptions<IotHubOptions> iotHubConfigOptions)
        {
            _iotHubConfig = iotHubConfigOptions.Value;
            _registryManager = RegistryManager.CreateFromConnectionString(_iotHubConfig.ConnectionString);
            _serviceClient = ServiceClient.CreateFromConnectionString(_iotHubConfig.ConnectionString);
        }

        public async Task<IEnumerable<GetDeviceViewModel>> GetDevicesAsync ()
        {
            var query = _registryManager.CreateQuery("SELECT * FROM devices", 100);

            var cloudDevices = (await query.GetNextAsTwinAsync());

            return cloudDevices.Select(a => new GetDeviceViewModel { Id = a.DeviceId, 
                Status = a.Status.ToString(),
                LastActivityTime = a.LastActivityTime,
                ConnectionState = a.ConnectionState
            });
        }

        public async Task OpenValve(string deviceId) 
        {
            var commandMessage = new
                Message(Encoding.ASCII.GetBytes("OpenValve"));
                await _serviceClient.SendAsync(deviceId, commandMessage);
        }
    }
}