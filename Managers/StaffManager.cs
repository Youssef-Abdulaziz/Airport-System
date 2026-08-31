using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meridian_Terminal.Enums;
using Meridian_Terminal.Models;

namespace Meridian_Terminal.Managers
{
    public class StaffManager
    {
        private const double MaxDutyHours = 8;
        private readonly List<GroundStaff> _staff;
        public StaffManager(List<GroundStaff> staff )
        {
            _staff = staff ?? throw new ArgumentNullException(nameof(staff));
        }

        //Registering a staff
        public GroundStaff RegisterStaff(string staffId, string staffName) 
        {
            if (_staff.Any(s => s.StaffId == staffId))
                throw new InvalidOperationException($"Staff member {staffId} is already registerd");

            var staff = new GroundStaff(staffId, staffName);
            _staff.Add(staff);
            return staff;
        }
        public void AssignStaffToFlight(string staffId, Flight flight, double hours) 
        {
            if (flight.Status == FlightStatus.Departed || flight.Status == FlightStatus.Cancelled)
                throw new InvalidOperationException(
                    $"Cannot assign staff to flight {flight.FlightNumber} because its status is {flight.Status}.");

            GroundStaff staff = _staff.FirstOrDefault(s => s.StaffId == staffId)
                ?? throw new InvalidOperationException($"Staff with ID: {staffId} not found");

            double newTotal = staff.TotalHours + hours;

            if (newTotal > MaxDutyHours)
                throw new InvalidOperationException(
                    $"Assigning {hours}h to {staff.StaffName} would bring their total to {newTotal}h, " +
                    $"exceeding the {MaxDutyHours}h maximum duty hours per shift.");

            var assginment = new StaffAssignment(flight, hours);
            staff.AddAssignment(assginment);

        }
        public double GetTotalHours(string staffId)
        {
            GroundStaff staff = _staff.FirstOrDefault(s => s.StaffId == staffId)
                ?? throw new InvalidOperationException($"Staff member {staffId} not found.");

            return staff.TotalHours;
        }
    }
}
