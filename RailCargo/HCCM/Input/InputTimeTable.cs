using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RailCargo.HCCM.staticVariables;

namespace RailCargo.HCCM.Input
{
    public class UsedTrains
    {
        [JsonProperty("train_id")] public string TrainId { get; set; }
        [JsonProperty("start_location")] public string StartLocation { get; set; }
        [JsonProperty("departure_time")] public DateTime StartTime { get; set; }
        [JsonProperty("arrival_location")] public string ArrivalLocation { get; set; }
        [JsonProperty("arrival_time")] public DateTime ArrivalTime { get; set; }
    }

    public class Wagon
    {
        [JsonProperty("wagon_id")] public string WagonId { get; set; }
        [JsonProperty("uuid")] public string Uuid { get; set; }
        [JsonProperty("wagon_mass")] public string WagonMass { get; set; }
        [JsonProperty("wagon_length")] public string WagonLength { get; set; }
        [JsonProperty("destination_rpc")] public string DestinationRpc { get; set; }
        [JsonProperty("end_location")] public string EndLocation { get; set; }
        [JsonProperty("start_location")] public string StartLocation { get; set; }
        [JsonProperty("end_time")] public string EndTime { get; set; }
        [JsonProperty("acceptance_date")] public string AcceptanceDate { get; set; }
        [JsonProperty("booking_date")] public string BookingDate { get; set; }
        [JsonProperty("rebooked")] public List<string> RebookingTimes { get; set; }

        [JsonProperty("used_trains")] public List<UsedTrains> UsedTrains { get; set; }
    }

    public class Train
    {
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("departure_time")] public string DepartureTime { get; set; }
        [JsonProperty("arrival_time")] public string ArrivalTime { get; set; }
        [JsonProperty("start_location")] public string StartLocation { get; set; }
        [JsonProperty("end_location")] public string EndLocation { get; set; }
        [JsonProperty("start_train")] public bool StartTrain { get; set; }

        [JsonProperty("end_train")] public bool EndTrain { get; set; }

        [JsonProperty("append")] public bool Append { get; set; }

        [JsonProperty("pop")] public bool Pop { get; set; }
        [JsonProperty("train_weight")] public int TrainWeight { get; set; }
        [JsonProperty("train_length")] public int TrainLength { get; set; }
        [JsonProperty("formations_time")] public int FormationsTime { get; set; }
        [JsonProperty("disassemble_time")] public int DisassembleTime { get; set; }
        [JsonProperty("rpc_codes")] public List<List<string>> RpcCodes { get; set; }
        [JsonProperty("wagons")] public List<Wagon> Wagons { get; set; }
    }

    public class InputTimeTable
    {
        List<Train> _trains = new List<Train>();
        public List<Wagon> _wagons = new List<Wagon>();

        public List<Wagon> Wagons
        {
            get => _wagons;
            set => _wagons = value;
        }


        public List<Train> Trains
        {
            get => _trains;
            set => _trains = value;
        }
        public InputTimeTable()
        {
            string json = File.ReadAllText(
                @"C:\Users\koenig11\RiderProjects\HCDESLib\RailCargo\HCCM\Data\timetable_simulation_model_2.json");
            List<Train> test = JsonConvert.DeserializeObject<List<Train>>(json);
            test.ForEach(x =>
            {
                Trains.Add(x);
                _wagons = _wagons.Concat(x.Wagons).ToList();
            });
            _wagons = _wagons.GroupBy(w =>
                w.Uuid
            ).Select(x => x.OrderBy(w => w.AcceptanceDate).First()).ToList();
            _wagons = _wagons.OrderBy(x => DateTime.Parse(x.BookingDate)).ToList();
            Helper.Print("wtf");
        }
    }
}