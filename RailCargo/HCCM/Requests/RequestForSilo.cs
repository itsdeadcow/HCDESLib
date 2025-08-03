using System;
using RailCargo.HCCM.Entities;
using SimulationCore.HCCMElements;
namespace RailCargo.HCCM.Requests
{
    public class RequestForSilo : ActivityRequest
    {
        public RequestForSilo(string activity, Entity entity, DateTime time) : base(activity, entity, time)
        {
        }
    }
}