using System;
using System.Collections.Generic;
using System.Linq;

namespace LD42
{
    static class GameConfiguration
    {
        public const int WeeksToWin = 9;
        public const int ThirdPartySellOfCapacityInResourceUnits = 13;
        public const int WarehouseCapacityInResourceUnits = 13;
        public const int CardsDrawnAtLowProductionLevel = 1;
        public const int CardsDrawnAtMediumProductionLevel = 2;
        public const int CardsDrawnAtHighProductionLevel = 3;
        public const int MaxResourceMiningCardValue = 4;
        public const int ResourceMiningCardsDrawnToStartTheGame = 3;
    }

    abstract class Resource
    {
        public class Copper : Resource { }
        public class Lithium : Resource { }
        public class Silver : Resource { }
        public class Gold : Resource { }
        public class Iron : Resource { }
    }

    enum ProductionLevels
    {
        Low,
        Medium,
        High,
    }

    interface IGameBoardMapElement { }

    interface ILocationOccupant { }

    class EmptyLocation : ILocationOccupant { }

    class Location : IGameBoardMapElement
    {
        public Location(string name, ILocationOccupant occupant)
        {
            Name = name;
            Occupant = occupant;
        }
        public string Name { get; }
        public ILocationOccupant Occupant { get; private set; }
    }

    static class Locations
    {
        public static Location Hibbing(Mine<Resource.Iron> ironMine) => new Location($"{nameof(Hibbing)}", ironMine);
        public static Location KansasCity() => new Location($"{nameof(KansasCity)}", new EmptyLocation());
        public static Location Chicago() => new Location($"{nameof(Chicago)}", new EmptyLocation());
    }

    class ResourceTransport
    {
        public IEnumerable<Resource> ResourceUnitsInTransit { get; private set; }
    }

    class RouteSegment : IGameBoardMapElement
    {
        private List<ResourceTransport> transports = new List<ResourceTransport>();

        public RouteSegment(IGameBoardMapElement nextToLocation1, IGameBoardMapElement nextToLocation2)
        {
            NextToLocation1 = nextToLocation1;
            NextToLocation2 = nextToLocation2;
        }

        public IGameBoardMapElement NextToLocation1 { get; }
        public IGameBoardMapElement NextToLocation2 { get; }
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

        public int ResourceCapacityInUnits { get; } = GameConfiguration.WarehouseCapacityInResourceUnits;
        public int TotalUnits => unitsOfResources.Count;
        public IEnumerable<Resource> UnitsOfResources => unitsOfResources;

        public void ReceiveResources(IEnumerable<Resource> resources)
        {
            var resourceQueue = new Queue<Resource>(resources);
            while (resourceQueue.Any() && TotalUnits < ResourceCapacityInUnits)
                unitsOfResources.Add(resourceQueue.Dequeue());
            thirdParties.ReceiveResources(resourceQueue);
        }
    }

    class ThirdParties
    {
        private List<Resource> unitsOfResources = new List<Resource>();
        public const int ResourceCapacityInUnits = GameConfiguration.ThirdPartySellOfCapacityInResourceUnits;
        public IEnumerable<Resource> Resources => unitsOfResources;
        internal void ReceiveResources(IEnumerable<Resource> excessResources) =>
            unitsOfResources.AddRange(excessResources);
    }

    class MineOutputCard<T> where T : Resource, new()
    {
        public MineOutputCard(int resourceUnitsMined) => ResourceUnitsMined = resourceUnitsMined;
        public int ResourceUnitsMined { get; }
    }

    class MineOutputDeck<T> where T : Resource, new()
    {
        private Queue<MineOutputCard<T>> cards;
        public MineOutputDeck(Random random)
        {
            cards = new Queue<MineOutputCard<T>>(Enumerable.Range(0,
                    GameConfiguration.CardsDrawnAtHighProductionLevel *
                    GameConfiguration.WeeksToWin
                )
                .Select((i) => new MineOutputCard<T>(
                    (i % GameConfiguration.MaxResourceMiningCardValue) + GameConfiguration.ResourceMiningCardsDrawnToStartTheGame)
                )
                .OrderBy(i => random.Next())
            );
        }
        public MineOutputCard<T> DrawOne() => cards.Dequeue();
    }

    abstract class Mine : ILocationOccupant { }

    class Mine<T> : Mine where T : Resource, new()
    {
        public Mine(string name, Random random, ThirdParties thirdParties)
        {
            Name = name;
            ProductionLevel = ProductionLevels.High;
            Deck = new MineOutputDeck<T>(random);
            Storage = new Warehouse(thirdParties);
        }

        public string Name { get; }
        public ProductionLevels ProductionLevel { get; private set; }
        public MineOutputDeck<T> Deck { get; }
        public Warehouse Storage { get; }

        public void Produce()
        {
            var unitsProduced = ProductionLevel == ProductionLevels.High
                ? GameConfiguration.CardsDrawnAtHighProductionLevel
                : ProductionLevel == ProductionLevels.Medium
                ? GameConfiguration.CardsDrawnAtMediumProductionLevel
                : ProductionLevel == ProductionLevels.Low
                ? GameConfiguration.CardsDrawnAtLowProductionLevel
                : throw new Exception("you changed the production levels");

            Storage.ReceiveResources(
                Enumerable.Range(0,
                    Enumerable
                        .Range(0, unitsProduced)
                        .Select(_ => Deck.DrawOne())
                        .Aggregate(0, (total, card) => total + card.ResourceUnitsMined)
                )
                .Select(_ => Activator.CreateInstance<T>())
            );
        }
    }

    static class Mines
    {
        public static Mine<Resource.Iron> NorthStarMine(Random random, ThirdParties thirdParties) => 
            new Mine<Resource.Iron>("North Star Mine", random, thirdParties);
    }

    class SaleCard
    {
        public string DeliverToLocation { get; }
        public int WeeksToFulfill { get; }
        public int UnitsOfCopper { get; }
        public int UnitsOfLithium { get; }
        public int UnitsOfSilver { get; }
        public int UnitsOfGold { get; }
        public int UnitsOfIron { get; }
    }

    class PurchaseOrder
    {
        private SaleCard sale;

        public PurchaseOrder(SaleCard sale)
        {
            this.sale = sale;
        }
    }

    class GameBoardMap
    {
        private GameBoardMap(IEnumerable<IGameBoardMapElement> mapElements)
        {
            MapElements = mapElements;
        }

        public IEnumerable<IGameBoardMapElement> MapElements { get; }

        public static GameBoardMap Create(Random random)
        {
            throw new NotImplementedException();
        }
    }
}
