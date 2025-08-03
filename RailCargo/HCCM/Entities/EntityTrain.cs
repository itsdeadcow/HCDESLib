using System;
using System.Collections.Generic;
using System.Linq;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;

namespace RailCargo.HCCM.Entities
{
    public class EntityTrain : Entity, IActiveEntity
    {
        
        private List<Activity> _currentActivities = new List<Activity>();
        private List<EntityWagon> _actualWagonList = new List<EntityWagon>();

        private readonly int _trainId;

        private readonly string _startLocation;

        private readonly DateTime _departurTime;

        private readonly string _arrivalStation;

        private readonly DateTime _arrivalTime;

        private readonly int _formationsTime;

        private readonly int _disassembleTime;

        private readonly List<List<string>> _rpcCodes;

        private readonly int _trainWeight;

        private readonly int _trainLength;

        private readonly bool _startTrain;

        public List<Activity> CurrentActivities => _currentActivities;

        public int TrainId => _trainId;

        public string StartLocation => _startLocation;

        public DateTime DeparturTime => _departurTime;

        public string ArrivalStation => _arrivalStation;

        public DateTime ArrivalTime => _arrivalTime;

        public int FormationsTime => _formationsTime;

        public int DisassembleTime => _disassembleTime;

        public List<List<string>> RpcCodes => _rpcCodes;

        public int TrainWeight => _trainWeight;

        public int TrainLength => _trainLength;

        public bool isStartTrain => _startTrain;

        public List<string> Wagons => _wagons;

        private readonly List<string> _wagons;
        private readonly bool _append;
        private readonly bool _pop;
        private readonly bool _endTrain;
        private EntitySilo _silo;

        public EntitySilo Silo
        {
            get => _silo;
            set => _silo = value;
        }


        public bool StartTrain => _startTrain;

        public bool Append => _append;

        public bool Pop => _pop;

        public bool EndTrain => _endTrain;

        public List<EntityWagon> ActualWagonList
        {
            get => _actualWagonList;
            set => _actualWagonList = value;
        }
        

        public EntityTrain(int trainId, string startLocation,
            DateTime departureTime, string arrivalStation, DateTime arrivalTime, int formationsTime,
            int disassembleTime,
            List<List<string>> rpcCodes, int trainWeight, int trainLength, bool startTrain, bool endTrain,
            List<string> wagons, bool append, bool pop) : base(trainId)
        {
            _trainId = trainId;
            _startLocation = startLocation;
            _departurTime = departureTime;
            _arrivalStation = arrivalStation;
            _arrivalTime = arrivalTime;
            _formationsTime = formationsTime;
            _disassembleTime = disassembleTime;
            _rpcCodes = rpcCodes;
            _trainWeight = trainWeight * 1000;
            _trainLength = trainLength;
            _startTrain = startTrain;
            _endTrain = endTrain;
            _wagons = wagons;
            _append = append;
            _pop = pop;
            _silo = null;

        }

        public override string ToString()
        {
            return "EntityTrain";
        }

        public override Entity Clone()
        {
            throw new System.NotImplementedException();
        }

        public List<Activity> GetCurrentActivities()
        {
            return _currentActivities;
        }

        public void StopCurrentActivities(DateTime time, ISimulationEngine simEngine)
        {
            while (_currentActivities.Count > 0)
            {
                _currentActivities.First().EndEvent.Trigger(time, simEngine);
            }
        }

        public void StopWaitingActivity(DateTime time, ISimulationEngine simEngine)
        {
            throw new NotImplementedException();
        }

        public Event StartWaitingActivity(IDynamicHoldingEntity waitingArea = null)
        {
            throw new NotImplementedException();
        }

        public void AddActivity(Activity activity)
        {
            _currentActivities.Add(activity);
        }

        public void RemoveActivity(Activity activity)
        {
            _currentActivities.Remove(activity);
        }

        public bool IsWaiting()
        {
            throw new NotImplementedException();
        }

        public bool IsWaitingOrPreEmptable()
        {
            throw new NotImplementedException();
        }

        public bool IsInOnlyActivity(string activity)
        {
            throw new NotImplementedException();
        }
    }
}