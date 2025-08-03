using System;
using SimulationCore.HCCMElements;
namespace RailCargo.HCCM.Requests
{
    public class RequestCheckSiloStatus : ActivityRequest
    {
        public RequestCheckSiloStatus(string activity, Entity[] origin, DateTime time) : base(activity, origin, time)
        {
        }

        public RequestCheckSiloStatus(string activity, Entity origin, DateTime time) : base(activity, origin, time)
        {
        }
    }
}