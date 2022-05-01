using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kafka_producer_energy_price.Models
{
    internal class EnergyPrice
    { 
        public double? energyPrice { get; set; }
        public DateTime? updated { get; set; }
    }
}
