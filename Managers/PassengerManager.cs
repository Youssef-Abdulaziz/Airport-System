using Meridian_Terminal.Enums;
using Meridian_Terminal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meridian_Terminal.Managers
{
    public class PassengerManager
    {
        private readonly List<Passenger> _passengers;

        public PassengerManager(List<Passenger> passengers)
        {
            _passengers = passengers ?? throw new ArgumentNullException(nameof(passengers));
        }

        public Passenger RegisterPassenger(string passengerId, string name, PassengerCategory category,
                                            Flight flight, Flight? connectingFlight = null)
        {
            if (_passengers.Any(p => p.PassengerId == passengerId))
                throw new InvalidOperationException($"Passenger {passengerId} is already registered.");

            var passenger = new Passenger(passengerId, name, category, flight, connectingFlight);
            _passengers.Add(passenger);
            return passenger;
        }

        public Passenger GetPassenger(string passengerId)
        {
            return _passengers.FirstOrDefault(p => p.PassengerId == passengerId)
                ?? throw new InvalidOperationException($"Passenger {passengerId} not found.");
        }
    }
}
