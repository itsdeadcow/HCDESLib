using System;
using RailCargo.HCCM.Entities;
using RailCargo.HCCM.Events;
using RailCargo.HCCM.Requests;
using RailCargo.HCCM.staticVariables;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;

namespace RailCargo.HCCM.Activities
{
    public class ActivityTrainWaitingForDeparture : Activity
    {
        public EntityTrain Train { get; set; }

        public ActivityTrainWaitingForDeparture(ControlUnit parentControlUnit, string activityType, bool preEmptable, EntityTrain train) : base(parentControlUnit, activityType, preEmptable)
        {
            Train = train;
        }

        public override void StateChangeStartEvent(DateTime time, ISimulationEngine simEngine)
        {
            //TODO next timestamp is is in one hour how to fix this
            
        }

        public override void StateChangeEndEvent(DateTime time, ISimulationEngine simEngine)
        {
            var trainDrive =
                new ActivityTrainDrive(ParentControlUnit, Constants.ActivityTrainDrive, false, Train);
            
            EndEvent.SequentialEvents.Add(trainDrive.StartEvent);
        }

        public override string ToString()
        {
            return Constants.ActivityTrainWaitingForDeparture;
        }

        public override Activity Clone()
        {
            throw new NotImplementedException();
        }

        public override Entity[] AffectedEntities { get { return new Entity[] { Train }; } }
    }
}