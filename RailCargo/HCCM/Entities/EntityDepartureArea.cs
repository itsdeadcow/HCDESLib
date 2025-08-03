using System.Collections.Generic;
using SimulationCore.HCCMElements;

namespace RailCargo.HCCM.Entities
{
    public class EntityDepartureArea : Entity, IDynamicHoldingEntity
    {
        private static int _sIdentifier;
        private List<Entity> _holdedEntities = new List<Entity>();

        public EntityDepartureArea() : base(++_sIdentifier)
        {
        }

        public override string ToString()
        {
            return "EntityDepartureArea" + Identifier;
        }

        public override Entity Clone()
        {
            return new EntityDepartureArea();
        }

        public List<Entity> HoldedEntities
        {
            get => _holdedEntities;
            set => _holdedEntities = value;
        }
    }
}