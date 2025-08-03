using System;
using System.Collections.Generic;
using RailCargo.HCCM.ControlUnits;

namespace RailCargo.HCCM.staticVariables
{
    public class AllShuntingYards
    {
        private static AllShuntingYards _instance;

        public static AllShuntingYards Instance
        {
            get {
                if (_instance == null)
                {
                    _instance = new AllShuntingYards();
                    return _instance;
                }
                return _instance;
            }
        }

        private Dictionary<string, CuShuntingYard> _sYards = new Dictionary<string, CuShuntingYard>();

        public CuShuntingYard GetYards(string key)
        {
            return _sYards[key];
        }

        public void SetYards(string key, CuShuntingYard cuShuntingYard)
        {
            _sYards.Add(key,cuShuntingYard);
        }
    }
}
