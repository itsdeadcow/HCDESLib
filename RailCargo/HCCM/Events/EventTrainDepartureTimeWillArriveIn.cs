using System;
using RailCargo.HCCM.Activities;
using RailCargo.HCCM.Entities;
using RailCargo.HCCM.Requests;
using RailCargo.HCCM.staticVariables;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;

namespace RailCargo.HCCM.Events
{
    public class EventTrainDepartureTimeWillArriveIn : Event
    {
        private readonly EntityTrain _train;

        public EventTrainDepartureTimeWillArriveIn(EventType type, ControlUnit parentControlUnit, EntityTrain train) :
            base(type, parentControlUnit)
        {
            _train = train;
        }

        protected override void StateChange(DateTime time, ISimulationEngine simEngine)
        {
            // //TODO is this correct to stop waiting for wagon collection
            //_train.StopCurrentActivities(time, simEngine);
            var affectedShuntingYard = AllShuntingYards.Instance.GetYards(_train.StartLocation);
            //TODO who triggers this?
            var waitingForAllowance = new ActivityWaitingForAllowance(affectedShuntingYard, Constants.ActivityWaitingForAllowance, false, _train);
            RequestForDepartureArea requestForDepartureArea =
                new RequestForDepartureArea(Constants.RequestForDepartureArea, _train, time);
            SequentialEvents.Add(waitingForAllowance.StartEvent);
            affectedShuntingYard.AddRequest(requestForDepartureArea);
            
        }

        public override string ToString()
        {
            return "Event_Train_Departure_Time_Will_Arrive_In";
        }

        public override Event Clone()
        {
            throw new NotImplementedException();
        }

        public override Entity[] AffectedEntities
        {
            get { return new Entity[] { _train }; }
        }
    }
}