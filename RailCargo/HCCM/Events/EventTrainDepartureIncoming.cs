using System;
using RailCargo.HCCM.Activities;
using RailCargo.HCCM.Entities;
using RailCargo.HCCM.Requests;
using RailCargo.HCCM.staticVariables;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;

namespace RailCargo.HCCM.Events
{
    public class EventTrainDepartureIncoming : Event
    {
        private readonly EntityTrain _train;
        //Triggered by timetable (Fahrplan)
        public EventTrainDepartureIncoming(EventType type, EntityTrain train, ControlUnit parentControlUnit) : base(
            type, parentControlUnit)
        {
            _train = train;
        }

        protected override void StateChange(DateTime time, ISimulationEngine simEngine)
        {
            // var waitingForAllowance =
            //     new ActivityWaitingForAllowance(ParentControlUnit, Constants.ACTIVITY_WAITING_FOR_ALLOWANCE, false, _train);
            // RequestForDepartureArea requestForDepartureArea =
            //     new RequestForDepartureArea(Constants.REQUEST_FOR_DEPARTURE_AREA, _train, time);
            // SequentialEvents.Add(waitingForAllowance.StartEvent);
            // ParentControlUnit.AddRequest(requestForDepartureArea);
            
        }

        public override string ToString()
        {
            return "Event_Train_Departure_Incoming";
        }

        public override Event Clone()
        {
            throw new NotImplementedException();
        }

        public override Entity[] AffectedEntities { get { return new Entity[] { _train }; } }
    }
}