using System;
using System.Collections.Generic;
using System.Linq;

namespace LD42
{
    static class GameConfiguration
    {
        public const int WeeksToWin = 9;
        public const int WarehouseCapacityInResourceUnits = 13;
        public const int MaxResourceMiningCardValue = 4;
        public const int LocationCardsPerLocation = 10;
        public const int CorporationCardsPerCoporation = 10;

        public const int ResourceMiningCardsDrawnToStartTheGame = 3; // must be (InitialToMines + InitialToWarehouse) / mine locations
        public const int InitialResourcesToMines = 9;
        public const int InitialResourcesToWarehouse = 3;
    }

    interface ICard { }

    abstract class Deck<T> where T : ICard
    {
        protected Random random;
        protected Queue<T> cards;

        public Deck(Random random)
        {
            this.random = random;
            ResetDeck();
        }

        public T DrawOne()
        {
            var card = cards.Dequeue();
            if (!cards.Any()) ResetDeck();
            return card;
        }

        public IEnumerable<T> Draw(int count) =>
            Enumerable
                .Range(0, count)
                .Select(_ => DrawOne())
                .ToArray();

        public abstract void ResetDeck();
    }

    abstract class Resource
    {
        public class Copper : Resource { }
        public class Zinc : Resource { }
        public class Silver : Resource { }
        public class Iron : Resource { }
    }

    interface ILocationOccupant { }

    class EmptyLocation : ILocationOccupant { }

    class Location
    {
        public Location(string name, ILocationOccupant occupant)
        {
            Name = name;
            Occupant = occupant;
        }
        public string Name { get; }
        public ILocationOccupant Occupant { get; private set; }
        internal void SetOccupant(ILocationOccupant occupant)
        {
            if (!(this.Occupant is EmptyLocation)) throw new Exception("can't place over existing occupant");
            Occupant = occupant;
        }
    }

    struct LocationCard : ICard
    {
        public LocationCard(Location location) => Location = location;
        public Location Location { get; }
    }

    class LocationsDeck : Deck<LocationCard>
    {
        private IEnumerable<Location> locations;

        public LocationsDeck(IEnumerable<Location> locations, Random random) : base(random)
        {
            this.locations = locations;
            ResetDeck();
        }

        public override void ResetDeck()
        {
            cards = new Queue<LocationCard>(
                locations
                    .SelectMany(l => Enumerable
                        .Range(0, GameConfiguration.LocationCardsPerLocation)
                        .Select(_ => new LocationCard(l))
                    )
                    .OrderBy(i => random.Next())
                    .ToArray()
            );
        }
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
        private List<Resource> overflowUnitsOfResources = new List<Resource>();
        public int ResourceCapacityInUnits { get; } = GameConfiguration.WarehouseCapacityInResourceUnits;
        public int TotalUnits => unitsOfResources.Count;
        public IEnumerable<Resource> UnitsOfResources => unitsOfResources;

        public void ReceiveResources(IEnumerable<Resource> resources)
        {
            var resourceQueue = new Queue<Resource>(resources);
            while (resourceQueue.Any() && TotalUnits < ResourceCapacityInUnits)
                unitsOfResources.Add(resourceQueue.Dequeue());
            overflowUnitsOfResources.AddRange(resourceQueue);
        }
    }

    struct MineOutputCard : ICard
    {
        public MineOutputCard(Mine mine, int resourceUnitsMined)
        {
            Mine = mine;
            ResourceUnitsMined = resourceUnitsMined;
        }
        public Mine Mine { get; }
        public int ResourceUnitsMined { get; }
    }

    class MineOutputsDeck : Deck<MineOutputCard>
    {
        private Mine mine;

        public MineOutputsDeck(Mine mine, Random random) : base(random)
        {
            this.mine = mine;
            ResetDeck();
        }

        public override void ResetDeck()
        {
            cards = new Queue<MineOutputCard>(Enumerable
                .Range(0, 100)
                .Select((i) => new MineOutputCard(mine, (i % GameConfiguration.MaxResourceMiningCardValue) + 1))
                .OrderBy(i => random.Next())
                .ToArray()
            );
        }
    }

    abstract class Mine : ILocationOccupant
    {
        protected Mine(Random random) => Deck = new MineOutputsDeck(this, random);
        public MineOutputsDeck Deck { get; }
        public abstract void Produce();
        public abstract void ProduceFromCard(MineOutputCard card);
        public abstract IEnumerable<Resource> GenerateResourcesForCard(MineOutputCard card);
    }

    class Mine<T> : Mine where T : Resource, new()
    {
        public Mine(string name, Random random)
            : base(random)
        {
            Name = name;
            Storage = new Warehouse();
        }

        public string Name { get; }

        public Warehouse Storage { get; }

        public override void Produce() =>
            ProduceFromCard(Deck.DrawOne());

