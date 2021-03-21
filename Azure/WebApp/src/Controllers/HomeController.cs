namespace SmartSolutions.WebApp.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using SmartSolutions.WebApp.Services;

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPowerBIService _pbiService;
        private readonly IDeviceService _deviceService;

        public HomeController(ILogger<HomeController> logger, 
            IPowerBIService pbiService,
            IDeviceService deviceService)
        {
            _logger = logger;
            _pbiService = pbiService;
            _deviceService = deviceService;
        }

        public async Task<IActionResult> Index()
        {
            var devices = await _deviceService.GetDevicesAsync();
            return View(devices);
        }

        public async Task<IActionResult> Report()
        {
            var embedDasboard = await _pbiService.GetEmbedDashboard();
            return View(embedDasboard);
        }

        
        [HttpPost]
        public async Task<IActionResult> OpenValve(string deviceId) 
        {
            _logger.LogInformation($"Open valve for {deviceId}' device...");
            await _deviceService.OpenValve(deviceId);            
            return Ok();
        }
    }
}
