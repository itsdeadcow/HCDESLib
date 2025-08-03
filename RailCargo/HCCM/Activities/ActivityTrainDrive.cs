using System;
using RailCargo.HCCM.Entities;
using RailCargo.HCCM.Events;
using RailCargo.HCCM.staticVariables;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;

namespace RailCargo.HCCM.Activities
{
    public class ActivityTrainDrive : Activity
    {
        private readonly EntityTrain _train;

        public ActivityTrainDrive(ControlUnit parentControlUnit, string activityType, bool preEmptable,
            EntityTrain train) : base(parentControlUnit, activityType, preEmptable)
        {
            _train = train;
        }

        public override void StateChangeStartEvent(DateTime time, ISimulationEngine simEngine)
        {
            var timeToDrive = _train.ArrivalTime;
            simEngine.AddScheduledEvent(EndEvent, timeToDrive);
        }

        public override void StateChangeEndEvent(DateTime time, ISimulationEngine simEngine)
        {
            var destinationTyp = "VBF";
            var affectedShuntingYard = AllShuntingYards.Instance.GetYards(_train.ArrivalStation);
            EventTrainArrival trainArrival = new EventTrainArrival(EventType.Standalone, affectedShuntingYard, _train,
                destinationTyp, _train.ArrivalStation);
            EndEvent.SequentialEvents.Add(trainArrival);
        }

        public override string ToString()
        {
            return Constants.ActivityTrainDrive;
        }

        public override Activity Clone()
        {
            throw new NotImplementedException();
        }

        public override Entity[] AffectedEntities { get { return new Entity[] { _train }; } }
    }
}