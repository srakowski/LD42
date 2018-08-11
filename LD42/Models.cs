using System;
using System.Collections.Generic;
using System.Linq;

namespace LD42
{
    static class GameConfiguration
    {
        public const int ThirdPartySellOfCapacityInResourceUnits = 13;
        public const int UnitsProducedAtLowProductionLevel = 1;
        public const int UnitsProducedAtMediumProductionLevel = 2;
        public const int UnitsProducedAtHighProductionLevel = 3;
    }

    abstract class Resource
    {
        public class Iron : Resource { }
        public class Coal : Resource { }
        public class Copper : Resource { }
        public class Silver : Resource { }
        public class Uranium : Resource { }
        public class Gold : Resource { }
    }

    enum ProductionLevels
    {
        Low,
        Medium,
        High,
    }

    enum MineLevels
    {
        Level1,
        Level2,
        Level3
    }

    interface IMapElement { }

    interface ILocationOccupant { }

    class EmptyLocation : ILocationOccupant { }

    abstract class Location : IMapElement
    {
        public Location(string name, ILocationOccupant occupant)
        {
            Name = name;
            Occupant = occupant;
        }
        public string Name { get; }
        public ILocationOccupant Occupant { get; private set; }
    }

    class ResourceTransport
    {
        public IEnumerable<Resource> ResourceUnitsInTransit { get; private set; }
    }

    class RouteSegment : IMapElement
    {
        private List<ResourceTransport> transports = new List<ResourceTransport>();

        public RouteSegment(IMapElement nextToLocation1, IMapElement nextToLocation2)
        {
            NextToLocation1 = nextToLocation1;
            NextToLocation2 = nextToLocation2;
        }

        public IMapElement NextToLocation1 { get; }
        public IMapElement NextToLocation2 { get; }
        public IEnumerable<ResourceTransport> Transports => transports;
    }

    class Route
    {
        public Location Location1 { get; }
        public Location Location2 { get; }
    }

    class Warehouse : ILocationOccupant
    {
        private List<Resource> unitsOfResources = new List<Resource>();
        private ThirdParties thirdParties;

        public Warehouse(ThirdParties thirdParties)
        {
            this.thirdParties = thirdParties;
        }

        public int UnitCapacity { get; }
        public int TotalUnits => unitsOfResources.Count;
        public IEnumerable<Resource> UnitsOfResources => unitsOfResources;

        public void ReceiveResources(IEnumerable<Resource> resources)
        {
            var resourceQueue = new Queue<Resource>(resources);
            while (resourceQueue.Any() && TotalUnits < UnitCapacity)
                unitsOfResources.Add(resourceQueue.Dequeue());
            thirdParties.ReceiveResources(resourceQueue);
        }
    }

    class ThirdParties
    {
        private List<Resource> unitsOfResources = new List<Resource>();
        public const int ResourceCapacity = GameConfiguration.ThirdPartySellOfCapacityInResourceUnits;
        public IEnumerable<Resource> Resources => unitsOfResources;
        internal void ReceiveResources(IEnumerable<Resource> excessResources) =>
            unitsOfResources.AddRange(excessResources);
    }

    abstract class Mine : ILocationOccupant { }

    class Mine<T> : Mine where T : Resource, new()
    {
        public Mine(ProductionLevels productionLevel, ThirdParties thirdParties)
        {
            ProductionLevel = productionLevel;
            Storage = new Warehouse(thirdParties);
        }

        public MineLevels MineLevel { get; private set; }
        public ProductionLevels ProductionLevel { get; private set; }
        public Warehouse Storage { get; }

        public void Produce()
        {
            var unitsProduced = ProductionLevel == ProductionLevels.High
                ? GameConfiguration.UnitsProducedAtHighProductionLevel
                : ProductionLevel == ProductionLevels.Medium
                ? GameConfiguration.UnitsProducedAtMediumProductionLevel
                : ProductionLevel == ProductionLevels.Low
                ? GameConfiguration.UnitsProducedAtLowProductionLevel
                : throw new Exception("you changed the production levels");

            Storage.ReceiveResources(
                Enumerable
                    .Range(0, unitsProduced)
                    .Select(_ => Activator.CreateInstance<T>())
            );
        }
    }

    class GameMap
    {
        public GameMap(IEnumerable<IMapElement> mapElements)
        {
            MapElements = mapElements;
        }
        public IEnumerable<IMapElement> MapElements { get; }
    }
}
