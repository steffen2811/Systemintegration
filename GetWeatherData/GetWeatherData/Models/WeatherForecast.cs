namespace GetWeatherData.Models
{
    public class WeatherForecast
    {
        public DateTime? date { get; set; }
        public float? cloudCover { get; set; }
        public float? temperature { get; set; }

        public WeatherForecast(DateTime? date, float? cloudCover, float? temperature)
        {
            this.date = date;
            this.cloudCover = cloudCover;
            this.temperature = temperature;
        }
    }
}
