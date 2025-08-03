using System;
using RailCargo.HCCM.Entities;
using RailCargo.HCCM.staticVariables;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;

namespace RailCargo.HCCM.Activities
{
    public class ActivityWaitingForTrainSelectionSilo : Activity 
    {
        private readonly EntitySilo _silo;

        public ActivityWaitingForTrainSelectionSilo(ControlUnit parentControlUnit, string activityType, bool preEmptable, EntitySilo silo) : base(parentControlUnit, activityType, preEmptable)
        {
            _silo = silo;
        }

        public override void StateChangeStartEvent(DateTime time, ISimulationEngine simEngine)
        {
            //throw new NotImplementedException();
        }

        public override void StateChangeEndEvent(DateTime time, ISimulationEngine simEngine)
        {
            // ActivityShuntingWagons shuntingWagons =
            //     new ActivityShuntingWagons(ParentControlUnit, Constants.ActivityShuntingWagon, false, _silo);
            // //_silo.AddActivity(shuntingWagons);
            // EndEvent.SequentialEvents.Add(shuntingWagons.StartEvent);
        }

        public override string ToString()
        {
            return Constants.ActivityWaitingForTrainSelectionSilo;
        }

        public override Activity Clone()
        {
            throw new NotImplementedException();
        }

        public override Entity[] AffectedEntities { get { return new Entity[] { _silo }; } }
    }
}