using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meridian_Terminal.Enums;
using Meridian_Terminal.Models;
namespace Meridian_Terminal.Managers
{
    public class FlightManager
    {
        private readonly List<Flight> _flights;
        private readonly List<Gate> _gates;

        public FlightManager(List<Flight> flights, List<Gate> gates) 
        {
            _flights = flights ?? throw new ArgumentNullException(nameof(flights));
            _gates = gates ?? throw new ArgumentNullException(nameof(gates));
        }

        // Now i wwill create some of the functions that will manage the flights

        public Flight RegiserFlight(string flightNumber, FlightType type, DateTime scheduledArrival, DateTime scheduledDepature, int seatCapacity) 
        {
            // cheking up the flights list if we have a flight number that is the same as the one entered for registeration
            if (_flights.Any(f => f.FlightNumber == flightNumber))
                throw new InvalidOperationException($"Flightwith number {flightNumber} is already registered");
            var flight = new Flight(flightNumber, type, scheduledArrival, scheduledDepature, seatCapacity);
            _flights.Add(flight);

            return flight;
        }
        public void AssignGate(string flightNumber, string gateNumber) 
        {
            //First i wll have to chekc if i already have the flight and the gate that is going to be assigned to flight
            Flight flight = _flights.FirstOrDefault(f => f.FlightNumber == flightNumber)
                ?? throw new InvalidOperationException($"Flight {flightNumber} not found!!");
            Gate gate = _gates.FirstOrDefault(g => g.GateNumber == gateNumber)
                ?? throw new InvalidOperationException($"Gate with {gateNumber} not found!!");

            // Here I will check if the gate supports international flights
            if (flight.Type == FlightType.International && !gate.SupportInternational)
                throw new InvalidOperationException($"Gate {gateNumber} does not support international flights");

            // here is going to be the solution for the conflict which is a big problem that needs to be fixed
            bool conflict = _flights.Any(f =>
            f.FlightNumber != flightNumber &&
            f.AssignedGate != null &&
            f.AssignedGate.GateNumber == gateNumber &&
            f.ScheduledArrival < flight.ScheduledDeparture &&
            flight.ScheduledArrival < f.ScheduledDeparture
            );

            if (conflict)
                throw new InvalidOperationException($"Gate {gateNumber} is laready taken by another flight during an overlapping time window");

            flight.AssignGate(gate);
        }

        // updating flight status
        public void UpdateFlightStatus(string flightNumber, FlightStatus newStatus) 
        {
            Flight flight = _flights.FirstOrDefault(f => f.FlightNumber == flightNumber)
                    ?? throw new InvalidOperationException($"Flight {flightNumber} not found");
            flight.UpdateStatus(newStatus);
        }

        public Flight GetFlight(string flightNumber) 
        {
            return _flights.FirstOrDefault(f => f.FlightNumber == flightNumber)
                ?? throw new InvalidOperationException($"Flight {flightNumber} not found");
        }

    }
}
