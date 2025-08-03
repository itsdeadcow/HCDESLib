using System;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;

namespace RailCargo.HCCM.Activities
{
    public class ActivityWaitingInDepartureArea : Activity
    {
        public ActivityWaitingInDepartureArea(ControlUnit parentControlUnit, string activityType, bool preEmptable) : base(parentControlUnit, activityType, preEmptable)
        {
        }

        public override void StateChangeStartEvent(DateTime time, ISimulationEngine simEngine)
        {
            throw new NotImplementedException();
        }

        public override void StateChangeEndEvent(DateTime time, ISimulationEngine simEngine)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        public override Activity Clone()
        {
            throw new NotImplementedException();
        }

        public override Entity[] AffectedEntities { get; }
    }
}