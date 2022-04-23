namespace GetDashboardData.Models
{
    public class ReturnData
    {
        public WeatherData weatherData { get; set; }
        public InverterData inverterData { get; set; }
        public RoomTempData roomTempData { get; set; }
        public EnergyPrice energyPrice { get; set; }

        public ReturnData(WeatherData weatherData, InverterData inverterData, RoomTempData roomTempData, EnergyPrice energyPrice)
        {
            this.weatherData = weatherData;
            this.inverterData = inverterData;
            this.roomTempData = roomTempData;
            this.energyPrice = energyPrice;
        }
    }
}
