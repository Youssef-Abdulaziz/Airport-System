using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meridian_Terminal.Models;
using Meridian_Terminal.Enums;


namespace Meridian_Terminal.Managers
{
    public  class BookingManager
    {
        private const int MinConnectionTime = 45;
        private const int StandbyCapacity = 10;

        public void BookPassenger(Flight flight, Passenger passenger) 
        {
            if (flight.Status == FlightStatus.Departed || flight.Status == FlightStatus.Cancelled)
                throw new InvalidOperationException($"Can't book on a flight {flight.FlightNumber} becasue its status is {flight.Status}");

            if (flight.HasAvailableSeat)
            {
                flight.AddConfirmedPassenger(passenger);
                passenger.SetBookingStatus(BookingStatus.Confirmed);
            }
            else 
            {
                if (flight.StandbyList.Count >= StandbyCapacity)
                    throw new InvalidOperationException($"Standby list for flight {flight.FlightNumber} is full (limit {StandbyCapacity})");
                
                
                flight.AddToStandby(passenger);
                passenger.SetBookingStatus(BookingStatus.Standby);
            }
        }

        public Passenger? CancelBooking(Flight flight, Passenger passenger) 
        {
            if (passenger.BookingStatus != BookingStatus.Confirmed)
                throw new InvalidOperationException($"Passenger {passenger.PassengerId} does not have a confirmed booking to cancel");
            
            flight.RemoveConfirmedPassenger(passenger);
            passenger.SetBookingStatus(BookingStatus.Cancelled);

            Passenger? promoted = flight.PromoteEarliestStandby();
            if(promoted != null)
                promoted.SetBookingStatus(BookingStatus.Confirmed);
            return promoted;
        }

        public (bool elibible, string reason) CheckBoardingEligibility(Passenger passenger, Flight nextFlight) 
        {
            if (nextFlight.Status == FlightStatus.Departed || nextFlight.Status == FlightStatus.Cancelled)
                return (false, $"Flight {nextFlight.FlightNumber} status is {nextFlight.Status}; boarding not allowed.");

            if (passenger.BookingStatus != BookingStatus.Confirmed)
                return (false, $"Passenger {passenger.PassengerId} does not have a confirmed booking on this flight.");

            if (!passenger.IsConnectingPassenger)
                return (true, "No connection to verify; passenger is eligible to board.");

            Flight arrivingFlight = passenger.ConnectingFlight!;
            TimeSpan gap = nextFlight.ScheduledDeparture - arrivingFlight.ScheduledArrival;

            if (gap.TotalMinutes < MinConnectionTime)
            {
                int remaining = (int)gap.TotalMinutes;
                return (false,
                    $"Only {remaining} minutes remain since the connecting flight's arrival; " +
                    $"the minimum connection time is {MinConnectionTime} minutes.");
            }

            return (true, "Sufficient connection time; passenger is eligible to board.");
        }

    }
}
