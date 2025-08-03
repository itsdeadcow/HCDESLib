using System;
using SimulationCore.HCCMElements;
namespace RailCargo.HCCM.Requests
{
    public class RequestTrainContinuation : ActivityRequest
    {
        public RequestTrainContinuation(string activity, Entity[] train, DateTime time) : base(activity, train, time)
        {
        }

        public RequestTrainContinuation(string activity, Entity train, DateTime time) : base(activity, train, time)
        {
        }
    }
}

