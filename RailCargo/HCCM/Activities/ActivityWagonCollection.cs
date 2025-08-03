using System;
using RailCargo.HCCM.Entities;
using RailCargo.HCCM.staticVariables;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;

namespace RailCargo.HCCM.Activities
{
    public class ActivityWagonCollection : Activity
    {
        private readonly EntityTrain _train;

        public ActivityWagonCollection(ControlUnit parentControlUnit, string activityType, bool preEmptable,
            EntityTrain train ) : base(parentControlUnit, activityType, preEmptable)
        {
            _train = train;
        }

        public override void StateChangeStartEvent(DateTime time, ISimulationEngine simEngine)
        {
            //throw new NotImplementedException();
        }

        public override void StateChangeEndEvent(DateTime time, ISimulationEngine simEngine)
        {
            //throw new NotImplementedException();
            //TODO does waiting for wagon collection end with departureTime arrives in x min end?
            Console.WriteLine("BLAAA");
        }

        public override string ToString()
        {
            return Constants.ActivityWagonCollection;
        }

        public override Activity Clone()
        {
            throw new NotImplementedException();
        }

        public override Entity[] AffectedEntities { get { return new Entity[] { _train }; } }
    }
}