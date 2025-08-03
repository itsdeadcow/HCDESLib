using System;
using RailCargo.HCCM.Entities;
using RailCargo.HCCM.staticVariables;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;

namespace RailCargo.HCCM.Activities
{
    public class ActivityWaitingInSilo : Activity
    {
        private readonly EntityWagon _wagon;

        public ActivityWaitingInSilo(ControlUnit parentControlUnit, string activityType, bool preEmptable,
            EntityWagon wagon) : base(parentControlUnit, activityType, preEmptable)
        {
            _wagon = wagon;
        }

        public override void StateChangeStartEvent(DateTime time, ISimulationEngine simEngine)
        {
            //throw new NotImplementedException();
        }

        public override void StateChangeEndEvent(DateTime time, ISimulationEngine simEngine)
        {
            //throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Constants.ActivityWaitingInSilo;
        }

        public override Activity Clone()
        {
            throw new NotImplementedException();
        }

        public override Entity[] AffectedEntities
        {
            get { return new Entity[] { _wagon }; }
        }
    }
}