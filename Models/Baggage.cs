using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meridian_Terminal.Models
{
    public class Baggage
    {
        public string BaggageId { get; private set; }
        public Passenger Passenger { get; private set; }
        
        public double WeightKg { get; private set; }

        public Baggage(string baggageId, Passenger passenger, double weight )
        {
            if (string.IsNullOrWhiteSpace(baggageId))
                throw new ArgumentException("Baggage ID can't be empty");
            if (weight <= 0)
                throw new ArgumentException("Baggage weight must be greater than zero");

            BaggageId = baggageId;
            Passenger = passenger ?? throw new ArgumentNullException(nameof(passenger));
            WeightKg = weight;
        }
    }
}
