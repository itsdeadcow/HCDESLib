using System;
using System.Linq;
using RailCargo.HCCM.ControlUnits;
using RailCargo.HCCM.Entities;
using RailCargo.HCCM.staticVariables;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;

namespace RailCargo.HCCM.Events
{
    public class EventWagonArrivalInEndDestination : Event
    {
        private readonly EntityWagon _wagon;

        public EventWagonArrivalInEndDestination(EventType type, ControlUnit parentControlUnit, EntityWagon wagon) : base(type, parentControlUnit)
        {
            _wagon = wagon;
        }

        protected override void StateChange(DateTime time, ISimulationEngine simEngine)
        {

            //CuBookingSystem.AllWagons.Where(x => x.Uuid == _wagon.Uuid).ToList().ForEach(x => x.arrived = true);
            
            if (_wagon.WagonId == 338578142980) Helper.Print("wtf");
            var calculatedTime = _wagon.EndTime;
            var real_time = time;
            _wagon.RealTime = time;
            _wagon.TimeDelta = (calculatedTime - real_time).TotalMinutes;
            _wagon.CurrentTrain = null;
            //Helper.Print("Arrival Time for " + _wagon.WagonId.ToString() +" " + _wagon.TimeDelta.ToString());
        }

        public override string ToString()
        {
            return "Event_Wagon_Arrival_In_Enddestination";
        }

        public override Event Clone()
        {
            throw new NotImplementedException();
        }

        public override Entity[] AffectedEntities
        {
            get { return new Entity[] { _wagon }; }
        }
    }
}