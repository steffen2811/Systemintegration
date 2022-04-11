namespace Dashboard.Data
{
    public class RoomTempData
    {
        public decimal temperature { get; set; }
        public DateTime? updated { get; set; }

        public RoomTempData(decimal temperature, DateTime? updated)
        {
            if (updated != null)
            {
                TimeZoneInfo cet = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
                var utcTime = TimeZoneInfo.ConvertTimeToUtc((DateTime)updated, cet);

                this.updated = utcTime.ToLocalTime();
            }
            else
                this.updated = updated;
            
        }
    }
}
