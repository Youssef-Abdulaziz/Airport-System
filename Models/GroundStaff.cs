using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Meridian_Terminal.Models
{
    public class GroundStaff
    {
        public string StaffId { get; private set; }
        public string StaffName {  get; private set; }

        private readonly List<StaffAssignment> _assignments;

        public GroundStaff(string staffId, string staffName) 
        {
            if (string.IsNullOrWhiteSpace(staffId))
                throw new ArgumentException("Staff ID cannot be empty.");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Staff name cannot be empty.");

            StaffId = staffId;
            StaffName = staffName;
            _assignments = new List<StaffAssignment>();
        }
        public IReadOnlyList<StaffAssignment> Assignments => _assignments.AsReadOnly();

        public double TotalHours
        {
            get
            {
                double total = 0;
                foreach (var a in _assignments)
                    total += a.Hours;
                return total;
            }
        }

        public void AddAssignment(StaffAssignment assignment)
        {
            _assignments.Add(assignment ?? throw new ArgumentNullException(nameof(assignment)));
        }
    }
}
