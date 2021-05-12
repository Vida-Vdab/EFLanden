using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Entities
{
    public class Taal
    {
        public string ISOTaalCode { get; set; }
        public string NaamNL { get; set; }
        public string NaamTaal { get; set; }


        public virtual ICollection<LandTaal> LandenTalen { get; set; }
    }
}
