using System;
using RailCargo.HCCM.Entities;
using RailCargo.HCCM.staticVariables;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;

namespace RailCargo.HCCM.Activities
{
    public class ActivityWaitingForAllowance : Activity
    {
        private readonly EntityTrain _train;

        public ActivityWaitingForAllowance(ControlUnit parentControlUnit, string activityType, bool preEmptable, EntityTrain train) : base(parentControlUnit, activityType, preEmptable)
        {
            _train = train;
        }

        public override void StateChangeStartEvent(DateTime time, ISimulationEngine simEngine)
        {
            //throw new NotImplementedException();
        }

        public override void StateChangeEndEvent(DateTime time, ISimulationEngine simEngine)
        {
            ActivityDriveToDepartureArea driveToDepartureArea =
                new ActivityDriveToDepartureArea(ParentControlUnit, _train, Constants.ActivityDriveToDepartureArea, false);
            EndEvent.SequentialEvents.Add(driveToDepartureArea.StartEvent);
        }

        public override string ToString()
        {
            return Constants.ActivityWaitingForAllowance;
        }

        public override Activity Clone()
        {
            throw new NotImplementedException();
        }

        public override Entity[] AffectedEntities { get { return new Entity[] { _train }; } }
    }
}