using System;
using System.Collections.Generic;
using System.Linq;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;

namespace RailCargo.HCCM.Entities
{
    public class EntitySilo : Entity, IActiveEntity
    {
        private static int _sIdentifier;
        private readonly string _destination;
        private List<Activity> _currentActivities = new List<Activity>();
        private List<EntityWagon> _wagonList = new List<EntityWagon>();

        public List<EntityWagon> WagonList
        {
            get => _wagonList;
            set => _wagonList = value;
        }

        private int _currentCapactiy;
        private readonly int _maxLength;
        private readonly int _maxWeight;
        private int _currentLength;
        private int _currentWeight;
        private readonly List<EntityWagon> _waitingWagons;

        public string Destination => _destination;

        public int MaxLength => _maxLength;

        public int MaxWeight => _maxWeight;


        public int CurrentLength
        {
            get => _currentLength;
            set => _currentLength = value;
        }

        public int CurrentWeight
        {
            get => _currentWeight;
            set => _currentWeight = value;
        }

        public EntityTrain Train { get; set; }

        public EntitySilo(string destination, int maxLength, int maxWeight, List<EntityWagon> wagons) : base(++_sIdentifier)
        {
            _destination = destination;
            _maxLength = maxLength;
            _maxWeight = maxWeight;
            _wagonList = wagons;
            _currentLength = 0;
            _currentWeight = 0;
            wagons.ForEach(x =>
            {
                _currentLength += x.WagonLength;
                _currentWeight += x.WagonMass;
            });
            
        }

        public override string ToString()
        {
            return "EntitySilo" + Identifier;
        }

        public override Entity Clone()
        {
            return new EntitySilo(_destination, _maxLength, _maxWeight, _waitingWagons);
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