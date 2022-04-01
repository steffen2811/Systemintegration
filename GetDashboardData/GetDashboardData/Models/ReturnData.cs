namespace GetDashboardData.Models
{
    public class ReturnData
    {
        public WeatherData weatherData { get; set; }
        public InverterData inverterData { get; set; }
        public RoomTempData roomTempData { get; set; }

        public ReturnData(WeatherData weatherData, InverterData inverterData, RoomTempData roomTempData)
        {
            this.weatherData = weatherData;
            this.inverterData = inverterData;
            this.roomTempData = roomTempData;
        }
    }
}
