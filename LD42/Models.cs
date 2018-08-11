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
        public class Zinc : Resource { }
        public class Silver : Resource { }
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

    class ResourceTransport
    {
        public IEnumerable<Resource> ResourceUnitsInTransit { get; private set; }
    }

    enum RouteType
    {
        Ship,
        Train,
        Truck,
        TruckOrTrain,
    }

    class Route
    {

        private List<ResourceTransport> _truckTransports = new List<ResourceTransport>();
        private List<ResourceTransport> _trainTransports = new List<ResourceTransport>();

        public Route(RouteType routeType, Location l1, Location l2)
        {
            RouteType = routeType;
            Location1 = l1;
            Location2 = l2;
        }

        private RouteType RouteType { get; }
        public Location Location1 { get; }
        public Location Location2 { get; }
        
        public IEnumerable<ResourceTransport> TruckTransports => _truckTransports;
        public IEnumerable<ResourceTransport> TrainTransports => _trainTransports;
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

    class SaleCard
    {
        public string DeliverToLocation { get; }
        public int WeeksToFulfill { get; }
        public int UnitsOfCopper { get; }
        public int UnitsOfZinc { get; }
        public int UnitsOfSilver { get; }
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
        const string Reno = "Reno";
        const string Knoxville = "Knoxville";
        const string Tuscon = "Tuscon";
        const string Duluth = "Duluth";
        const string Denver = "Denver";
        const string LosAngeles = "Los Angeles";
        const string Detroit = "Detroit";
        const string Houston = "Houston";
        const string Seattle = "Seattle";
        const string Jacksonville = "Jacksonville";
        const string Boston = "Boston";
        const string Indianapolis = "Indianapolis";
        const string NewOrleans = "New Orleans";
        const string SaltLakeCity = "Salt Lake City";
        const string Billings = "Billings";
        const string KansasCity = "Kansas City";

        private GameBoardMap(IEnumerable<IGameBoardMapElement> mapElements)
        {
            MapElements = mapElements;
        }

        public IEnumerable<IGameBoardMapElement> MapElements { get; }

        public static GameBoardMap Create(Random random)
        {
            var tp = new ThirdParties();

            var duluthMine = new Mine<Resource.Iron>("North Star Iron Mine", random, tp);
            var knoxvillMine = new Mine<Resource.Zinc>("Smokey Appalachian Zinc Mine", random, tp);
            var tusconMine = new Mine<Resource.Copper>("Great Canyon State Copper Mine", random, tp);
            var renoMine = new Mine<Resource.Silver>("Five Card Silver Mine", random, tp);

            var mines = new Mine[]
            {
                duluthMine,
                knoxvillMine,
                tusconMine,
                renoMine,
            };

            var locations = new Location[]
            {
                new Location(Reno, renoMine),
                new Location(Knoxville, knoxvillMine),
                new Location(Tuscon, tusconMine),
                new Location(Duluth, duluthMine),
                new Location(Denver, new EmptyLocation()),
                new Location(LosAngeles, new EmptyLocation()),
                new Location(Detroit, new EmptyLocation()),
                new Location(Houston, new EmptyLocation()),
                new Location(Seattle, new EmptyLocation()),
                new Location(Jacksonville, new EmptyLocation()),
                new Location(Boston, new EmptyLocation()),
                new Location(Indianapolis, new EmptyLocation()),
                new Location(NewOrleans, new EmptyLocation()),
                new Location(SaltLakeCity, new EmptyLocation()),
                new Location(Billings, new EmptyLocation()),
                new Location(KansasCity, new EmptyLocation()),
            };

            var ll = locations.ToDictionary((l) => l.Name);

            var route = new Route[]
            {
                new Route(RouteType.Ship, ll[Seattle], ll[LosAngeles]),
                new Route(RouteType.TruckOrTrain, ll[Seattle], ll[Reno]),
                new Route(RouteType.TruckOrTrain, ll[Seattle], ll[SaltLakeCity]),
                new Route(RouteType.TruckOrTrain, ll[Seattle], ll[Billings]),
                new Route(RouteType.TruckOrTrain, ll[Reno], ll[SaltLakeCity]),
                new Route(RouteType.TruckOrTrain, ll[Reno], ll[LosAngeles]),
                new Route(RouteType.TruckOrTrain, ll[LosAngeles], ll[SaltLakeCity]),
                new Route(RouteType.TruckOrTrain, ll[LosAngeles], ll[Tuscon]),
                new Route(RouteType.TruckOrTrain, ll[SaltLakeCity], ll[Billings]),
                new Route(RouteType.TruckOrTrain, ll[SaltLakeCity], ll[Tuscon]),
                new Route(RouteType.TruckOrTrain, ll[SaltLakeCity], ll[Denver]),
                new Route(RouteType.TruckOrTrain, ll[Tuscon], ll[Denver]),
                new Route(RouteType.TruckOrTrain, ll[Billings], ll[Denver]),
                new Route(RouteType.Train, ll[Tuscon], ll[Houston]),
                new Route(RouteType.Train, ll[Billings], ll[Duluth]),
                new Route(RouteType.TruckOrTrain, ll[Denver], ll[KansasCity]),
                new Route(RouteType.TruckOrTrain, ll[Duluth], ll[KansasCity]),
                new Route(RouteType.Ship, ll[Duluth], ll[Detroit]),
                new Route(RouteType.TruckOrTrain, ll[Duluth], ll[Indianapolis]),
                new Route(RouteType.TruckOrTrain, ll[KansasCity], ll[Indianapolis]),
                new Route(RouteType.TruckOrTrain, ll[KansasCity], ll[NewOrleans]),
                new Route(RouteType.TruckOrTrain, ll[KansasCity], ll[Houston]),
                new Route(RouteType.TruckOrTrain, ll[Houston], ll[NewOrleans]),
                new Route(RouteType.Ship, ll[NewOrleans], ll[Jacksonville]),
                new Route(RouteType.TruckOrTrain, ll[NewOrleans], ll[Knoxville]),
                new Route(RouteType.TruckOrTrain, ll[Jacksonville], ll[Knoxville]),
                new Route(RouteType.TruckOrTrain, ll[Indianapolis], ll[Knoxville]),
                new Route(RouteType.Train, ll[Boston], ll[Indianapolis]),
                new Route(RouteType.Ship, ll[Detroit], ll[Boston]),
                new Route(RouteType.Ship, ll[Jacksonville], ll[Boston]),
                new Route(RouteType.Truck, ll[Detroit], ll[Indianapolis]),
            };



            return new GameBoardMap();
        }
    }
}
