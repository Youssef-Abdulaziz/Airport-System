using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meridian_Terminal.Models
{
    public  class Gate
    {
        public string GateNumber { get; set; }
        public bool SupportInternational { get; set; }
        public Gate(string gateNumber, bool supportInternational)
        {
            if (string.IsNullOrWhiteSpace(gateNumber))
                throw new ArgumentNullException("Gate Number Can't be Empty");

            GateNumber = gateNumber;
            SupportInternational = supportInternational;
        }
    }
}
