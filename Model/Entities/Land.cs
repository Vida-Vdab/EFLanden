using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Entities
{
    public class Land
    {
        public string ISOLandCode { get; set; }
        public string NISLandCode { get; set; }
        public string Naam { get; set; }
        public int AantalInwoners { get; set; }
        public float Oppervlakte { get; set; }
        public byte[] Aangepast { get; set; }


        public virtual ICollection<Stad> Steden { get; set; }
        public virtual ICollection<LandTaal> LandenTalen { get; set; }
    }
}
