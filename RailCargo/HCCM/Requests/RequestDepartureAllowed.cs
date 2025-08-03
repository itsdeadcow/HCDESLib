using System;
using SimulationCore.HCCMElements;
namespace RailCargo.HCCM.Requests
{
    public class EventDepartureAllowed : ActivityRequest
    {
        public EventDepartureAllowed(string activity, Entity[] origin, DateTime time) : base(activity, origin, time)
        {
        }

        public EventDepartureAllowed(string activity, Entity origin, DateTime time) : base(activity, origin, time)
        {
        }
    }
}