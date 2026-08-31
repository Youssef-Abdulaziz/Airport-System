
using Meridian_Terminal.Enums;
using Meridian_Terminal.Managers;
using Meridian_Terminal.Models;
using System.Diagnostics.Metrics;
using System.Linq;

namespace Meridian_Terminal 
{
    public class program
    {

        private static readonly List<Flight> _flights = new List<Flight>();
        private static readonly List<Gate> _gates = new List<Gate>();
        private static readonly List<Passenger> _passengers = new List<Passenger>();
        private static readonly List<Baggage> _baggageRecords = new List<Baggage>();
        private static readonly List<GroundStaff> _staff = new List<GroundStaff>();

        private static readonly FlightManager _flightManager = new FlightManager(_flights, _gates);
        private static readonly BookingManager _bookingManager = new BookingManager();
        private static readonly BaggageManager _baggageManager = new BaggageManager(_baggageRecords);
        private static readonly StaffManager _staffManager = new StaffManager(_staff);
        private static readonly PassengerManager _passengerManager = new PassengerManager(_passengers);

        public static void Main(string[] args)
        {
            SeedGates();

            bool running = true;
            while (running)
            {
                PrintMenu();
                string? choice = Console.ReadLine(); // reading user input variable

                try
                {
                    switch (choice)
                    {
                        case "1": RegisterFlight(); break;
                        case "2": AssignGate(); break;
                        case "3": RegisterPassenger(); break;
                        case "4": CheckBoardingEligibility(); break;
                        case "5": RegisterBaggage(); break;
                        case "6": ManageBookings(); break;
                        case "7": AssignStaff(); break;
                        case "8": running = false; break;
                        default: Console.WriteLine("Invalid option, try again."); break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Operation failed: {ex.Message}");
                }
                Console.WriteLine();
            }
            Console.WriteLine("Session ended");
        }
        private static void PrintMenu()
        {
            Console.WriteLine("=== Meridian Terminal — Ground Operations System ===");
            Console.WriteLine("1. Register Flight");
            Console.WriteLine("2. Assign Gate");
            Console.WriteLine("3. Register Passenger");
            Console.WriteLine("4. Check Boarding Eligibility");
            Console.WriteLine("5. Register Baggage");
            Console.WriteLine("6. Manage Bookings & Standby");
            Console.WriteLine("7. Assign Staff");
            Console.WriteLine("8. Exit");
            Console.Write("Select an option: ");
        }
        private static void SeedGates()
        {
            _gates.Add(new Gate("G1", true));
            _gates.Add(new Gate("G2", false));
            _gates.Add(new Gate("G3", true));
        }
        private static void RegisterFlight()
        {
            Console.WriteLine("Enter Flight Number: ");
            string flightNumber = Console.ReadLine() ?? "";

            Console.WriteLine("Enter Flight Type(1 = Domestic, 2 = International):");
            string typeInput = Console.ReadLine() ?? "";
            FlightType type = typeInput == "2" ? FlightType.International : FlightType.Domestic;

            Console.Write("Enter Scheduled Arrival (yyyy-MM-dd HH:mm): ");
            DateTime arrival = DateTime.Parse(Console.ReadLine() ?? "");

            Console.Write("Enter Scheduled Departure (yyyy-MM-dd HH:mm): ");
            DateTime departure = DateTime.Parse(Console.ReadLine() ?? "");

            Console.Write("Enter Seat Capacity: ");
            int capacity = int.Parse(Console.ReadLine() ?? "");

            Flight flight = _flightManager.RegiserFlight(flightNumber, type, arrival, departure, capacity);

            Console.WriteLine($"Result : Flight REGISTERED");
            Console.WriteLine($"Flight {flight.FlightNumber} ({flight.Type}) scheduled {flight.ScheduledArrival} → {flight.ScheduledDeparture}, capacity {flight.SeatCapacity}.");
        }

        // Assigning a GATE
        private static void AssignGate()
        {
            Console.WriteLine("Enter Flight Number: ");
            string flightNumber = Console.ReadLine() ?? "";

            Console.WriteLine("Enter Gate Number: ");
            string gateNumber = Console.ReadLine() ?? "";

            _flightManager.AssignGate(flightNumber, gateNumber);

            Console.WriteLine($"Result: GATE ASSIGNED");
            Console.WriteLine($"Flight {flightNumber} has been assigned to gate {gateNumber}.");
        }

        // Registering a PASSENGER 
        private static void RegisterPassenger()
        {
            Console.Write("Enter Passenger ID: ");
            string passengerId = Console.ReadLine() ?? "";

            Console.Write("Enter Passenger Name: ");
            string name = Console.ReadLine() ?? "";

            Console.Write("Enter Category (1 = Standard, 2 = VIP, 3 = Reduced Mobility): ");
            string categoryInput = Console.ReadLine() ?? "";
            PassengerCategory category = categoryInput switch
            {
                "2" => PassengerCategory.VIP,
                "3" => PassengerCategory.ReducedMobility,
                _ => PassengerCategory.Standard
            };

            Console.Write("Enter Flight Number: ");
            string flightNumber = Console.ReadLine() ?? "";
            Flight flight = _flightManager.GetFlight(flightNumber);

            Console.Write("Is this passenger connecting from an earlier flight? (y/n): ");
            string connectingAnswer = Console.ReadLine() ?? "";

            Flight? connectingFlight = null;
            if (connectingAnswer.Trim().ToLower() == "y")
            {
                Console.Write("Enter Connecting (Arrival) Flight Number: ");
                string connectingFlightNumber = Console.ReadLine() ?? "";
                connectingFlight = _flightManager.GetFlight(connectingFlightNumber);
            }

            Passenger passenger = _passengerManager.RegisterPassenger(passengerId, name, category, flight, connectingFlight);

            Console.WriteLine("Result: PASSENGER REGISTERED");
            Console.WriteLine($"Passenger {passenger.PassengerId} ({passenger.Category}) registered on flight {flight.FlightNumber}.");
        }

        //
        private static void CheckBoardingEligibility()
        {
            Console.Write("Enter Passenger ID: ");
            string passengerId = Console.ReadLine() ?? "";
            Passenger passenger = _passengerManager.GetPassenger(passengerId);

            Console.Write("Enter Next Flight Number: ");
            string nextFlightNumber = Console.ReadLine() ?? "";
            Flight nextFlight = _flightManager.GetFlight(nextFlightNumber);

            var (eligible, reason) = _bookingManager.CheckBoardingEligibility(passenger, nextFlight);

            if (eligible)
            {
                Console.WriteLine("Result: BOARDING ALLOWED");
                Console.WriteLine($"Reason: {reason}");
            }
            else
            {
                Console.WriteLine("Result: BOARDING DENIED");
                Console.WriteLine($"Reason: {reason}");
            }
        }

        // Registering BAGGAGE
        private static void RegisterBaggage()
        {
            Console.Write("Enter Passenger ID: ");
            string passengerId = Console.ReadLine() ?? "";
            Passenger passenger = _passengerManager.GetPassenger(passengerId);

            Console.Write("Enter Baggage Weight (kg): ");
            double weight = double.Parse(Console.ReadLine() ?? "");

            Baggage baggage = _baggageManager.RegisterBaggage(passenger, weight);
            double total = _baggageManager.GetTotalWeight(passenger);

            Console.WriteLine("Result: BAGGAGE REGISTERED");
            Console.WriteLine($"Bag {baggage.BaggageId} ({baggage.WeightKg}kg) added for passenger {passenger.PassengerId}. Total checked baggage: {total}kg.");
        }

        // Managing BOOKINGS & STANDBY
        private static void ManageBookings()
        {
            Console.WriteLine("--- Manage Bookings & Standby ---");
            Console.WriteLine("1. Book/Confirm a Passenger");
            Console.WriteLine("2. Cancel a Confirmed Booking");
            Console.WriteLine("3. View Flight's Standby List");
            Console.Write("Select an option: ");
            string subChoice = Console.ReadLine() ?? "";

            switch (subChoice)
            {
                case "1": BookPassenger(); break;
                case "2": CancelBooking(); break;
                case "3": ViewStandbyList(); break;
                default: Console.WriteLine("Invalid option."); break;
            }
        }

        private static void BookPassenger()
        {
            Console.Write("Enter Passenger ID: ");
            string passengerId = Console.ReadLine() ?? "";
            Passenger passenger = _passengerManager.GetPassenger(passengerId);

            Flight flight = passenger.Flight;

            _bookingManager.BookPassenger(flight, passenger);

            Console.WriteLine("Result: BOOKING PROCESSED");
            Console.WriteLine($"Passenger {passenger.PassengerId} status is now {passenger.BookingStatus} on flight {flight.FlightNumber}.");
        }

        private static void CancelBooking()
        {
            Console.Write("Enter Passenger ID: ");
            string passengerId = Console.ReadLine() ?? "";
            Passenger passenger = _passengerManager.GetPassenger(passengerId);

            Flight flight = passenger.Flight;

            Passenger? promoted = _bookingManager.CancelBooking(flight, passenger);

            Console.WriteLine("Result: BOOKING CANCELLED");
            Console.WriteLine($"Passenger {passenger.PassengerId}'s booking on flight {flight.FlightNumber} has been cancelled.");

            if (promoted != null)
                Console.WriteLine($"Passenger {promoted.PassengerId} has been promoted from standby to a confirmed seat.");
        }

        private static void ViewStandbyList()
        {
            Console.Write("Enter Flight Number: ");
            string flightNumber = Console.ReadLine() ?? "";
            Flight flight = _flightManager.GetFlight(flightNumber);

            Console.WriteLine($"Standby list for flight {flight.FlightNumber}:");

            if (flight.StandbyList.Count == 0)
            {
                Console.WriteLine("No passengers on standby.");
                return;
            }

            int position = 1;
            foreach (Passenger p in flight.StandbyList)
            {
                Console.WriteLine($"{position}. {p.PassengerId} - {p.PassengerName} ({p.Category})");
                position++;
            }
        }

        // Assigning STAFF
        private static void AssignStaff()
        {
            Console.Write("Enter Staff ID: ");
            string staffId = Console.ReadLine() ?? "";

            if (!_staff.Any(s => s.StaffId == staffId))
            {
                Console.Write("New staff member. Enter Name: ");
                string name = Console.ReadLine() ?? "";
                _staffManager.RegisterStaff(staffId, name);
            }

            Console.Write("Enter Flight Number: ");
            string flightNumber = Console.ReadLine() ?? "";
            Flight flight = _flightManager.GetFlight(flightNumber);

            Console.Write("Enter Assignment Hours: ");
            double hours = double.Parse(Console.ReadLine() ?? "");

            _staffManager.AssignStaffToFlight(staffId, flight, hours);
            double total = _staffManager.GetTotalHours(staffId);

            Console.WriteLine("Result: STAFF ASSIGNED");
            Console.WriteLine($"Staff {staffId} assigned to flight {flight.FlightNumber} for {hours}h. Total duty hours: {total}h.");
        }

    }
}