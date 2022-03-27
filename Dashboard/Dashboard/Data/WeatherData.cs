namespace Dashboard.Data
{
    public class WeatherData
    {
        public string location { get; set; }
        public float? cloudCover { get; set; }
        public float? temperature { get; set; }
        public bool sunAvaliable { get; set; }
        public DateTime? updated { get; set; }
        public List<WeatherForecast> weatherForecast { get; set; }
    }
}