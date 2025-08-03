using System;
using RailCargo.HCCM.Activities;
using RailCargo.HCCM.Entities;
using RailCargo.HCCM.staticVariables;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;

namespace RailCargo.HCCM.Events
{
    public class EventTrainCreation : Event
    {
        private readonly EntityTrain _entityTrain;
        private readonly string _departureTyp;

        public EventTrainCreation(ControlUnit parentControlUnit, EntityTrain entityTrain) : base(
            EventType.Standalone, parentControlUnit)
        {
            _entityTrain = entityTrain;
        }

        protected override void StateChange(DateTime time, ISimulationEngine simEngine)
        {
            // switch (_departureTyp)
            // {
                // case "BB":
                //     ActivityTrainPreparation trainPreparation =
                //         new ActivityTrainPreparation(ParentControlUnit, Constants.ACTIVITY_TRAIN_PREPARATION, false);
                //     //TODO change to acutal time
                //     trainPreparation.StartEvent.Trigger(time, simEngine);
                //     //GET train depature time somehow
                //     EventTrainDepartureTimeArrived trainDepartureTimeArrived =
                //         new EventTrainDepartureTimeArrived(EventType.Standalone, ParentControlUnit, _entityTrain);
                //     simEngine.AddScheduledEvent(trainDepartureTimeArrived, DateTime.Today);
                //     break;
                // case "VBF":
                //     //TODO overthink the concept of train creation
                    var affectedShuntingYard = AllShuntingYards.Instance.GetYards(_entityTrain.StartLocation);
                    affectedShuntingYard.AddRequest(new Requests.RequestForSilo(Constants.RequestForSilo,
                        _entityTrain, time));
                    //zug wartet hier
                    // var waitingForSilo = new ActivityWaitingForSilo(affectedShuntingYard,
                    //     Constants.ActivityWaitingForSilo, true, _entityTrain);
                    // //_entityTrain.AddActivity(waitingForSilo);
                    // SequentialEvents.Add(waitingForSilo.StartEvent);
                    // var trainDepartureIncoming =
                    //     new EventTrainDepartureIncoming(EventType.Standalone, _entityTrain, affectedShuntingYard);
                    // simEngine.AddScheduledEvent(trainDepartureIncoming, _entityTrain.DepartureTime.AddMinutes(-15));
            //         break;
            //     default:
            //         Console.WriteLine("Not implemented" + _departureTyp);
            //         break;
            // }
        }

        public override string ToString()
        {
            return "EventTrainCreation";
        }

        public override Event Clone()
        {
            throw new NotImplementedException();
        }

        public override Entity[] AffectedEntities
        {
            get
            {
                return new Entity[]
                {
                    _entityTrain
                };
            }
        }
    }
}