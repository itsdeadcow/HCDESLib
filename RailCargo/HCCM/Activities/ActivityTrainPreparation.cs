using System;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;

namespace RailCargo.HCCM.Activities
{
    public class ActivityTrainPreparation : Activity
    {
        public ActivityTrainPreparation(ControlUnit parentControlUnit, string activityType, bool preEmptable) : base(parentControlUnit, activityType, preEmptable)
        {
        }

        public override void StateChangeStartEvent(DateTime time, ISimulationEngine simEngine)
        {
            //TODO get Train PrepartationTime
            simEngine.AddScheduledEvent(EndEvent, DateTime.Now);
        }

        public override void StateChangeEndEvent(DateTime time, ISimulationEngine simEngine)
        {
            //throw new NotImplementedException();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        public override Activity Clone()
        {
            throw new NotImplementedException();
        }

        public override Entity[] AffectedEntities { get { return new Entity[] {  }; } }
    }
}