using System;
using RailCargo.HCCM.Activities;
using RailCargo.HCCM.Entities;
using RailCargo.HCCM.Requests;
using RailCargo.HCCM.staticVariables;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;

namespace RailCargo.HCCM.Events
{
    public class EventTrainArrival : Event
    {
        private readonly EntityTrain _train;
        private readonly string _destinationTyp;
        private readonly string _destination;

        public EventTrainArrival(EventType type, ControlUnit parentControlUnit, EntityTrain train,
            string destinationTyp, string destination) : base(type, parentControlUnit)
        {
            //TODO need to now where we arrived
            _train = train;
            _destinationTyp = destinationTyp;
            _destination = destination;
        }


        //TODO request a new silo
        protected override void StateChange(DateTime time, ISimulationEngine simEngine)
        {
            switch (_destinationTyp)
            {
                case "BB":
                    if (_train.ArrivalStation == _destination)
                    {
                        EventTrainArrivalInEndDestination trainArrivalInEndDestination =
                            new EventTrainArrivalInEndDestination(EventType.Standalone, ParentControlUnit, _train);
                        trainArrivalInEndDestination.Trigger(time, simEngine);
                        break;
                    }
                    ActivityTrainPreparation trainPreparation =
                        new ActivityTrainPreparation(ParentControlUnit, Constants.ActivityTrainPreparation, false);
                    //TODO change to acutal time
                    trainPreparation.StartEvent.Trigger(time, simEngine);
                    //GET train depature time somehow
                    EventTrainDepartureTimeArrived trainDepartureTimeArrived =
                        new EventTrainDepartureTimeArrived(EventType.Standalone, ParentControlUnit, _train);
                    simEngine.AddScheduledEvent(trainDepartureTimeArrived, DateTime.Today);
                    break;
                case "VBF":
                    //RequestForSilo requestForSilo = new RequestForSilo(Constants.REQUEST_FOR_SILO, _train, time);
                    //ParentControlUnit.AddRequest(requestForSilo);
                    _train.ActualWagonList.ForEach(x => { x.CurrentTrain = null;
                        x.Silo = null;
                    });
                    var requestTrainContinuation =
                        new RequestTrainContinuation(Constants.RequestTrainContinuation, _train, time);
                    var affectedShuntingYard = AllShuntingYards.Instance.GetYards(_train.ArrivalStation);
                    affectedShuntingYard.AddRequest(requestTrainContinuation);
                    
                   // var wagonArrival =
                    //    new EventWagonArrival(EventType.Standalone, ParentControlUnit, _train);
                    
                    //TODO add zerlegezeiten
                    //simEngine.AddScheduledEvent();
                    //SequentialEvents.Add(wagonArrival);
                    break;
                default:
                    Console.WriteLine("Not implemented typ");
                    break;
            }
        }

        public override string ToString()
        {
            return "Event_Train_Arrival";
        }

        public override Event Clone()
        {
            throw new NotImplementedException();
        }

        public override Entity[] AffectedEntities { get { return new Entity[] { _train }; } }
    }
}