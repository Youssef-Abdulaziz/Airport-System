using Meridian_Terminal.Enums;
using Meridian_Terminal.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meridian_Terminal.Models
{
    public class Passenger
    {
        public string PassengerId { get; private set; }
        public string PassengerName { get; private set; }
        public PassengerCategory Category { get; private set;}
        public Flight Flight { get; private set; }
        public Flight? ConnectingFlight { get; private set;} // might be null becasue not all passengers are going to connect
        public BookingStatus BookingStatus { get; private set; }

        public Passenger (string passengerId,string name, PassengerCategory category, Flight flight, Flight? connectingFlight = null ) 
        {
            // quick check ups on the id and the name inputs to not be empty
            if(string.IsNullOrWhiteSpace(passengerId))
                throw new ArgumentException("Passenger ID can't be empty");
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Passenger name can't be empty");

            PassengerId = passengerId;
            PassengerName = name;
            Category = category;
            Flight = flight ?? throw new ArgumentNullException(nameof(flight));
            ConnectingFlight = connectingFlight;
            BookingStatus = BookingStatus.NotBooked; // having the booking status cancelled becasue he is not booked yet 
        }
        public bool IsConnectingPassenger => ConnectingFlight != null;
        public void SetBookingStatus(BookingStatus status) 
        {
            BookingStatus = status;
        }
    }
}
