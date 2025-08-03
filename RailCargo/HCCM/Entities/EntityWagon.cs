using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using RailCargo.HCCM.Input;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;

namespace RailCargo.HCCM.Entities
{
    public class EntityWagon : Entity, IActiveEntity
    {
        private static int _sIdentifier = 0;
        private List<Activity> _currentActivies = new List<Activity>();
        private EntitySilo _silo = null;
        private readonly int _wagonLength;
        private readonly int _wagonMass;
        private readonly string _destinationRpc;
        private readonly string _endLocation;

        public int WagonLength => _wagonLength;

        public int WagonMass => _wagonMass;

        public long WagonId => _wagonId;

        private readonly long _wagonId;
        private readonly DateTime _endTime;
        private readonly DateTime _acceptanceDate;
        private readonly string _startLocation;
        private EntityTrain _currentTrain = null;
        public List<string> alreadyStations = new List<string>();
        private readonly DateTime _bookingDate;
        private readonly List<string> _rebookedTimes;
        private readonly List<UsedTrains> _usedTrains;
        private List<List<String>> _actualDepartureTime;
        public int currentIndex = 0;
        private readonly string _uuid;
        public bool arrived;
        public bool printed = false;

        public string Uuid => _uuid;

        public EntityTrain CurrentTrain
        {
            get => _currentTrain;
            set => _currentTrain = value;
        }

        public DateTime EndTime => _endTime;

        public DateTime AcceptanceDate => _acceptanceDate;

        public string StartLocation => _startLocation;

        public EntitySilo Silo
        {
            get => _silo;
            set => _silo = value;
        }

        public EntityWagon(long wagonId, string wagonLength, string wagonMass,string startLocation, string endLocation,
            string destinationRcp, string endTime, string acceptance_date, string bookingDate, List<string> rebookedTimes, List<UsedTrains> usedTrains, string uuid) : base(_sIdentifier++)
        {
            _wagonId = wagonId;
            _wagonLength = (int)float.Parse(wagonLength, CultureInfo.InvariantCulture.NumberFormat);
            _wagonMass = (int)float.Parse(wagonMass, CultureInfo.InvariantCulture.NumberFormat);
            _startLocation = startLocation;
            _endLocation = endLocation;
            _destinationRpc = destinationRcp;
            _endTime = DateTime.Parse(endTime);
            _acceptanceDate = DateTime.Parse(acceptance_date);
            _bookingDate = DateTime.Parse(bookingDate);
            _rebookedTimes = rebookedTimes;
            _usedTrains = usedTrains;
            _actualDepartureTime = new List<List<string>>();
            _uuid = uuid;
        }

        public DateTime BookingDate => _bookingDate;

        public List<string> RebookedTimes => _rebookedTimes;

        public List<UsedTrains> UsedTrains => _usedTrains;

        public List<List<string>> ActualDepartureTime
        {
            get => _actualDepartureTime;
            set => _actualDepartureTime = value;
        }

        public List<Activity> CurrentActivies => _currentActivies;

        public string DestinationRpc => _destinationRpc;

        public string EndLocation => _endLocation;
        public double TimeDelta { get; set; }
        public DateTime RealTime { get; set; }

        public override string ToString()
        {
            return "Entity_Wagon";
        }

        public override Entity Clone()
        {
            throw new System.NotImplementedException();
        }

        public List<Activity> GetCurrentActivities()
        {
            return _currentActivies;
        }

        public void StopCurrentActivities(DateTime time, ISimulationEngine simEngine)
        {
            //TODO other function with while should i not delte them
            while (_currentActivies.Count > 0)
            {
                _currentActivies.First().EndEvent.Trigger(time, simEngine);
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
            _currentActivies.Add(activity);
        }

        public void RemoveActivity(Activity activity)
        {
            _currentActivies.Remove(activity);
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