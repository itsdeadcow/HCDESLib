using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using RailCargo.HCCM.ControlUnits;
using RailCargo.HCCM.Entities;
using RailCargo.HCCM.Events;
using SimulationCore.SimulationClasses;
using RailCargo.HCCM.Input;
using RailCargo.HCCM.Requests;
using RailCargo.HCCM.staticVariables;
using SimulationCore.HCCMElements;


namespace RailCargo
{
    public class TrainNetworkSimulation : SimulationModel
    {
        private readonly CuNetwork network;
        private readonly CuBookingSystem bookingSystem;
        private readonly ControlUnit[] shuntingYards;

        public TrainNetworkSimulation(DateTime startTime, DateTime endTime) : base(startTime, endTime)
        {
            List<string> locations = new List<string>();
            var inputTimeTable = new InputTimeTable();
            inputTimeTable.Trains.ForEach(x =>
            {
                locations.Add(x.EndLocation);
                locations.Add(x.StartLocation);
            });
            locations = locations.Distinct().ToList();
            bookingSystem = new CuBookingSystem("CU_BOOKINGSYSTEM", null, this, inputTimeTable);
            network = new CuNetwork("CU_NETWORK", bookingSystem, this);
            shuntingYards = new ControlUnit[locations.Count];
            var index = 0;
            foreach (var x in locations)
            {
                var tmp = new CuShuntingYard(x, network, this);
                shuntingYards[index++] = tmp;
                AllShuntingYards.Instance.SetYards(x, tmp);
            }

            network.SetChildControlUnits(shuntingYards);

            bookingSystem.SetChildControlUnits(new ControlUnit[] { network });


            _rootControlUnit = bookingSystem;
        }

        public override void CustomInitializeModel()
        {
        }

        public override string GetModelString()
        {
            throw new NotImplementedException();
        }

        public override void CreateSimulationResultsAfterStop()
        {
            foreach (var yard in shuntingYards)
            {
                var sortingRequest = yard.RAEL.Where(p => p.Activity == Constants.RequestForSorting)
                    .Cast<RequestSorting>()
                    .ToList();
                if (sortingRequest.Count != 0)
                {
                    foreach (var request in sortingRequest)
                    {
                        var wagon = (EntityWagon)request.Origin[0];
                        var calculatedTime = wagon.EndTime;
                        var real_time = ((RequestSorting)request).ArrivalTime;
                        wagon.RealTime = real_time;
                        wagon.TimeDelta = Math.Abs((calculatedTime - real_time).TotalMinutes);
                    }
                }
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Use this for EPPlus 5.0 onwards
            using (ExcelPackage package =
                   new ExcelPackage(new FileInfo(@"C:\Users\koenig11\RiderProjects\HCDESLib\RailCargo\result.xlsx")))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];
                worksheet.Cells.Clear();
                worksheet.Cells[1, 1].Value = "WagonId";
                worksheet.Cells[1, 2].Value = "Timedelta";
                worksheet.Cells[1, 3].Value = "Calculated Time";
                worksheet.Cells[1, 4].Value = "Actual Time";
                worksheet.Cells[1, 5].Value = "Booked Time";
                worksheet.Cells[1, 6].Value = "Acceptance Date";
                worksheet.Cells[1, 7].Value = "Trains used";
                worksheet.Cells[1, 8].Value = "Recalculated on";
                var counter = 2;
                foreach (var wagon in CuBookingSystem.AllWagons)
                {
                    worksheet.Cells[counter, 1].Value = wagon.WagonId.ToString(); // Writes "Hello" to cell A1
                    worksheet.Cells[counter, 2].Value = wagon.TimeDelta.ToString();
                    worksheet.Cells[counter, 3].Value = wagon.EndTime.ToString();
                    var realtime = wagon.RealTime.ToString();
                    if (realtime == "01.01.0001 00:00:00") continue;
                    worksheet.Cells[counter, 4].Value = wagon.RealTime.ToString();
                    Helper.Print(wagon.RealTime.ToString());
                    worksheet.Cells[counter, 5].Value = wagon.BookingDate.ToString();
                    Helper.Print(wagon.BookingDate.ToString());
                    worksheet.Cells[counter, 6].Value = wagon.AcceptanceDate.ToString();
                    Helper.Print(wagon.AcceptanceDate.ToString());
                    worksheet.Cells[counter, 7].Value = "Calculated Trains: " +
                                                        Helper.StringifyCollection(wagon.UsedTrains) + "\n" +
                                                        "Actual Trains: " +
                                                        Helper.StringifyCollection(wagon.ActualDepartureTime);
                    worksheet.Cells[counter, 7].Style.WrapText = true;
                    worksheet.Cells[counter, 8].Value = Helper.StringifyCollection(wagon.RebookedTimes);
                    counter += 1;
                }

                package.Save();
            }
        }
    }
}