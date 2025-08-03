using GeneralHealthCareElements.Activities;
using GeneralHealthCareElements.BookingModels;
using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.GeneralClasses.ActionTypesAndPaths;
using GeneralHealthCareElements.ResourceHandling;
using GeneralHealthCareElements.SpecialFacility;
using GeneralHealthCareElements.TreatmentAdmissionTypes;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleHospitalModel.Diagnostics
{
    /// <summary>
    /// Sample control unit implementing a basic diagnostic department with XRay, MRI, CT and
    /// Lab facilities
    /// </summary>
    public class ControlUnitSpecialTreatmentModelDiagnostics : ControlUnitSpecialServiceModel
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="name">Name of control unit</param>
        /// <param name="parentControlUnit">Parent management control</param>
        /// <param name="parentSimulationModel">Simulation model the control unit belongs to</param>
        /// <param name="admissionsAvailable">Admission types handled by control unit</param>
        /// <param name="waitingListSchedule">Waiting list schedule responsible for booking of diagnostic assessments</param>
        /// <param name="inputData">Input data responsible for diagnostic control</param>
        public ControlUnitSpecialTreatmentModelDiagnostics(string name,
            ControlUnit parentControlUnit,
            SimulationModel parentSimulationModel,
            SpecialServiceAdmissionTypes[] admissionsAvailable,
            EntityWaitingListSchedule waitingListSchedule,
            IInputSpecialFacility inputData)
            : base(name,
                parentControlUnit,
                admissionsAvailable,
                parentSimulationModel,
                waitingListSchedule,
                inputData)
        {

        } // end of ControlUnit

        #endregion

        #region Initialize

        /// <summary>
        /// Just adds the waiting list schedule to the controlled entities and intializes it
        /// </summary>
        /// <param name="startTime">Start time of the simulation model</param>
        /// <param name="simEngine">SimEngine responsible for simulation execution</param>
        protected override void CustomInitialize(DateTime startTime, ISimulationEngine simEngine)
        {
            AddEntity(WaitingListSchedule);

            WaitingListSchedule.Initialize(startTime);

        } // end of Initialize

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Rule Handling
        //--------------------------------------------------------------------------------------------------

        #region PerformCustomRules

        /// <summary>
        /// Custom rules of diagnostic control, just calls dispatching method as long as no further event is triggered
        /// </summary>
        /// <param name="startTime">Time rules are executed</param>
        /// <param name="simEngine">SimEngine responsible for simulation execution</param>
        /// <returns>False</returns>
        protected override bool PerformCustomRules(DateTime time, ISimulationEngine simEngine)
        {
            bool eventLaunched = false;

            while (PerformDisptatching(time, simEngine))
            {
                eventLaunched = true;
            } // end while

            return eventLaunched;

        } // end of PerformCustomRules

        #endregion

        #region PerformDisptatching

        /// <summary>
        /// Dispatches all requests with respect to FIFO rule, does not distinguish between
        /// different treatments (XRay, MRI, CT, Lab)
        /// </summary>
        /// <param name="startTime">Start time of the simulation model</param>
        /// <param name="simEngine">SimEngine responsible for simulation execution</param>
        /// <returns>False</returns>
        private bool PerformDisptatching(DateTime time, ISimulationEngine simEngine)
        {
            List<RequestSpecialFacilityAction> actionRequests =
                RAEL.Where(p => p.GetType() == typeof(RequestSpecialFacilityAction))
                    .Cast<RequestSpecialFacilityAction>().ToList();

            while (actionRequests.Count > 0)
            {
                ResourceSet chosenResources;

                // Get action request Triage-FIFO
                RequestSpecialFacilityAction requestAction =
                    PatientPriorityPlusFIFO<RequestSpecialFacilityAction, SpecialServiceActionTypeClass>(
                        actionRequests);

                actionRequests.Remove(requestAction);

                if (!ChooseResourcesForAction(requestAction,
                        out chosenResources))
                {
                    continue;
                } // enf if

                // Remove Request from RAEL list
                RemoveRequest(requestAction);
                EntityPatient patient = (EntityPatient)requestAction.Origin.First();

                ActivityHealthCareAction<SpecialServiceActionTypeClass> action =
                    new ActivityHealthCareAction<SpecialServiceActionTypeClass>(
                        this,
                        InputData,
                        patient,
                        chosenResources,
                        requestAction.ActionType,
                        patient.SpecialFacilityPath);

                chosenResources.StopCurrentActivities(time, simEngine);
                patient.StopCurrentActivities(time, simEngine);
                action.StartEvent.Trigger(time, simEngine);

            } // end while

            return false;

        } // end of PerformDisptatching

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Enter Leave
        //--------------------------------------------------------------------------------------------------

        #region EntityEnterControlUnit

        /// <summary>
        /// Handles arriving patients by a EventSpecialFacilityPatientArrival event
        /// </summary>
        /// <param name="time">Time patient enters the control unit</param>
        /// <param name="simEngine">SimEngine responsible for simulation execution</param>
        /// <param name="entity">Entering entity, for this control a patient</param>
        /// <param name="originDelegate">The delegate that is responsible for entering</param>
        /// <returns>The entering event of the patient</returns>
        public override Event EntityEnterControlUnit(DateTime time, ISimulationEngine simEngine, Entity entity,
            IDelegate originDelegate)
        {
            return new EventSpecialFacilityPatientArrival(this, (EntityPatient)entity,
                ((RequestSpecialFacilitiyService)originDelegate), InputData);
        } // end of EntityEnterControlUnit

        #endregion

        #region EntityLeaveControlUnit

        /// <summary>
        /// Empty, no actions required
        /// </summary>
        /// <param name="time"></param>
        /// <param name="simEngine"></param>
        /// <param name="entity"></param>
        /// <param name="originDelegate"></param>
        public override void EntityLeaveControlUnit(DateTime time, ISimulationEngine simEngine, Entity entity,
            IDelegate originDelegate)
        {

        } // end of EntityLeaveControlUnit

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Custom Methods
        //--------------------------------------------------------------------------------------------------

        #region ChooseResourcesForAction

        /// <summary>
        /// Helping method to choose resource for action requests, first available resources
        /// that satisfy requirements are chosen
        /// </summary>
        /// <param name="actionRequest">Request for that resources are looked for</param>
        /// <param name="resources">Out parameter that represent the assigned resources</param>
        /// <returns>True if all resources have been assigned, false else</returns>
        public bool ChooseResourcesForAction(
            RequestSpecialFacilityAction actionRequest,
            out ResourceSet resources)
        {
            resources = new ResourceSet();

            #region Doctors

            List<EntityDoctor> chosenDoctors = new List<EntityDoctor>();

            #region MainDoc

            if (actionRequest.ActionType.MainDoctorRequirements != null)
            {
                if (actionRequest.ResourceSet.MainDoctor != null)
                {
                    if (actionRequest.ResourceSet.MainDoctor.IsWaiting())
                    {
                        chosenDoctors.Add(resources.MainDoctor);
                        resources.MainDoctor = actionRequest.ResourceSet.MainDoctor;
                    }
                    else
                    {
                        return false;
                    } // end if
                }
                else
                {

                } // end if

            } // end if

            #endregion

            #region AssistingDoctors

            if (actionRequest.ActionType.AssistingDoctorRequirements != null)
            {

                if (actionRequest.ResourceSet.AssistingDoctors != null)
                {
                    foreach (EntityDoctor doctor in actionRequest.ResourceSet.AssistingDoctors)
                    {
                        if (!doctor.IsWaiting())
                            return false;
                    } // end foreach
                }
                else
                {
                    List<EntityDoctor> foundDoctors = new List<EntityDoctor>();

                    List<EntityDoctor> allDoctors = new List<EntityDoctor>(ControlledDoctors);

                    foreach (SkillSet skillSet in actionRequest.ActionType.AssistingDoctorRequirements)
                    {
                        EntityDoctor foundDoc = null;

                        foreach (EntityDoctor doc in allDoctors)
                        {
                            if (doc.IsWaiting() &&
                                doc.SatisfiesSkillSet(skillSet)
                                && !chosenDoctors.Contains(doc)
                                && !doc.StaffOutsideShift)
                            {
                                foundDoc = doc;
                                break;
                            } // end if
                        } // end foreach

                        if (foundDoc == null)
                        {
                            return false;
                        }
                        else
                        {
                            allDoctors.Remove(foundDoc);
                            foundDoctors.Add(foundDoc);
                            chosenDoctors.Add(foundDoc);
                        } // end if

                    } // end foreach

                    resources.AssistingDoctors = foundDoctors.ToArray();

                } // end if

            } // end if

            #endregion

            #endregion

            #region Nurses

            List<EntityNurse> chosenNurses = new List<EntityNurse>();

            #region MainNurse

            if (actionRequest.ActionType.MainNurseRequirements != null)
            {
                if (actionRequest.ResourceSet.MainNurse != null)
                {
                    if (actionRequest.ResourceSet.MainNurse.IsWaiting())
                    {
                        chosenNurses.Add(resources.MainNurse);
                        resources.MainNurse = actionRequest.ResourceSet.MainNurse;
                    }
                    else
                    {
                        return false;
                    } // end if
                }
                else
                {
                    List<EntityNurse> possibleDocs = ControlledNurses.Where(p => p.IsWaiting()
                        && p.SatisfiesSkillSet(actionRequest.ActionType.MainNurseRequirements)
                        && !p.StaffOutsideShift).ToList();

                    if (possibleDocs.Count == 0)
                        return false;

                    resources.MainNurse = possibleDocs.First();
                    chosenNurses.Add(resources.MainNurse);
                } // end if

            } // end if

            #endregion

            #region AssistingNurses

            if (actionRequest.ActionType.AssistingNurseRequirements != null)
            {

                if (actionRequest.ResourceSet.AssistingNurses != null)
                {
                    foreach (EntityNurse nurse in actionRequest.ResourceSet.AssistingNurses)
                    {
                        if (!nurse.IsWaiting())
                            return false;
                    } // end foreach
                }
                else
                {
                    List<EntityNurse> foundNurses = new List<EntityNurse>();

                    List<EntityNurse> allNurses = new List<EntityNurse>(ControlledNurses);

                    foreach (SkillSet skillSet in actionRequest.ActionType.AssistingNurseRequirements)
                    {
                        EntityNurse foundDoc = null;

                        foreach (EntityNurse nurse in allNurses)
                        {
                            if (nurse.IsWaiting() &&
                                nurse.SatisfiesSkillSet(skillSet)
                                && !chosenNurses.Contains(nurse)
                                && !nurse.StaffOutsideShift)
                            {
                                foundDoc = nurse;
                                break;
                            } // end if
                        } // end foreach

                        if (foundDoc == null)
                        {
                            return false;
                        }
                        else
                        {
                            allNurses.Remove(foundDoc);
                            foundNurses.Add(foundDoc);
                            chosenNurses.Add(foundDoc);
                        } // end if

                    } // end foreach

                    resources.AssistingNurses = foundNurses.ToArray();

                } // end if

            } // end if

            #endregion

            #endregion

            #region TreatmentFacilities

            bool foundFacility = false;

            foreach (EntityTreatmentFacility fac in AssignedTreatmentFacilities.Where(p => !p.BlockedForPatient))
            {
                if (!fac.Occupied && fac.SatisfiesSkillSet(actionRequest.ActionType.FacilityRequirements))
                {
                    resources.TreatmentFacility = fac;
                    foundFacility = true;
                    break;
                } // end if                     
            } // end foreach

            if (!foundFacility)
                return false;

            #endregion

            return true;
        } // end of ChooseResourcesForAction

        #endregion
    }
} // end of DiagnosticControlUnit
