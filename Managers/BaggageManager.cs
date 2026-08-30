using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meridian_Terminal.Enums;
using Meridian_Terminal.Models;


namespace Meridian_Terminal.Managers
{
    public class BaggageManager
    {
        private readonly List<Baggage> _baggageRecords;

        private readonly Dictionary<PassengerCategory, double> _allowances = new Dictionary<PassengerCategory, double>
        {
            { PassengerCategory.Standard, 30 },
            { PassengerCategory.VIP, 50 },
            { PassengerCategory.ReducedMobility, 40 }
        };

        public BaggageManager(List<Baggage> baggageRecords) 
        {
            _baggageRecords = baggageRecords ?? throw new ArgumentNullException(nameof(baggageRecords));
        }

        public Baggage RegisterBaggage(Passenger passenger, double weightKg) 
        {
            Flight flight = passenger.Flight;
            if (flight.Status == FlightStatus.Departed || flight.Status == FlightStatus.Cancelled)
                throw new InvalidOperationException($"Can't register a baggage to Flight {flight.FlightNumber} becasue its status is {flight.Status} ");

            double currentTotal = GetTotalWeight(passenger);
            double allowance = _allowances[passenger.Category];
            double newTotal = currentTotal + weightKg;

            if (newTotal > allowance)
                throw new InvalidOperationException(
                    $"This bag would bring the passenger's total checked baggage to {newTotal}kg" +
                    $"exceeding the {allowance}kg allowance for {passenger.Category} passengers");

            var baggage = new Baggage(Guid.NewGuid().ToString(), passenger, weightKg);
            _baggageRecords.Add(baggage);
            return baggage;
        }
        public double GetTotalWeight(Passenger passenger) 
        {
            return _baggageRecords
                .Where(b => b.Passenger == passenger)
                .Sum(b => b.WeightKg);
        }
    }
}