        public override void ProduceFromCard(MineOutputCard card) =>
            Storage.ReceiveResources(GenerateResourcesForCard(card));

        public override IEnumerable<Resource> GenerateResourcesForCard(MineOutputCard card) =>
            Enumerable.Range(0, card.ResourceUnitsMined)
                .Select(_ => Activator.CreateInstance<T>());
    }

    class SaleCard : ICard
    {
        public int WeeksToFulfill { get; }
        public int UnitsOfCopper { get; }
        public int UnitsOfZinc { get; }
        public int UnitsOfSilver { get; }
        public int UnitsOfIron { get; }
    }

    class SalesDeck : Deck<SaleCard>
    {
        public SalesDeck(Random random) : base(random) { }

        public override void ResetDeck()
        {
            cards = new Queue<SaleCard>(Enumerable
                .Range(0, 100)
                .Select(_ => new SaleCard())
                .OrderBy(i => random.Next())
                .ToArray()
            );
        }
    }

    class Corporation
    {
        public Corporation(string name)
        {
            Name = name;
        }
        public string Name { get; }
    }

    struct CorporationCard : ICard
    {
        public CorporationCard(Corporation corporation)
        {
            Corporation = corporation;
        }
        public Corporation Corporation { get; }
    }

    class CorporationsDeck : Deck<CorporationCard>
    {
        private IEnumerable<Corporation> corporations;

        public CorporationsDeck(IEnumerable<Corporation> corporations, Random random) : base(random)
        {
            this.corporations = corporations;
            ResetDeck();
        }

        public override void ResetDeck()
        {
            cards = new Queue<CorporationCard>(
                corporations
                    .SelectMany(c => Enumerable
                        .Range(0, GameConfiguration.CorporationCardsPerCoporation)
                        .Select(_ => new LocationCard(c))
                    )
                    .OrderBy(i => random.Next())
                    .ToArray()
            );
        }
    }


    class PurchaseOrder
    {
        public PurchaseOrder(
            CorporationCard corporationCard,
            SaleCard saleCard,
            LocationCard shipToLocation)
        {
            Corporation = corporationCard;
            Sale = saleCard;
            ShipToLocation = shipToLocation;
        }
        public CorporationCard Corporation { get; }
        public SaleCard Sale { get; }
        public LocationCard ShipToLocation { get; }
    }

    class GameBoardMap
    {
        public const string Reno = "Reno";
        public const string Knoxville = "Knoxville";
        public const string Tuscon = "Tuscon";
        public const string Duluth = "Duluth";
        public const string Denver = "Denver";
        public const string LosAngeles = "Los Angeles";
        public const string Detroit = "Detroit";
        public const string Houston = "Houston";
        public const string Seattle = "Seattle";
        public const string Jacksonville = "Jacksonville";
        public const string Boston = "Boston";
        public const string Indianapolis = "Indianapolis";
        public const string NewOrleans = "New Orleans";
        public const string SaltLakeCity = "Salt Lake City";
        public const string Billings = "Billings";
        public const string KansasCity = "Kansas City";

        private GameBoardMap(
            IEnumerable<Mine> mines,
            IEnumerable<Location> locations,
            LocationsDeck locationDeck,
            IEnumerable<Corporation> corporations,
            CorporationsDeck corporationsDeck,
            SalesDeck salesDeck,
            IEnumerable<Route> routes)
        {
            Mines = mines;
            Locations = locations;
            LocationsDeck = locationDeck;
            CorporationsDeck = corporationsDeck;
            SalesDeck = salesDeck;
            Routes = routes;
        }

        public IEnumerable<Mine> Mines { get; }
        public IEnumerable<Location> Locations { get; }
        public LocationsDeck LocationsDeck { get; }
        public CorporationsDeck CorporationsDeck { get; }
        public SalesDeck SalesDeck { get; }
        public IEnumerable<Route> Routes { get; }

        public static GameBoardMap Create(Random random)
        {
            var duluthMine = new Mine<Resource.Iron>("North Star Iron Mine", random);
            var knoxvillMine = new Mine<Resource.Zinc>("Smokey Appalachian Zinc Mine", random);
            var tusconMine = new Mine<Resource.Copper>("Great Canyon State Copper Mine", random);
            var renoMine = new Mine<Resource.Silver>("Five Card Silver Mine", random);

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

            var routes = new Route[]
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

            var locationDeck = new LocationsDeck(locations, random);

            var corporations = new[]
            {
                new Corporation("LuDare Enterprises"),
                new Corporation("SparkTech Industries"),
                new Corporation("Adico"),
            };

            var corporationsDeck = new CorporationsDeck(corporations, random);

            var salesDeck = new SalesDeck(random);

            return new GameBoardMap(mines, 
                locations, 
                locationDeck,
                corporations,
                corporationsDeck,
                salesDeck,
                routes);
        }
    }
}
