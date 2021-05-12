using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model.Entities;
using Model.Repositories;

namespace Model.Services
{
    public class LandService
    {
        readonly private EFLandenContext context;
        
        public LandService(EFLandenContext context)
        {
            this.context = context;
        }
        public IEnumerable<Stad> GetStedenVoorLand(string landcode)
        {
            //throw new NotImplementedException();
            return context.Steden.Where(b => b.ISOLandCode == landcode).ToList();
        }
        public Stad GetSteden(int id)
        {
            //throw new NotImplementedException();
            if (id == 0)
            {
                throw new NotImplementedException();
            }
            return context.Steden.FirstOrDefault(b => b.StadId == id);
        }
        public void ToevoegenStad(Stad stad)
        {
            //throw new NotImplementedException();
            if (String.IsNullOrEmpty(stad.ISOLandCode))
            {
                throw new NotImplementedException();
            }
            context.Steden.Add(stad);
        }

        public IEnumerable<LandTaal> GetTaalVanLandTaal(string taalcode)
        {
            //throw new NotImplementedException();
            if (String.IsNullOrEmpty(taalcode))
            {
                throw new NotImplementedException();
            }

            return context.LandenTalen.Where(b => b.TaalCode == taalcode).ToList();
        }

        public IEnumerable<LandTaal> GetLandVanLandTaal(string landcode)
        {
            //throw new NotImplementedException();
            if (String.IsNullOrEmpty(landcode))
            {
                throw new NotImplementedException();
            }

            return context.LandenTalen.Where(b => b.LandCode == landcode).ToList();
        }
    }
}
