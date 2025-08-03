using System;
using RailCargo.HCCM.Activities;
using RailCargo.HCCM.Entities;
using RailCargo.HCCM.staticVariables;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;

namespace RailCargo.HCCM.Events
{
    public class EventFreeSilo : Event
    {
        private readonly EntitySilo _silo;

        public EventFreeSilo(EventType type, ControlUnit parentControlUnit, EntitySilo silo) : base(type,
            parentControlUnit)
        {
            _silo = silo;
        }

        protected override void StateChange(DateTime time, ISimulationEngine simEngine)
        {
            ActivityShuntingWagons shuntingWagons =
                new ActivityShuntingWagons(ParentControlUnit, Constants.ActivityShuntingWagons, false, _silo);
            //_silo.AddActivity(shuntingWagons);
            SequentialEvents.Add(shuntingWagons.StartEvent);
            // var waitingForTrainSelectionSilo = new ActivityWaitingForTrainSelectionSilo(ParentControlUnit,
            //     Constants.ActivityWaitingForTrainSelectionSilo, false, _silo);
            // //_silo.AddActivity(waitingForTrainSelectionSilo);
            // // As the train is already selected, we can assign it instantly
            // SequentialEvents.Add(waitingForTrainSelectionSilo.EndEvent);
            
        }

        public override string ToString()
        {
            return "Event_Free_Silo";
        }

        public override Event Clone()
        {
            throw new NotImplementedException();
        }

        public override Entity[] AffectedEntities { get { return new Entity[] { _silo }; } }
    }
}