using SimulationCore.HCCMElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleHospitalModel.ModelLog
{
    /// <summary>
    /// Creates a state of a control unit for logging
    /// </summary>
    public class LogControlUnitState
    {

        #region Constructor

        /// <summary>
        /// String representation of state is created
        /// </summary>
        /// <param name="timeStamp">Time for which the state is generated</param>
        /// <param name="loggingEngine">The logging engine for which the state is generated</param>
        /// <param name="controlUnit">The control unit for which the state is generated</param>
        public LogControlUnitState(DateTime timeStamp,
                                    BaseLoggingEngine loggingEngine,
                                    ControlUnit controlUnit)
        {
            StringBuilder description = new StringBuilder();

            description.Append(timeStamp.ToString("MM.dd:HH:mm:ss:fff")).Append(": ").Append(controlUnit.Name).Append(": RAEL length: ").Append(controlUnit.RAEL.Count).Append(", ");

            foreach (KeyValuePair<Type, HashSet<Entity>> entityType in controlUnit.ControlledEntities)
            {
                description.Append(loggingEngine.GetStringRepDefaultEntityType(entityType.Key)).Append(": ,").Append(entityType.Value.Count).Append(", ");
            } // end foreach

            foreach (KeyValuePair<string, List<Activity>> activityType in controlUnit.CurrentActivitiesPerType)
            {
                description.Append(activityType.Key).Append(": ,").Append(activityType.Value.Count).Append(", ");
            } // end foreach

            description.AppendLine();

            foreach (ControlUnit child in controlUnit.ChildControlUnits)
            {
                LogControlUnitState childState = new LogControlUnitState(timeStamp, loggingEngine, child);
                description.Append(childState.GetDescription());
            } // end foreach

            //_logState = description.ToString();
        } // end of LogControlUnitState

        #endregion

        #region LogState

        private string _logState;

        /// <summary>
        /// String state of the control unit
        /// </summary>
        public string LogState
        {
            get
            {
                return _logState;
            }
        } // end of LogState

        #endregion

        #region GetDescription

        /// <summary>
        /// Returns the state description
        /// </summary>
        /// <returns></returns>
        public string GetDescription()
        {
            return _logState;
        } // end of GetDescription

        #endregion
    }
}
