using System;
using System.Collections.Generic;
using System.Text;


namespace Model.Entities
{
    public class LandTaal
    {
        public string LandCode { get; set; }
        public string TaalCode { get; set; }


        public virtual Land Land { get; set; }
        public virtual Taal Taal { get; set; }
    }
}
