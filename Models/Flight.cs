using Meridian_Terminal.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meridian_Terminal.Models
{
    public class Flight
    {
        public string FlightNumber { get; private set; }
        public FlightType Type { get; private set; }
        public DateTime SchedualArrival { get; private set; }
        public DateTime SchedualDeparture { get; private set; }
        public int SeatCapacity { get; private set; }
        public FlightStatus Status { get; private set; }

        public Gate? AssignedGate { get; private set; } // Hving it null because a flight might not have a gate (yet)

        // Now I will make two lists that have the passengers who are confirmed and who are on a standby
        private readonly List<Passenger> _confirmedPassengers;
        private readonly List<Passenger> _standbyList;

        public Flight(string flightNumber, FlightType type, DateTime schedualArrival, DateTime schedualDeparture, int seatCapacity)
        {

            //Quick checkup on the inputs given here
            if (string.IsNullOrWhiteSpace(flightNumber))
                throw new ArgumentException("Flight Number Can't be empty");
            if (seatCapacity <= 0)
                throw new ArgumentException("Seat Capacity must be greater than zero");
            if (schedualDeparture <= schedualArrival)
                throw new ArgumentException("Schedual departure must be after schedual arraival");

            FlightNumber = flightNumber;
            Type = type;
            SchedualArrival = schedualArrival;
            SchedualDeparture = schedualDeparture;
            SeatCapacity = seatCapacity;
            Status = FlightStatus.Schedualed;
            AssignedGate = null;
            
            _confirmedPassengers = new List<Passenger>();
            _standbyList = new List<Passenger>();
        }
        // I'm letting the code outside of this class only read these two lists and not be able to edit thme. I think it is one of the ways to apply encapsulation
        public IReadOnlyList<Passenger> ConfirmedPassengers => _confirmedPassengers.AsReadOnly();
        public IReadOnlyList<Passenger> StandbyList => _standbyList.AsReadOnly();

        public int ConfirmedCound => _confirmedPassengers.Count;
        public bool HasAvailableSeat => ConfirmedCound < SeatCapacity;

        // Here we will assign a gate to the flight
        public void AssignGate(Gate gate) 
        {
            AssignedGate = gate ?? throw new ArgumentNullException(nameof(gate)); // if the gate was not null then assign it, if it was null then i will throw the exception
        }
        // Updating the Status with a new one 
        public void UpdateStatus(FlightStatus newStatus) {
            Status = newStatus;
        }

        // I will make a few functions related to the two lists in case being used in the service class such as adding/removing/ and adding the standby passengers who been waiting
        public void AddConfirmedPassenger(Passenger passenger) 
        {
            _confirmedPassengers.Add(passenger);
        }
        public void AddToStandby(Passenger passenger) 
        {
            _standbyList.Add(passenger);
        }
        // This fun will deal with if the passenger cancelled his flight and gets removed from the confirmed list so there can be a place for the standby passengers
        public void RemoveConfirmedPassenger(Passenger passenger) 
        { 
            _confirmedPassengers.Remove(passenger);
        }
        // This function will promotw the standby passenger to the confirmed list
        public Passenger? PromoteEarliestStandby() 
        {
            //checking if there is any pasengers in the list first
            if(_standbyList.Count == 0)
                return null;

            Passenger nextInLine = _standbyList[0];
            _standbyList.RemoveAt(0);
            _confirmedPassengers.Add(nextInLine);
            return nextInLine;
        }
    }
}
