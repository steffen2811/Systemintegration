namespace GetDashboardData.Kafka
{
    public class ManageCorrelationIdentifiers
    {
        private static List<Tuple<string, long>> correl_ids = new List<Tuple<string, long>>();

        public static void AddCorrelationIdentifier(string correl_id, long dateTimeNow)
        {
            correl_ids.Add(new Tuple<string, long>(correl_id, dateTimeNow));
        }

        public static bool CheckCorrelationIdentifier(string correl_id)
        {
            if (correl_ids.Any(m => m.Item1 == correl_id))
            {
                correl_ids.RemoveAll(m => m.Item1 == correl_id);
                return true;
            }
            return false;
        }

        public static void DeleteOldCorrelationIdentifier()
        {
            correl_ids.RemoveAll(m => m.Item2 == DateTime.Now.AddMinutes(-5).Ticks);
        }
    }
}
