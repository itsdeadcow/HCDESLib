using System;
using RailCargo.HCCM.Input;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;

namespace RailCargo.HCCM.ControlUnits
{
    public class CuRoutingPath : ControlUnit
    {
        private readonly object _inputPaths;

        public CuRoutingPath(string name, ControlUnit parentControlUnit, SimulationModel parentSimulationModel, InputPath inputPath) : base(
            name, parentControlUnit, parentSimulationModel)
        {
            _inputPaths = inputPath;
        }

        protected override void CustomInitialize(DateTime startTime, ISimulationEngine simEngine)
        {
            Console.WriteLine("Hello we are running the simulation");
        }

        protected override bool PerformCustomRules(DateTime time, ISimulationEngine simEngine)
        {
            throw new NotImplementedException();
        }

        public override Event EntityEnterControlUnit(DateTime time, ISimulationEngine simEngine, Entity entity,
            IDelegate originDelegate)
        {
            throw new NotImplementedException();
        }

        public override void EntityLeaveControlUnit(DateTime time, ISimulationEngine simEngine, Entity entity,
            IDelegate originDelegate)
        {
            throw new NotImplementedException();
        }
    }
}