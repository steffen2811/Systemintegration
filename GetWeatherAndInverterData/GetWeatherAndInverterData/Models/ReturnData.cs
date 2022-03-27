namespace GetWeatherAndInverterData.Models
{
    public class ReturnData
    {
        public WeatherData weatherData { get; set; }
        public Inverter inverter { get; set; }

        public ReturnData(WeatherData weatherData, Inverter inverter)
        {
            this.weatherData = weatherData;
            this.inverter = inverter;
        }
    }
}
