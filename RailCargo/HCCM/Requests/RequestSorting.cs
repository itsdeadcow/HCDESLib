using System;
using SimulationCore.HCCMElements;
namespace RailCargo.HCCM.Requests
{
    public class RequestSorting : ActivityRequest
    {
        private readonly DateTime _arrivalTime;

        public DateTime ArrivalTime => _arrivalTime;
        public bool Occured { get; set; }


        public RequestSorting(string activity, Entity[] origin, DateTime time, DateTime arrivalTime) : base(activity, origin, time)
        {
            _arrivalTime = arrivalTime;
        }

        public RequestSorting(string activity, Entity origin, DateTime time, DateTime arrivalTime) : base(activity, origin, time)
        {
            _arrivalTime = arrivalTime;
            Occured = false;
        }
    }
}