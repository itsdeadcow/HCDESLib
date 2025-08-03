using System;
using SimulationCore.HCCMElements;
namespace RailCargo.HCCM.Requests
{
    public class RequestForDeparture : ActivityRequest
    {
        public RequestForDeparture(string activity, Entity[] origin, DateTime time) : base(activity, origin, time)
        {
        }

        public RequestForDeparture(string activity, Entity origin, DateTime time) : base(activity, origin, time)
        {
        }
    }
}