using GetWeatherAndInverterData.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceReference1;
using System.Net;
using System.IO;
using GetWeatherAndInverterData.backend;

namespace GetWeatherAndInverterData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherAndInverterDataController : ControllerBase
    {
        [HttpGet]
        public ReturnData Get()
        {
            WeatherData? weatherData = TimedBackgroundService.GetWeatherAndInverterDataFromBackend();
            Inverter? inverterData = TimedBackgroundService.GetInverterDataFromBackend();
            var response = new ReturnData(weatherData, inverterData);
            return response;
        }
    }
}
