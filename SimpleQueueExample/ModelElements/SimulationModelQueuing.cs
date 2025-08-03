using SimpleQueueExample.Drawing;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;
using SimulationWPFVisualizationTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFVisualizationBase;

namespace SimpleQueueExample.ModelElements
{
    /// <summary>
    /// This model implements a simple queuing example where the number of queues and servers
    /// can be specified as parameters of the model
    /// </summary>
    public class SimulationModelQueuing : SimulationModel
    {

        //--------------------------------------------------------------------------------------------------
        // Constructor 
        //--------------------------------------------------------------------------------------------------

        #region SimulationModelQueuing

        /// <summary>
        /// Basic constructor, just asssigns parameters
        /// </summary>
        /// <param name="startTime">Start time of simulation</param>
        /// <param name="endTime">End time of simulation</param>
        /// <param name="numberServers">Number of servers modeled</param>
        /// <param name="numberQueues">Number of queues for servers modeled</param>
        /// <param name="arrivalTime">Exponential mean of inter-arrival times</param>
        /// <param name="serviceTime">Exponential mean of service times</param>
        public SimulationModelQueuing(DateTime startTime, 
            DateTime endTime, 
            int numberServers, 
            int numberQueues,
            double arrivalTime,
            double serviceTime)
            : base(startTime, endTime)
        {
            _arrivalTime = arrivalTime;
            _serviceTime = serviceTime;
            _numberServers = numberServers;
            _numberQueues = numberQueues;          

            _rootControlUnit = new ControlUnitQueuingModel("QueuingControl", null, this, NumberQueues, NumberServers);

        } // end of Constructor

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods 
        //--------------------------------------------------------------------------------------------------

        #region CustomInitializeModel

        public override void CustomInitializeModel()
        {
        } // end of CustomInitializeModel

        #endregion
        

        public override string GetModelString() => "QueuingModel";  // end of GetModelString
        

        #region ResetModel

        /// <summary>
        /// Resets whole model for re-execution in gui
        /// </summary>
        public override void ResetModel()
        {
            _entityTracker = new Dictionary<Type, List<Entity>>();

            _rootControlUnit = new ControlUnitQueuingModel("QueuingControl", null, this, NumberQueues, NumberServers);
            _controlUnits = new Dictionary<string, ControlUnit>();

            EntityClient.RunningID = 0;
            EntityQueue.RunningID = 0;
            EntityServer.RunningID = 0;

        } // end of ResetModel

        #endregion

        #region InitializeVisualization

        public override void InitializeVisualization(object args = null)
        {
            _simulationDrawingEngine = new BaseWPFModelVisualization(this, (DrawingOnCoordinateSystem)args);

            ((BaseWPFModelVisualization)SimulationDrawingEngine).VisualizationPerControlUnit.Add(RootControlUnit, new QueueControlVisualizationEngine((DrawingOnCoordinateSystem)args));
        } // end of InitializeVisualization

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Members 
        //--------------------------------------------------------------------------------------------------
        
        #region NumberServers
        
        private int _numberServers;
        
        /// <summary>
        /// Number of servers modeled
        /// </summary>
        public int NumberServers
        {
          get
          {
            return _numberServers;
          }
        } // end of NumberServers
        
        #endregion

        #region NumberQueues
        
        private int _numberQueues;
        
        /// <summary>
        /// Number of queues for servers modeled
        /// </summary>
        public int NumberQueues
        {
          get
          {
            return _numberQueues;
          }
        } // end of NumberQueues
        
        #endregion

        #region ArrivalTime

        private double _arrivalTime;

        /// <summary>
        /// Exponential mean of inter-arrival times
        /// </summary>
        public double ArrivalTime
        {
            get
            {
                return _arrivalTime;
            }
        } // end of ArrivalTime

        #endregion

        #region ServiceTime

        private double _serviceTime;

        /// <summary>
        /// Exponential mean of service times
        /// </summary>
        public double ServiceTime
        {
            get
            {
                return _serviceTime;
            }
        } // end of ServiceTime

        #endregion

    } // end of SimulationModelQueuing
}
