using System;
using RailCargo.HCCM.Entities;
using RailCargo.HCCM.Events;
using RailCargo.HCCM.Requests;
using RailCargo.HCCM.staticVariables;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;

namespace RailCargo.HCCM.Activities
{
    public class ActivityShuntingWagons : Activity
    {
        private readonly EntitySilo _silo;

        public ActivityShuntingWagons(ControlUnit parentControlUnit, string activityType, bool preEmptable, EntitySilo silo) : base(
            parentControlUnit, activityType, preEmptable)
        {
            _silo = silo;
        }

        public override void StateChangeStartEvent(DateTime time, ISimulationEngine simEngine)
        {
            //throw new NotImplementedException();
             CheckSiloStatus =
                new RequestCheckSiloStatus(Constants.RequestForSiloStatus, _silo, time);
            ParentControlUnit.AddRequest(CheckSiloStatus);
        }

        public RequestCheckSiloStatus CheckSiloStatus { get; set; }

        public override void StateChangeEndEvent(DateTime time, ISimulationEngine simEngine)
        {
            //Need to remove the request as if silo is not finished it should also delete the request
            ParentControlUnit.RemoveRequest(CheckSiloStatus);
            EventSiloFinished siloFinished = new EventSiloFinished(EventType.Standalone, ParentControlUnit, _silo);
            EndEvent.SequentialEvents.Add(siloFinished);
        }

        public override string ToString()
        {
            return Constants.ActivityShuntingWagons;
        }

        public override Activity Clone()
        {
            throw new NotImplementedException();
        }

        public override Entity[] AffectedEntities {
            get { return new Entity[] { _silo }; }
        }
    }
}