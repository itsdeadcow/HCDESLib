using System;
using RailCargo.HCCM.Entities;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;

namespace RailCargo.HCCM.Events
{
    public class EventTrainArrivalInEndDestination : Event
    {
        private readonly EntityTrain _train;

        public EventTrainArrivalInEndDestination(EventType type, ControlUnit parentControlUnit, EntityTrain train) :
            base(type, parentControlUnit)
        {
            _train = train;
        }

        protected override void StateChange(DateTime time, ISimulationEngine simEngine)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "Event_Train_Arrival_In_End_Destination";
        }

        public override Event Clone()
        {
            throw new NotImplementedException();
        }

        public override Entity[] AffectedEntities
        {
            get { return new Entity[] { _train }; }
        }
    }
}