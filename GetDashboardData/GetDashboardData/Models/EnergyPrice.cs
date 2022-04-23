namespace GetDashboardData.Models
{
    public class EnergyPrice
    {
        public double? energyPrice { get; set; }
        public DateTime? updated { get; set; }

        public EnergyPrice(double? energyPrice, DateTime? updated)
        {
            this.energyPrice = energyPrice;
            this.updated = updated;
        }
    }
}
