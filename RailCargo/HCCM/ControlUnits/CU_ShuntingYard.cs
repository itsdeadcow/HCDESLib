using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using RailCargo.HCCM.Activities;
using RailCargo.HCCM.Entities;
using RailCargo.HCCM.Events;
using RailCargo.HCCM.Requests;
using RailCargo.HCCM.staticVariables;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;

namespace RailCargo.HCCM.ControlUnits
{
    public class CuShuntingYard : ControlUnit
    {
        private Dictionary<string, List<EntitySilo>> _silos = new Dictionary<string, List<EntitySilo>>();
        private Dictionary<int, List<EntityTrain>> _trains = new Dictionary<int, List<EntityTrain>>();
        private Dictionary<string, List<EntityWagon>> _waitingWagons = new Dictionary<string, List<EntityWagon>>();

        public Dictionary<int, List<EntityTrain>> Trains
        {
            get => _trains;
            set => _trains = value;
        }

        public CuShuntingYard(string name, ControlUnit parentControlUnit, SimulationModel parentSimulationModel) :
            base(
                name, parentControlUnit, parentSimulationModel)
        {
        }

        protected override void CustomInitialize(DateTime startTime, ISimulationEngine simEngine)
        {
            Console.WriteLine(Name);
        }

        private bool check_rpcs(EntityTrain train, EntityWagon wagon)
        {
            if (wagon.alreadyStations.Contains(train.ArrivalStation))
            {
                return false;
            }

            var rpc_codes = train.RpcCodes;
            var rpc_code_to_check = wagon.DestinationRpc;
            foreach (var rpc_code in rpc_codes)
            {
                var from = rpc_code[0].Replace("-", "");
                var until = rpc_code[1].Replace("-", "");
                if (rpc_code_to_check.Length == 4)
                {
                    var check_rpc = int.Parse(rpc_code_to_check);
                    var from_int = int.Parse(from.Substring(0, 4));
                    var until_int = int.Parse(until.Substring(0, 4));
                    if (from_int <= check_rpc && check_rpc <= until_int)
                    {
                        return true;
                    }
                }

                rpc_code_to_check = rpc_code_to_check.Replace("-", "");
                if (int.Parse(from) <= int.Parse(rpc_code_to_check) && int.Parse(rpc_code_to_check) <= int.Parse(until))
                {
                    return true;
                }
            }

            return false;
        }

