using GetWeatherData.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceReference1;
using System.Net;
using System.IO;
using GetWeatherData.backend;

namespace GetWeatherData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherForecastController : ControllerBase
    {
        [HttpGet]
        public ReturnData Get()
        {
            WeatherData? weatherData = TimedBackgroundService.GetWeatherDataFromBackend();
            Inverter? inverterData = TimedBackgroundService.GetInverterDataFromBackend();
            var response = new ReturnData(weatherData, inverterData);
            return response;
        }
    }
}
