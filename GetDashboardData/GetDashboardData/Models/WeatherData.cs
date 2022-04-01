namespace GetDashboardData.Models
{
    public class WeatherData
    {
        public string location { get; set; }
        public float? cloudCover { get; set; }
        public float? temperature { get; set; }
        public bool sunAvaliable { get; set; }
        public DateTime? updated { get; set; }
        public List<WeatherForecast>? weatherForecast { get; set; }

        public WeatherData(string location, float? cloudCover, float? temperature, bool sunAvaliable, List<WeatherForecast>? weatherForecast, DateTime? updated)
        {
            this.location = location;
            this.cloudCover = cloudCover;
            this.temperature = temperature;
            this.sunAvaliable = sunAvaliable;
            this.weatherForecast = weatherForecast;
            this.updated = updated;
        }
    }
}
