using System;
using System.Globalization;
using RailCargo.HCCM.Entities;
using RailCargo.HCCM.Requests;
using RailCargo.HCCM.staticVariables;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;

namespace RailCargo.HCCM.Activities
{
    public class ActivityWaitingForTrainSelectionWagon : Activity
    {
        private readonly EntityWagon _wagon;
        private readonly DateTime _arrivalTime;

        public DateTime ArrivalTime => _arrivalTime;

        public ActivityWaitingForTrainSelectionWagon(ControlUnit parentControlUnit, string activityType,
            bool preEmptable, EntityWagon wagon, DateTime arrivalTime) : base(parentControlUnit, activityType, preEmptable)
        {
            _wagon = wagon;
            _arrivalTime = arrivalTime;
        }

        public override void StateChangeStartEvent(DateTime time, ISimulationEngine simEngine)
        {
            //TODO check this why
            //throw new NotImplementedException();
            var requestSorting = new RequestSorting(Constants.RequestForSorting, _wagon, time, _arrivalTime);
            ParentControlUnit.AddRequest(requestSorting);
        }

        public override void StateChangeEndEvent(DateTime time, ISimulationEngine simEngine)
        {
            //throw new NotImplementedException();
            if (_wagon.Silo == null)
            {return;}
            _wagon.Silo.WagonList.Add(_wagon);
            _wagon.Silo.CurrentLength += _wagon.WagonLength;
            _wagon.Silo.CurrentWeight += _wagon.WagonMass;
            ActivityShuntingWagon shuntingWagon =
                new ActivityShuntingWagon(ParentControlUnit, Constants.ActivityShuntingWagon, false, _wagon);
            //TODO how long does the shunting need?
            simEngine.AddScheduledEvent(shuntingWagon.EndEvent, time);
        }

        public override string ToString()
        {
            return Constants.ActivityWaitingForTrainSelectionWagon;
        }

        public override Activity Clone()
        {
            throw new NotImplementedException();
        }

        public override Entity[] AffectedEntities { get { return new Entity[] { _wagon }; } }
    }
}