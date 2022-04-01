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
            InverterData? inverterData = TimedBackgroundService.GetInverterDataFromBackend();
            RoomTempData? roomTempData = TimedBackgroundService.GetTemperatureDataFromBackend();
            var response = new ReturnData(weatherData, inverterData, roomTempData);
            return response;
        }
    }
}
