using SampleHospitalModel;
using SampleHospitalModel.ModelLog;
using SimpleQueueExample.ModelElements;
using SimulationCore.HCCMElements;
using RailCargo;
using SimulationCore.Interfaces;
using SimulationCore.SimulationClasses;
using SimulationWPFVisualizationTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using RailCargo.HCCM.ControlUnits;
using WPFVisualizationBase;

namespace HiCoSimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //--------------------------------------------------------------------------------------------------
        // Constructor 
        //--------------------------------------------------------------------------------------------------

        #region MainWindow

        public MainWindow()
        {
            Loaded += WainWindowOnLoad;

            InitializeComponent();

            #region InitalizeStartEndDate

            DatePickerStartDate.SelectedDate = DateTime.Now;
            DatePickerEndDate.SelectedDate = DateTime.Now.AddMonths(1);

            #endregion

            #region InitiaizeDrawingSystem

            _drawingSystem = new DrawingOnCoordinateSystem(0, 300);
            DrawingSystem.VerticalAlignment = VerticalAlignment.Stretch;
            DrawingSystem.HorizontalAlignment = HorizontalAlignment.Stretch;
            DrawingSystem.SetValue(Grid.RowProperty, 1);
            DrawingSystem.SetValue(Grid.ColumnProperty, 1);
            DrawingSystem.SetValue(Grid.ColumnSpanProperty, 2);

            MainGrid.Children.Add(DrawingSystem);

            #endregion

            #region IntiliazeBackgroundWorker

            _backGroundWorkerNonVisualization = new BackgroundWorker();
            _backGroundWorkerNonVisualization.WorkerReportsProgress = true;
            _backGroundWorkerNonVisualization.WorkerSupportsCancellation = true;
            _backGroundWorkerNonVisualization.DoWork +=
                new System.ComponentModel.DoWorkEventHandler(BackGroundWorkerNonVisualizationDoWork);
            _backGroundWorkerNonVisualization.ProgressChanged +=
                new ProgressChangedEventHandler(BackGroundWorkerNonVisualizationProgressChanged);
            _backGroundWorkerNonVisualization.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(BackGroundWorkerNonVisualizationRunWorkerCompleted);

            #endregion

            ComboBoxTimeBase.SelectedIndex = 0;
            _simulationRunning = false;
            _simulationInitialized = false;

            _simulationTimer = new DispatcherTimer();
            SimulationTimer.Tick += PlaySimulationTick;
            TextBoxSimSpeed.Text = SimulationSpeed.ToString();

            CreateModel();

            #region InitializeClock

            _simulationDisplayClock = new DrawAnalogClockWithDate(Colors.Black, 0, 0);
            SimulationDisplayClock.VerticalAlignment = VerticalAlignment.Stretch;
            SimulationDisplayClock.HorizontalAlignment = HorizontalAlignment.Stretch;
            SimulationDisplayClock.SetValue(Grid.RowProperty, 0);
            SimulationDisplayClock.SetValue(Grid.ColumnProperty, 2);

            MainGrid.Children.Add(SimulationDisplayClock);

            Loaded += delegate { SimulationDisplayClock.Initialize(SimulationModel.StartTime); };

            #endregion
        } // end of MainWindow

        #endregion

        #region WainWindowOnLoad

        private void WainWindowOnLoad(object sender, RoutedEventArgs e)
        {
            // SimulationModel.SimulationDrawingEngine.InitializeModelVisualizationAtTime(SimulationModel.StartTime,
            //     SimulationModel);
        } // end of WainWindowOnLoad

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Members 
        //--------------------------------------------------------------------------------------------------

        #region SimulationInitialized

        private bool _simulationInitialized;

        public bool SimulationInitialized
        {
            get { return _simulationInitialized; }
        } // end of SimulationInitialized

        #endregion

        #region SimulationRunning

        private bool _simulationRunning;

        public bool SimulationRunning
        {
            get { return _simulationRunning; }
        } // end of SimulationRunning

        #endregion

        #region SimulationDisplayClock

        private DrawAnalogClockWithDate _simulationDisplayClock;

        public DrawAnalogClockWithDate SimulationDisplayClock
        {
            get { return _simulationDisplayClock; }
        } // end of SimulationDisplayClock

        #endregion

        #region DrawingSystem

        private DrawingOnCoordinateSystem _drawingSystem;

        public DrawingOnCoordinateSystem DrawingSystem
        {
            get { return _drawingSystem; }
        } // end of DrawingSystem

        #endregion

        #region SimulationSpeed

        private double _simulationSpeed = 32;

        public double SimulationSpeed
        {
            get { return _simulationSpeed; }
            set { _simulationSpeed = Math.Max(value, 1); }
        } // end of SimulationSpeed

        #endregion

        #region SimulationTimer

        private DispatcherTimer _simulationTimer;

        public DispatcherTimer SimulationTimer
        {
            get { return _simulationTimer; }
        } // end of SimulationTimer

        #endregion

        #region VisualizationEnabled

        private bool _visualizationEnabled;

        public bool VisualizationEnabled
        {
            get { return _visualizationEnabled; }
            set { _visualizationEnabled = value; }
        } // end of VisualizationEnabled

        #endregion

        #region SimulationEngine

        private SimulationEngine _simulationEngine;

        public SimulationEngine SimulationEngine
        {
            get { return _simulationEngine; }
            set { _simulationEngine = value; }
        } // end of SimulationEngine

        #endregion

        #region SimulationModel

        private SimulationModel _simulationModel;

        public SimulationModel SimulationModel
        {
            get { return _simulationModel; }
            set { _simulationModel = value; }
        } // end of SimulationModel

        #endregion

        #region BackgroundWorkerNonVisualization

        private BackgroundWorker _backGroundWorkerNonVisualization;

        public BackgroundWorker BackgroundWorkerNonVisualization
        {
            get { return _backGroundWorkerNonVisualization; }
        } // end of BackgroundWorkerNonVisualization

        #endregion

        #region ConstantTimerStep

        private TimeSpan _constantTimeSpan;

        public TimeSpan ConstantTimerStep
        {
            get { return _constantTimeSpan; }
            set { _constantTimeSpan = value; }
        } // end of ConstantTimerStep

        #endregion

        #region TickCallIsSimulationCall

        public bool TickCallIsSimulationCall
        {
            get { return NextSimulationTime <= NextTimerTime; }
        } // end of TickCallIsSimulationCall

        #endregion

        #region NextSimulationTime

        private DateTime _nextSimulationTime;

        public DateTime NextSimulationTime
        {
            get { return _nextSimulationTime; }
        } // end of NextSimulationTime

        #endregion

        #region NextTimerTime

        private DateTime _nextTimerTime;

        public DateTime NextTimerTime
        {
            get { return _nextTimerTime; }
        } // end of NextTimerTime

        #endregion

        #region NextTime

        public DateTime NextTime
        {
            get
            {
                if (NextTimerTime.Ticks < NextSimulationTime.Ticks)
                    return NextTimerTime;
                else
                    return NextSimulationTime;
            }
        } // end of NextTime

        #endregion

        #region TimerTime

        private DateTime _timerTime;

        public DateTime TimerTime
        {
            get { return _timerTime; }
        } // end of TimerTime

        #endregion

        #region CurrentTime

        private DateTime _currentTime;

        public DateTime CurrentTime
        {
            get { return _currentTime; }
        } // end of CurrentTime

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods 
        //--------------------------------------------------------------------------------------------------

        #region RunSimulation

        public void RunSimulation(double firstWait = 0)
        {
            _simulationRunning = true;

            if (VisualizationEnabled)
            {
                SimulationTimer.Interval = TimeSpan.FromMilliseconds(firstWait);
                SimulationTimer.Start();
            }
            else
            {
                BackgroundWorkerNonVisualization.RunWorkerAsync();
            } // end if
        } // end of RunSimulation

        #endregion

        #region StopSimulation

        public void StopSimulation(bool pauseSim)
        {
            if (VisualizationEnabled)
            {
                SimulationTimer.Stop();
            }
            else
            {
                BackgroundWorkerNonVisualization.CancelAsync();
            } // end if

            _simulationRunning = false;

            if (!pauseSim)
            {
                ActionsAfterFinishingSimulationRun();
            } // end if
        } // end of StopSimulation

        #endregion

        #region PlaySimulationTick

        public void PlaySimulationTick(object sender, EventArgs e)
        {
            // security call in case the timer ticks bfore
            // it disabled by stop simulation
            if (!VisualizationEnabled)
                return;

            DateTime timeOfCall;

            if (TickCallIsSimulationCall)
            {
                timeOfCall = CurrentTime;

                DateTime newTime;
                bool modelRunning = SimulationEngine.RunSingleStepSimulationModel(CurrentTime, out newTime);

                // in case end of model is reached the simulation is stopped
                if (!modelRunning)
                {
                    StopSimulation(false);
                    return;
                } // end if

                SimulationDisplayClock.SetDateTime(CurrentTime);
                SimulationModel.SimulationDrawingEngine.CreateModelVisualization(CurrentTime, SimulationModel,
                    SimulationEngine.CurrentlyTriggeredEvents);
                ProgressBarSimulationProgress.Value = SimulationModel.GetSimulationProgress(CurrentTime);

                _nextSimulationTime = newTime;
                _currentTime = newTime;
            }
            else
            {
                timeOfCall = NextTimerTime;

                SimulationDisplayClock.SetDateTime(NextTimerTime);
                SimulationModel.SimulationDrawingEngine.CreateModelVisualization(NextTimerTime, SimulationModel,
                    new List<Event>());
                ProgressBarSimulationProgress.Value = SimulationModel.GetSimulationProgress(CurrentTime);
                _nextTimerTime += ConstantTimerStep;
            } // end if

            SimulationTimer.Interval = TimeSpan.FromMilliseconds(
                Math.Max((int)(((NextTime - timeOfCall).Ticks / ConstantTimerStep.Ticks) * SimulationSpeed), 1));
        } // end of PlaySimulationTick

        #endregion

        #region TransformControlsToSimulationGap

        public TimeSpan TransformControlsToSimulationGap()
        {
            switch (ComboBoxTimeBase.SelectedIndex)
            {
                case 0:
                    return TimeSpan.FromSeconds(1);
                case 1:
                    return TimeSpan.FromMinutes(1);
                case 2:
                    return TimeSpan.FromHours(1);
                case 3:
                    return TimeSpan.FromDays(1);
                case 4:
                    return TimeSpan.FromDays(1);
                case 5:
                    return TimeSpan.FromDays(1);
                case 6:
                    return TimeSpan.FromDays(1);
                default:
                    break;
            } // end switch

            return TimeSpan.FromDays(1);
        }

        #endregion

        #region ActionsAfterFinishingSimulationRun

        public void ActionsAfterFinishingSimulationRun()
        {
            ProgressBarSimulationProgress.Value = 100;
            ButtonPlaySimulation.Content = FindResource("Play");

            SimulationEngine.LoggingEngine.CreateSimulationResult();
            SimulationModel.CreateSimulationResultsAfterStop();
            _simulationRunning = false;
            _simulationInitialized = false;
            //SimulationModel = null;
            //SimulationModel.ControlUnits[].
        } // end of ActionsAfterFinishingSimulationRun

        #endregion

        #region NonVisualizationLoop

        public void NonVisualizationLoop()
        {
            DateTime originalTimerTime = CurrentTime;

            while (BackgroundWorkerNonVisualization.CancellationPending == false
                   && !SimulationModel.StopSimulation(CurrentTime))
            {
                DateTime newTime;
                bool modelRunning = SimulationEngine.RunSingleStepSimulationModel(_currentTime, out newTime);

                if (modelRunning)
                {
                    BackgroundWorkerNonVisualization.ReportProgress(
                        SimulationEngine.SimulationModel.GetSimulationProgress(newTime));
                    _currentTime = newTime;
                } // end if
            } // end while
        } // end of

        #endregion

        #region CreateModel

        public void CreateModel()
        {
            DrawingSystem.ClearSystem();


            /*_simulationModel = new HospitalSimulationModelWithVisualization((DateTime)DatePickerStartDate.SelectedDate,
            (DateTime)DatePickerEndDate.SelectedDate);*/
            // _simulationModel = new SimulationModelQueuing((DateTime)DatePickerStartDate.SelectedDate,
            //     (DateTime)DatePickerEndDate.SelectedDate,
            //     2,
            //     3,
            //     5,
            //     10);
            _simulationModel = new TrainNetworkSimulation(DateTime.Parse("2023-02-01 00:00:00"),
                DateTime.Parse("2023-03-31 23:59:59"));

            _simulationEngine = new SimulationEngine();
            SimulationEngine.LoggingEngine = new BaseLoggingEngine(SimulationModel);
            SimulationEngine.CreateEventLog = true;
            SimulationEngine.SimulationModel = SimulationModel;
            SimulationModel.Initialize(SimulationEngine);
            //SimulationModel.InitializeVisualization(DrawingSystem);
        } // end of CreateModel

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Button Click Methods 
        //--------------------------------------------------------------------------------------------------

        #region PlaySimulation_Click

        private void PlaySimulation_Click(object sender, RoutedEventArgs e)
        {
            // this is needed as pause and run are handled by the same button
            if (!SimulationInitialized)
            {
                if (SimulationModel == null)
                    CreateModel();

                // initialize visualization system
                SimulationModel.InitializeVisualization(DrawingSystem);

                // initialize drawing engine
                // SimulationModel.SimulationDrawingEngine.InitializeModelVisualizationAtTime(SimulationModel.StartTime,
                //     SimulationModel);

                // times are set
                _currentTime = SimulationModel.StartTime;
                _nextSimulationTime = SimulationModel.StartTime;
                _nextTimerTime = SimulationModel.StartTime + ConstantTimerStep;

                _simulationInitialized = true;
            } // end if


            SimulationTimer.Interval = TimeSpan.FromMilliseconds(SimulationSpeed);

            if (SimulationRunning)
            {
                ButtonPlaySimulation.Content = FindResource("Play");

                StopSimulation(true);
            }
            else
            {
                ButtonPlaySimulation.Content = FindResource("Pause");

                RunSimulation();
            }
        } // end of SimulationSpeed

        #endregion

        #region StopSimulation_Click

        public void StopSimulation_Click(object sender, RoutedEventArgs e)
        {
            StopSimulation(false);
        } // end of StopSimulation_Click

        #endregion

        #region ForwardSimulation_Click

        public void ForwardSimulation_Click(object sender, RoutedEventArgs e)
        {
            // Forwarding makes only sense when visualization is enabled
            // otherwise the simulation runs as fast as possible with
            // no delays
            if (!VisualizationEnabled || !SimulationRunning)
                return;

            // stop the current timer
            StopSimulation(true);

            // set next timer time to the simulation time plus the constant timer step
            // this will cause a simulation execution of the timer
            _nextTimerTime = _nextSimulationTime + ConstantTimerStep;

            // re-start the timer
            RunSimulation();
        } // end of ForwardSimulation_Click

        #endregion

        //--------------------------------------------------------------------------------------------------
        // ControlChangingMethods 
        //--------------------------------------------------------------------------------------------------

        #region SimulationSpeedSlider_ValueChanged

        public void SimulationSpeedSlider_ValueChanged(object sender, EventArgs e)
        {
            double speedChosen = ((Slider)sender).Value;

            if (speedChosen <= 0)
                TextBoxSimSpeed.Text = (32 * Math.Pow(2, -speedChosen)).ToString();
            else
                TextBoxSimSpeed.Text = (32 / Math.Pow(2d, speedChosen)).ToString();

            SimulationSpeed = 32 / Math.Pow(2, speedChosen);

            if (SimulationRunning && VisualizationEnabled)
            {
                StopSimulation(true);

                RunSimulation(SimulationSpeed);
            } // end if
        } // end of SimulationSpeedSlider_ValueChanged

        #endregion

        #region ComboBoxTimeBaseSelectedIndexChanged

        private void ComboBoxTimeBaseSelectedIndexChanged(object sender, EventArgs e)
        {
            ConstantTimerStep = TransformControlsToSimulationGap();

            if (SimulationRunning && VisualizationEnabled)
            {
                StopSimulation(true);

                RunSimulation(SimulationSpeed);
            } // end if
        } // end of ComboBoxTimeBaseSelectedIndexChanged

        #endregion

        #region CheckBoxShowVisualization_Checked

        private void CheckBoxShowVisualization_Checked(object sender, RoutedEventArgs e)
        {
            if (SimulationRunning)
            {
                StopSimulation(true);

                VisualizationEnabled = (bool)CheckBoxShowVisualization.IsChecked;

                if (VisualizationEnabled)
                {
                    _nextTimerTime = CurrentTime + ConstantTimerStep;
                    // initialize drawing engine
                    SimulationModel.SimulationDrawingEngine.InitializeModelVisualizationAtTime(CurrentTime,
                        SimulationModel);
                } //end if

                RunSimulation();
            }
            else
            {
                VisualizationEnabled = (bool)CheckBoxShowVisualization.IsChecked;
            } // end if
        } // end of CheckBoxShowVisualization_Checked

        #endregion

        //--------------------------------------------------------------------------------------------------
        // BackgroundWorker Methods 
        //--------------------------------------------------------------------------------------------------

        #region BackGroundWorkerNonVisualizationDoWork

        private void BackGroundWorkerNonVisualizationDoWork(object sender, DoWorkEventArgs e)
        {
            NonVisualizationLoop();
        } // end of BackGroundWorkerNonVisualizationDoWork

        #endregion

        #region BackGroundWorkerNonVisualizationProgressChanged

        void BackGroundWorkerNonVisualizationProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBarSimulationProgress.Value = e.ProgressPercentage;
        } // end of BackGroundWorkerNonVisualizationProgressChanged

        #endregion

        #region BackGroundWorkerNonVisualizationRunWorkerCompleted

        void BackGroundWorkerNonVisualizationRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (SimulationModel.StopSimulation(CurrentTime))
            {
                ActionsAfterFinishingSimulationRun();
            } // end if
        } // end of BackGroundWorkerNonVisualizationRunWorkerCompleted

        #endregion
    }
}