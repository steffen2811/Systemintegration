using GetDashboardData.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceReference1;
using System.Net;
using System.IO;
using GetDashboardData.backend;

namespace GetDashboardData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetDashboardDataController : ControllerBase
    {
        [HttpGet]
        public ReturnData Get()
        {
            WeatherData? weatherData = TimedBackgroundService.GetDashboardDataFromBackend();
            Inverter? inverterData = TimedBackgroundService.GetInverterDataFromBackend();
            var response = new ReturnData(weatherData, inverterData);
            return response;
        }
    }
}
