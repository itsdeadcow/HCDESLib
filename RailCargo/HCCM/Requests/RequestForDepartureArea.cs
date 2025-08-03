using System;
using SimulationCore.HCCMElements;
namespace RailCargo.HCCM.Requests
{
    public class RequestForDepartureArea: ActivityRequest
    {
        public RequestForDepartureArea(string activity, Entity[] origin, DateTime time) : base(activity, origin, time)
        {
        }

        public RequestForDepartureArea(string activity, Entity origin, DateTime time) : base(activity, origin, time)
        {
        }
    }
}