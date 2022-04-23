namespace GetDashboardData.Models
{
    public class InverterData
    {
        public bool powerProductionLastHour { get; set; }
        public int powerProduction { get; set; }
        public DateTime? updated { get; set; }

        public InverterData(bool powerProductionLastHour, int powerProduction, DateTime? updated)
        {
            this.powerProductionLastHour = powerProductionLastHour;
            this.powerProduction = powerProduction;
            this.updated = updated;
        }
    }
}
