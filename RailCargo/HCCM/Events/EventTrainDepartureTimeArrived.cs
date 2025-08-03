using System;
using RailCargo.HCCM.Activities;
using RailCargo.HCCM.Entities;
using RailCargo.HCCM.Requests;
using RailCargo.HCCM.staticVariables;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;

namespace RailCargo.HCCM.Events
{
    public class EventTrainDepartureTimeArrived : Event
    {
        private readonly EntityTrain _train;

        public EventTrainDepartureTimeArrived(EventType type, ControlUnit parentControlUnit, EntityTrain train) : base(
            type, parentControlUnit)
        {
            _train = train;
        }

        protected override void StateChange(DateTime time, ISimulationEngine simEngine)
        {
            // ActivityTrainWaitingForDeparture trainWaitingForDeparture =
            //     new ActivityTrainWaitingForDeparture(ParentControlUnit, Constants.ACTIVITY_WAITING_FOR_DEPARTURE, false,
            //         _train);
            // //_train.AddActivity(trainWaitingForDeparture);
            // trainWaitingForDeparture.StartEvent.Trigger(time, simEngine);
            // RequestForDeparture requestForDeparture =
            //     new RequestForDeparture(Constants.REQUEST_FOR_DEPARTURE, _train, time);
            // ParentControlUnit.AddRequest(requestForDeparture);
        }

        public override string ToString()
        {
            return "Event_Train_Departure_Time_Arrived";
        }

        public override Event Clone()
        {
            throw new NotImplementedException();
        }

        public override Entity[] AffectedEntities { get { return new Entity[] { _train }; } }
    }
}