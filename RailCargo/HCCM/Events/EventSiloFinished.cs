using System;
using RailCargo.HCCM.Entities;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;

namespace RailCargo.HCCM.Events
{
    public class EventSiloFinished : Event
    {
        private readonly EntitySilo _silo;

        public EventSiloFinished(EventType type, ControlUnit parentControlUnit, EntitySilo silo) : base(type, parentControlUnit)
        {
            _silo = silo;
        }

        protected override void StateChange(DateTime time, ISimulationEngine simEngine)
        {
            //_silo.Train.StopCurrentActivities(time, simEngine);
        }

        public override string ToString()
        {
            return "Event_Silo_Finished";
        }

        public override Event Clone()
        {
            throw new NotImplementedException();
        }

        public override Entity[] AffectedEntities { get { return new Entity[] { _silo }; } }
    }
}