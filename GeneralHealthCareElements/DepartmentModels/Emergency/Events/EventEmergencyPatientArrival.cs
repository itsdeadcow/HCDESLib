using SimulationCore.MathTool;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulationCore.HCCMElements;
using GeneralHealthCareElements.Entities;
using SimulationCore.SimulationClasses;
using GeneralHealthCareElements.Activities;
using System;

namespace GeneralHealthCareElements.DepartmentModels.Emergency
{
    /// <summary>
    /// Event that is triggered when patient arrives at emergency department
    /// </summary>
    public class EventEmergencyPatientArrival : Event
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="parentControlUnit">Parent emergency department control</param>
        /// <param name="patient">Patient that arrives</param>
        /// <param name="inputData">Correpsonding Emergency Input data</param>
        public EventEmergencyPatientArrival(ControlUnit parentControlUnit, EntityPatient patient,
            IInputEmergency inputData)
            : base(EventType.Standalone, parentControlUnit)
        {
            _patient = patient;
            _inputData = inputData;
        } // end of Event

        #endregion

        //--------------------------------------------------------------------------------------------------
        // State Change
        //--------------------------------------------------------------------------------------------------

        #region StateChange

        /// <summary>
        /// Overriden state change of the event. If the patient is arrving externaly then the next arrival is
        /// scheduled. Else the path is updated and the next/first action is taken from the path.
        /// </summary>
        /// <param name="time">Time the patient arrives</param>
        /// <param name="simEngine">SimEngine responsible for simulation execution</param>
        protected override void StateChange(DateTime time, ISimulationEngine simEngine)
        {
            // if patient has a path it is updated
            if (Patient.EmergencyTreatmentPath != null)
            {
                Patient.EmergencyTreatmentPath.UpdateNextAction();
            }
            else
            {
                // if patient has no path he is arriving externaly

                // path is created
                Patient.EmergencyTreatmentPath =
                    ((ControlUnitEmergency)ParentControlUnit).InputData.CreateEmergencyPath(Patient);

                // patient is added to control unit
                ParentControlUnit.AddEntity(Patient);

                // arrival of next patient is created and scheduled
                EntityPatient nextPatient = ((ControlUnitEmergency)ParentControlUnit).InputData.GetNextPatient();

                EventEmergencyPatientArrival nextPatientArrival =
                    new EventEmergencyPatientArrival(ParentControlUnit, nextPatient, InputData);

                simEngine.AddScheduledEvent(nextPatientArrival,
                    time + ((ControlUnitEmergency)ParentControlUnit).InputData.PatientArrivalTime(time));
            } // end if

            // next action on path is taken
            if (Patient.EmergencyTreatmentPath.TakeNextAction(simEngine, this, time, ParentControlUnit))
            {
                // possible waiting or waiting in facility is triggered
                if (Patient.OccupiedFacility == null ||
                    Patient.OccupiedFacility.ParentDepartmentControl != ParentControlUnit)
                {
                    SequentialEvents.Add(Patient.StartWaitingActivity(
                        ((ControlUnitEmergency)ParentControlUnit).WaitingAreaPatientForNextActionType(
                            Patient.EmergencyTreatmentPath.GetCurrentActionType())));
                }
                else
                {
                    ActivityWaitInFacility waitInFacility =
                        new ActivityWaitInFacility(ParentControlUnit, Patient, Patient.OccupiedFacility);
                    SequentialEvents.Add(waitInFacility.StartEvent);
                } // end if
            } // end if
        } // end of Trigger

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Members 
        //--------------------------------------------------------------------------------------------------

        #region InputData

        private IInputEmergency _inputData;

        /// <summary>
        /// Emergency department input data
        /// </summary>
        public IInputEmergency InputData
        {
            get { return _inputData; }
        } // end of InputData

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Affected Entities
        //--------------------------------------------------------------------------------------------------

        #region Patient

        private EntityPatient _patient;

        /// <summary>
        /// Arriving patient
        /// </summary>
        public EntityPatient Patient
        {
            get { return _patient; }
        } // end of Patient

        #endregion

        #region AffectedEntites

        /// <summary>
        /// Affected entities only consist of the patient
        /// </summary>
        public override Entity[] AffectedEntities
        {
            get { return new Entity[] { Patient }; }
        } // end of AffectedEntities

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region ToString

        public override string ToString()
        {
            return "EventPatientArrival";
        } // end of ToString

        #endregion

        #region Clone

        override public Event Clone()
        {
            return new EventEmergencyPatientArrival(ParentControlUnit, (EntityPatient)Patient.Clone(), InputData);
        } // end of Clone

        #endregion
    }
}