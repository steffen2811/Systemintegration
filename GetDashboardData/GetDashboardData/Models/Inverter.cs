namespace GetDashboardData.Models
{
    public class Inverter
    {
        public bool powerProductionLastHour { get; set; }
        public int powerProduction { get; set; }
        public DateTime? updated { get; set; }

        public Inverter(bool powerProductionLastHour, int powerProduction, DateTime? updated)
        {
            this.powerProductionLastHour = powerProductionLastHour;
            this.powerProduction = powerProduction;
            this.updated = updated;
        }
    }
}
