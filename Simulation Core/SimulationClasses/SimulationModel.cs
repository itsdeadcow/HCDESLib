using SimulationCore.HCCMElements;
using SimulationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SimulationCore.SimulationClasses
{
    /// <summary>
    /// Abstract base class of simulation models
    /// </summary>
    abstract public class SimulationModel
    {
        
        #region Contructor

        /// <summary>
        /// Basic constructor with start and end time
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        public SimulationModel(DateTime startTime, DateTime endTime)
        {
            _startTime = startTime;
            _endTime = endTime;
            _entityTracker = new Dictionary<Type, List<Entity>>();

            _controlUnits = new Dictionary<string, ControlUnit>();
            
        } // end of SimulationModel

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods 
        //--------------------------------------------------------------------------------------------------

        #region SetStartEndTime

        /// <summary>
        /// Start and end time may be altered after construction, but before execution
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        public void SetStartEndTime(DateTime startTime, DateTime endTime)
        {
            _startTime = startTime;
            _endTime = endTime;
        } // end of SetStartEndTime

        #endregion

        #region CustomInitializeModel

        /// <summary>
        /// Method to custom initialize model
        /// </summary>
        /// <param name="startTime"></param>
        abstract public void CustomInitializeModel();

        #endregion

        #region InitializeVisualization

        /// <summary>
        /// Method to initialize model
        /// </summary>
        /// <param name="args">Possible visualization arguments as a graphics object or a wpf canvas
        /// that is required for csustom visualization</param>
        virtual public void InitializeVisualization(object args = null)
        { 
        
        } // end of InitializeVisualization

        #endregion

        #region Initialize

        /// <summary>
        /// Default initialization of model. Adds and initializes all control units
        /// and calls the custom initalize method
        /// </summary>
        /// <param name="simEngine"></param>
        public void Initialize(ISimulationEngine simEngine)
        {
            _simEngine = simEngine;
            AddControlUnit(RootControlUnit);

            RootControlUnit.Initialize(StartTime, simEngine);

            CustomInitializeModel();

        }

        public ISimulationEngine SimEngine => _simEngine;
        // end of Initialize

        #endregion

        #region AddControlUnit

        /// <summary>
        /// Recursive function to add all control units to the model
        /// </summary>
        /// <param name="control">Current control unit in the tree</param>
        private void AddControlUnit(ControlUnit control)
        {
            _controlUnits.Add(control.Name, control);

            foreach (ControlUnit childControl in control.ChildControlUnits)
            {
                AddControlUnit(childControl);
            } // end foreach

        } // end of AddControlUnit

        #endregion

        #region AddEntityToTracker

        /// <summary>
        /// Adds an entity to the tracker
        /// </summary>
        /// <param name="entitiy">Entity to add</param>
        public void AddEntityToTracker(Entity entitiy)
        {
            if (!_entityTracker.ContainsKey(entitiy.GetType()))
                _entityTracker.Add(entitiy.GetType(), new List<Entity>());

            _entityTracker[entitiy.GetType()].Add(entitiy);
        } // end of AddEntitiyToTracker

        #endregion

        #region GetEntitiesOfType

        /// <summary>
        /// All entities of a certain type stored in the tracker
        /// </summary>
        /// <typeparam name="T">Type of the entities that need to be obtained</typeparam>
        /// <returns>A list of all entities of Type T tracked</returns>
        public List<T> GetEntitiesOfType<T>() where T : Entity
        {
            if (_entityTracker.ContainsKey(typeof(T)))
                return _entityTracker[typeof(T)].Cast<T>().ToList();

            return new List<T>();
        } // end of GetEntitiesOfType


        #endregion

        #region ResetModel

        /// <summary>
        /// Resets the model, clears the entity tracker
        /// </summary>
        public virtual void ResetModel()
        {
            _entityTracker = new Dictionary<Type, List<Entity>>();
        } // end of ResetTracker

        #endregion

        #region StopSimulation

        /// <summary>
        /// Can be overwritten by the user to define stopping criteria of the model.
        /// In its basic version soley time based.
        /// </summary>
        /// <param name="currentTime">Current time of model execution</param>
        /// <returns>Returns true if simulation should be stopped</returns>
        public virtual bool StopSimulation(DateTime currentTime)
        {
            return currentTime >= EndTime;
        } // end of StopSimulation

        #endregion

        #region SimulationProgress

        /// <summary>
        /// Gets the simulation progress of the model
        /// </summary>
        /// <param name="currentTime">Current time of the simulation execution</param>
        /// <returns>Percentage of simulation progress</returns>
        public virtual int GetSimulationProgress(DateTime currentTime)
        {
            double secEnd = (currentTime - StartTime).TotalSeconds / (Duration).TotalSeconds;

            return (Math.Min((int)(secEnd * 100), 100));
        } // end of GetSimulationProgress

        #endregion

        #region CreateSimulationResultsAfterStop

        /// <summary>
        /// Virtual method to be overwritten if actions at end of model execution are required.
        /// </summary>
        public virtual void CreateSimulationResultsAfterStop()
        {

        } // end of DisplaySimulationResultsAfterStop

        #endregion

        #region GetModelString

        /// <summary>
        /// String representation of the model
        /// </summary>
        /// <returns></returns>
        public abstract string GetModelString();

        #endregion

        #region PerformModelRules

        /// <summary>
        /// Performs model's conditional rules (represented by the rule sets of control units) to be performed
        /// after triggering of scheduled events at specific time
        /// </summary>
        /// <param name="currentlyTriggeredScheduledEvents">Scheduled events triggered at current time</param>
        /// <param name="currentTime">Current time of model execution</param>
        /// <param name="simEngine">SimEngine responsible for simulation execution</param>
        public void PerformModelRules(IEnumerable<Event> currentlyTriggeredScheduledEvents, DateTime currentTime, ISimulationEngine simEngine)
        {
            RootControlUnit.SetRecursiveNeedsUpdateToFalse();

            foreach (Event ev in currentlyTriggeredScheduledEvents)
            {
                ev.ParentControlUnit.BehaviorOccured = true;
            } // end foreach

            bool eventLaunched = true;

            // If new controlled behavior was launched we keep looping
            while (eventLaunched)
            {
                RootControlUnit.PerformRules(currentTime, out eventLaunched, simEngine);
            } // end while

        } // end of PerformModelRules

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region StartTime

        protected DateTime _startTime;

        public DateTime StartTime
        {
            get
            {
                return _startTime;
            }
        } // end of StartTime

        #endregion

        #region EndTime

        protected DateTime _endTime;

        public DateTime EndTime
        {
            get
            {
                return _endTime;
            }
        } // end of EndTime

        #endregion

        #region Duration

        public TimeSpan Duration
        {
            get
            {
                return EndTime - StartTime;
            }
        } // end of Duration

        #endregion

        #region RootControlUnit

        protected ControlUnit _rootControlUnit;

        /// <summary>
        /// Root control unit of control tree
        /// </summary>
        public ControlUnit RootControlUnit
        {
            get
            {
                return _rootControlUnit;
            }
        } // end of RootControlUnit

        #endregion

        #region ControlUnits

        protected Dictionary<string,ControlUnit> _controlUnits;

        /// <summary>
        /// List of all control units in the tree
        /// </summary>
        public Dictionary<string,ControlUnit> ControlUnits
        {
            get
            {
                return _controlUnits;
            }
        } // end of ControlUnits

        #endregion        

        #region EntityTracker

        protected Dictionary<Type, List<Entity>> _entityTracker;

        /// <summary>
        /// Stores all created entities in the simulation run per type
        /// </summary>
        public Dictionary<Type, List<Entity>> EntityTracker
        {
            get
            {
                return _entityTracker;
            }
        } // end of EntityTracker

        #endregion

        #region SimulationDrawingEngine

        protected IDrawingSimulationEngine _simulationDrawingEngine;
        private ISimulationEngine _simEngine;

        public IDrawingSimulationEngine SimulationDrawingEngine
        {
            get
            {
                return _simulationDrawingEngine;
            }
        } // end of SimulationDrawingEngine

        #endregion

    }
}
