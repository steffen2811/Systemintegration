namespace GetDashboardData.Models
{
    public class RoomTempData
    {
        public decimal temperature { get; set; }
        public DateTime? updated { get; set; }

        public RoomTempData(decimal temperature, DateTime? updated)
        {
            this.temperature = temperature;
            this.updated = updated;
        }
    }
}
