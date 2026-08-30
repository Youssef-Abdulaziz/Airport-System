using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meridian_Terminal.Models
{
    public class StaffAssignment
    {
        public Flight Flight { get; private set; }
        public double Hours { get; private set; }

        public StaffAssignment(Flight flight, double hours)
        {
            if (hours <= 0)
                throw new ArgumentException("Assigned hours must be greater than zero.");

            Flight = flight ?? throw new ArgumentNullException(nameof(flight));
            Hours = hours;
        }
    }
}
