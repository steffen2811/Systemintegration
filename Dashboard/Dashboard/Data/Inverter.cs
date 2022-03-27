namespace Dashboard.Data
{
    public class Inverter
    {
        public bool powerProductionLastHour { get; set; }
        public int powerProduction { get; set; }
        public DateTime? updated { get; set; }
    }
}