        // TODO if to much wagon in silo + train the wagon
        // get shifted through code to a silo which is created later in time at the first
        // location to contain FIFO
        // TODO order of poping from previous train
        protected override bool PerformCustomRules(DateTime time, ISimulationEngine simEngine)
        {
            var requestForTrainContinuation =
                RAEL.Where(p => p.Activity == Constants.RequestTrainContinuation).Cast<RequestTrainContinuation>()
                    .ToList();
            foreach (var request in requestForTrainContinuation)
            {
                var train = (EntityTrain)request.Origin[0];

                var trainId = train.TrainId;
                var possibleTrains = (Trains.ContainsKey(trainId)) ? Trains[trainId] : null;
                if (possibleTrains != null)
                {
                    possibleTrains = possibleTrains
                        .Where(x => 0 < (x.DeparturTime - train.ArrivalTime).TotalDays &&
                                    (x.DeparturTime - train.ArrivalTime).TotalDays < 1).ToList();
                }

                RemoveRequest(request);
                var ticks = 0;
                foreach (var wagon in train.ActualWagonList)
                {
                    if (wagon.WagonId == 318127391791)
                    {
                        Helper.Print("wtf");
                    }
                    if (wagon.EndLocation == Name || (wagon.EndLocation == "-1" && !Name.StartsWith("81")))
                    {
                        
                        EventWagonArrivalInEndDestination wagonArrivalInEndDestination =
                            new EventWagonArrivalInEndDestination(EventType.Standalone, this, wagon);
                        wagonArrivalInEndDestination.Trigger(time, simEngine);
                        continue;
                    }

                    EntityTrain possibleTrain = null;
                    if (possibleTrains != null && possibleTrains.Count != 0)
                    {
                        possibleTrain = possibleTrains[0];
                    }

                    if (possibleTrain != null &&
                        (check_rpcs(possibleTrain, wagon) || !train.Pop))
                    {
                        possibleTrain.ActualWagonList.Add(wagon);
                        wagon.CurrentTrain = possibleTrain;
                        continue;
                    }

                    var waitingForTrainSelectionWagon =
                        new ActivityWaitingForTrainSelectionWagon(this,
                            Constants.ActivityWaitingForTrainSelectionWagon, true, wagon, time);
                    simEngine.AddScheduledEvent(waitingForTrainSelectionWagon.StartEvent,
                        time.AddMinutes(train.DisassembleTime).AddTicks(ticks));
                    ticks += 1;
                }
            }

            var requestsForSilo =
                RAEL.Where(p => p.Activity == Constants.RequestForSilo).Cast<RequestForSilo>().ToList();
            foreach (RequestForSilo request in requestsForSilo)
            {
                var siloCreationPossible = true;
                var train = (EntityTrain)request.Origin[0];
                if (train.TrainId == 45708 && Name == "81011841") Helper.Print("wtf");
                if (!train.Append) continue;
                if (siloCreationPossible)
                {
                    train.Silo = null;
                    //Some need to check which direction the silo has and what about multiple silos in the same direction
                    var waitingWagons = new List<EntityWagon>();
                    if (_waitingWagons.ContainsKey(train.ArrivalStation))
                    {
                        waitingWagons = _waitingWagons[train.ArrivalStation];
                        if (waitingWagons.Where(x => x.WagonId == 318149323970).ToList().Count >= 1)
                        {
                            Helper.Print("Wtf");
                        }

                        _waitingWagons.Remove(train.ArrivalStation);
                    }

                    var siloSelection = new EventSiloSelection(this, train, waitingWagons);
                    siloSelection.Trigger(time, simEngine);
                    //train.GetCurrentActivities().First().EndEvent.Trigger(time, simEngine);
                    if (!_silos.ContainsKey(train.ArrivalStation))
                    {
                        _silos.Add(train.ArrivalStation, new List<EntitySilo>());
                    }

                    _silos[train.ArrivalStation].Add(siloSelection.Silo);

                    //maybe start wagon collection for train here

                    RemoveRequest(request);
                }
            }

            var requestsForSorting = RAEL.Where(p => p.Activity == Constants.RequestForSorting).Cast<RequestSorting>().ToList();
            for (var index = 0; index < requestsForSorting.Count; index++)
            {
                var request = requestsForSorting[index];

                var wagon = (EntityWagon)request.Origin[0];
                if (wagon.WagonId == 318127391791)
                {
                    Helper.Print("wtf");
                }
     
                var has_train = false;
                foreach (var silos in _silos.Values)
                {
                    if (has_train) break;
                    if (silos.Count > 1)
                    {
                        Helper.Print("wtf");
                    }

                    EntityTrain possible_train = null;
                    bool availableSilo = false;
                    for (int i = 0; i < silos.Count; i++)
                    {
                        possible_train = silos[i].Train;
                        availableSilo = check_rpcs(possible_train, wagon);
                        if (availableSilo)
                        {
                            break;
                        }
                    }
                    
                    // Must stay, as booking system only matches for starting trains
                    // further research can be done by implementing passing-by trains.
                    if (!possible_train.StartTrain) break;
                    if (availableSilo)
                    {
                        foreach (var silo in silos)
                        {
                            var maxLength = silo.MaxLength;
                            var maxWeight = silo.MaxWeight;
                            var currentLength = silo.CurrentLength;
                            var currentWeight = silo.CurrentWeight;
                            // TODO if inaccurate round up
                            if (currentLength + wagon.WagonLength <= maxLength &&
                                (currentWeight + wagon.WagonMass) <= (maxWeight))
                            {
                                if (wagon.WagonId == 338578142980)
                                {
                                    Helper.Print("Wtf");
                                }

                                has_train = true;
                                wagon.Silo = silo;
                                wagon.StopCurrentActivities(time, simEngine);
                                RemoveRequest(request);
                                break;
                            }
                        }
                    }
                }
            }

            var requestForSiloStatus = RAEL.Where(p => p.Activity == Constants.RequestForSiloStatus)
                .Cast<RequestCheckSiloStatus>().ToList();
            foreach (var request in requestForSiloStatus)
            {
                var silo = (EntitySilo)request.Origin[0];
                var siloMaxWeight = silo.MaxWeight;
                var siloMaxLength = silo.MaxLength;
                var siloCurrentWeight = silo.CurrentWeight;
                var siloCurrentLength = silo.CurrentLength;


                // For further research, as we can specify the max silo lenght

                if (siloCurrentLength >= siloMaxLength || siloCurrentWeight >= siloMaxWeight)
                {
                    silo.StopCurrentActivities(time, simEngine);
                    RemoveRequest(request);
                }
            }

            var requestForDepartureArea = RAEL.Where(p => p.Activity == Constants.RequestForDepartureArea)
                .Cast<RequestForDepartureArea>().ToList();
            foreach (var request in requestForDepartureArea)
            {
                //Idea close silo, and create a request with the wagons on the closed silo, which is appended to the train when departure time has arrived
                var train = (EntityTrain)request.Origin[0];
                if (train.TrainId == 66315)
                {
                    Helper.Print("wtf");
                }

                train = Trains[train.TrainId].Where(x => x == train).ToList()[0];
                RemoveRequest(request);
                train.GetCurrentActivities().First().EndEvent.Trigger(time, simEngine);
                if (!train.Append)
                {
                    train.Silo = null;
                    continue;
                }

                var silo = _silos[train.ArrivalStation].First();

                silo.StopCurrentActivities(time, simEngine);
                //TODO should we really keep the drive to departure area or instantly send request to cu network 
                //or only close the silo and append wagon only to train when time arrived
                train.Silo = silo;
                if (_silos[train.ArrivalStation].Count != 0) _silos[train.ArrivalStation].Remove(silo);
                if (_silos[train.ArrivalStation].Count == 0) _silos.Remove(train.ArrivalStation);
            }

            return false;
        }

