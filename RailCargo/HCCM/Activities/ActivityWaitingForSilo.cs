using System;
using RailCargo.HCCM.Entities;
using RailCargo.HCCM.Events;
using RailCargo.HCCM.Requests;
using RailCargo.HCCM.staticVariables;
using SimulationCore.HCCMElements;
using SimulationCore.MathTool.Distributions;
using SimulationCore.SimulationClasses;

namespace RailCargo.HCCM.Activities
{
    public class ActivityWaitingForSilo : Activity
    {
        private readonly EntityTrain _train;

        public ActivityWaitingForSilo(ControlUnit parentControlUnit, string activityType, bool preEmptable,
            Entity train) : base(
            parentControlUnit, activityType, preEmptable)
        {
            _train = (EntityTrain)train;
        }

        public override void StateChangeStartEvent(DateTime time, ISimulationEngine simEngine)
        {
        }

        public override void StateChangeEndEvent(DateTime time, ISimulationEngine simEngine)
        {
            Console.WriteLine("Received successfully a silo");
            var activityWagonCollection =
                new ActivityWagonCollection(ParentControlUnit, Constants.ActivityWagonCollection, false, _train);
            EndEvent.SequentialEvents.Add(activityWagonCollection.StartEvent);
        }

        public override string ToString()
        {
            return Constants.ActivityWaitingForSilo;
        }

        public override Activity Clone()
        {
            throw new NotImplementedException();
        }

        public override Entity[] AffectedEntities
        {
            get { return new Entity[] { _train }; }
        }
    }
}