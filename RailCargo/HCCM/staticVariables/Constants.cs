using System;
using System.Collections.Generic;

namespace RailCargo.HCCM.staticVariables
{
    public class Constants
    {
        //---------------------- integer values
        public static readonly int TimeToWaitForSilo = 2;

        public static Dictionary<string, int> TrainCapacity = new Dictionary<string, int>
            { { "Fast", 5 }, { "Slow", 2 } };

        //---------------------- requests
        public static readonly string RequestForSilo = "REQUEST_FOR_SILO";
        public static string RequestForDepartureArea = "REQUEST_FOR_DEPARTURE_AREA";
        public static string RequestForDeparture = "REQUEST_FOR_DEPARTURE";
        public static string RequestForSorting = "REQUEST_FOR_SORTING";
        public static string RequestForSiloStatus = "REQUEST_FOR_SILO_STATUS";
        public static string RequestTrainContinuation = "REQUEST_TRAIN_CONTINUATION";

        //---------------------- activity
        public static readonly string ActivityWaitingForSilo = "ACTIVITY_WAITING_FOR_SILO";
        public static string ActivityDriveToDepartureArea = "ACTIVITY_DRIVE_TO_DEPARTURE_AREA";
        public static string ActivityWagonCollection = "ACTIVITY_WAGON_COLLECTION";
        public static string ActivityWaitingForDeparture = "ACTIVITY_WAITING_FOR_DEPARTURE";
        public static string ActivityTrainDrive = "ACTIVITY_TRAIN_DRIVE";
        public static string ActivityWaitingForTrainSelectionSilo = "ACTIVITY_WAITING_FOR_TRAIN_SELECTION_SILO";
        public static string ActivityWaitingInSilo = "ACTIVITY_WAITING_IN_SILO";
        public static string ActivityShuntingWagon = "ACTIVITY_SHUNTING_WAGON";
        public static string ActivityWaitingForTrainSelectionWagon = "ACTIVITY_WAITING_FOR_TRAIN_SELECTION_WAGON";
        public static string ActivityWaitingForAllowance = "ACTIVITY_WAITING_FOR_ALLOWANCE";
        public static string ActivityTrainPreparation = "ACTIVITY_TRAIN_PREPARATION";
        public static string ActivityShuntingWagons = "ACTIVITY_SHUNTING_WAGONS";
        public static string ActivityTrainWaitingForDeparture = "ACTIVITY_TRAIN_WAITING_FOR_DEPARTURE";
        
    }
}