        public void calculatePossibleWagons(EntityTrain train, EntitySilo silo)
        {
            var train_length = train.TrainLength;
            var train_weight = train.TrainWeight;
            var current_weight = 0;
            var current_length = 0;
            var wagonList = train.ActualWagonList;
            var wagonsToDelete = new List<EntityWagon>();
            wagonList.ForEach(x =>
            {
                if ((current_length + x.WagonLength) > train_length || (current_weight + x.WagonMass) > train_weight)
                {
                    wagonsToDelete.Add(x);
                    return;
                }

                current_weight += x.WagonMass;
                current_length += x.WagonLength;
            });
            if (silo != null)
            {
                silo.WagonList.ForEach(x =>
                {
                    if ((current_length + x.WagonLength) > train_length ||
                        (current_weight + x.WagonMass) > train_weight)
                    {
                        wagonsToDelete.Add(x);
                        return;
                    }

                    current_weight += x.WagonMass;
                    current_length += x.WagonLength;
                    train.ActualWagonList.Add(x);
                });
            }

            wagonsToDelete.ForEach(x =>
            {
                if (!_waitingWagons.ContainsKey(train.ArrivalStation))
                {
                    _waitingWagons[train.ArrivalStation] = new List<EntityWagon>();
                }

                _waitingWagons[train.ArrivalStation].Add(x);
                if (train.ActualWagonList.Contains(x))
                {
                    train.ActualWagonList.Remove(x);
                }
            });
        }

        public override Event EntityEnterControlUnit(DateTime time, ISimulationEngine simEngine, Entity entity,
            IDelegate originDelegate)
        {
            throw new NotImplementedException();
        }

        public override void EntityLeaveControlUnit(DateTime time, ISimulationEngine simEngine, Entity entity,
            IDelegate originDelegate)
        {
            throw new NotImplementedException();
        }
    }
}