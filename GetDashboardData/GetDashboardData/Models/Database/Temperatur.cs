using System;
using System.Collections.Generic;

namespace GetDashboardData.Models.Database
{
    public partial class Temperatur
    {
        public DateTime Dato { get; set; }
        public TimeSpan Tidspunkt { get; set; }
        public decimal Grader { get; set; }
    }
}
