using System;
using System.Collections.Generic;
using System.Linq;

namespace LD42
{
    static public class GameConfiguration
    {
        public const int WeeksToWin = 9;
        public const int WarehouseCapacityInResourceUnits = 13;
        public const int WarehouseOverflowCapacityInResourceUnits = 3;
        public const int MaxResourceMiningCardValue = 4;
        public const int LocationCardsPerLocation = 10;
        public const int CorporationCardsPerCoporation = 10;

        public const int ResourceMiningCardsDrawnToStartTheGame = 3; // must be (InitialToMines + InitialToWarehouse) / mine locations
        public const int InitialResourcesToMines = 9;
        public const int InitialResourcesToWarehouse = 3;

        public const int PurchaseOrderOptionCount = 5;

        public const int MinDeliveryTime = 1;
        public const int ModDeliveryTime = 4;

        public const int IronHeavySale = 0;
        public const int SilverHeavySale = 1;
        public const int CopperHeavySale = 2;
        public const int ZincHeavySale = 3;

        public const int ActionPointsPerRound = 5;
    }

    public interface ICard
    {
        string Description { get; }
    }

    abstract public class Deck<T> where T : ICard
    {
        protected Random random;
        protected Queue<T> cards;

        public Deck(Random random)
        {
            this.random = random;
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

    abstract public class Resource
    {
        public class Copper : Resource { }
        public class Zinc : Resource { }
        public class Silver : Resource { }
        public class Iron : Resource { }
    }

    public interface ILocationOccupant { }

    public class EmptyLocation : ILocationOccupant { }

    public class Location
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

    public struct LocationCard : ICard
    {
        public LocationCard(Location location) => Location = location;
        public Location Location { get; }
        public string Description => this.Location.Name;
    }

    public class LocationsDeck : Deck<LocationCard>
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

    public class ResourceTransport
    {
        public IEnumerable<Resource> ResourceUnitsInTransit { get; private set; }
    }

    public enum RouteType
    {
        Ship,
        Train,
        Truck,
        TruckOrTrain,
    }

    public class Route
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

    public class Warehouse : ILocationOccupant
    {
        private List<Resource> unitsOfResources = new List<Resource>();
        private List<Resource> overflowUnitsOfResources = new List<Resource>();
        public int ResourceCapacityInUnits { get; } = GameConfiguration.WarehouseCapacityInResourceUnits;
        public int OverflowCapacityInUnits { get; } = GameConfiguration.WarehouseOverflowCapacityInResourceUnits;
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

    public struct MineOutputCard : ICard
    {
        public MineOutputCard(Mine mine, int resourceUnitsMined)
        {
            Mine = mine;
            ResourceUnitsMined = resourceUnitsMined;
        }
        public Mine Mine { get; }
        public int ResourceUnitsMined { get; }
        public string Description => $"{Mine.ResourceTypeDescription} x {ResourceUnitsMined}";
    }

    public class MineOutputsDeck : Deck<MineOutputCard>
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

    abstract public class Mine : ILocationOccupant
    {
        protected Mine(Random random) => Deck = new MineOutputsDeck(this, random);
        public MineOutputsDeck Deck { get; }
        public abstract Warehouse Storage { get; }
        public abstract string ResourceTypeDescription { get; }

        public abstract void Produce();
        public abstract void ProduceFromCard(MineOutputCard card);
        public abstract IEnumerable<Resource> GenerateResourcesForCard(MineOutputCard card);
    }

    public class Mine<T> : Mine where T : Resource, new()
    {
        public Mine(string name, Random random)
            : base(random)
        {
            Name = name;
            Storage = new Warehouse();
        }

        public string Name { get; }

        public override Warehouse Storage { get; }

        public override string ResourceTypeDescription => typeof(T).Name;

        public override void Produce() =>
            ProduceFromCard(Deck.DrawOne());

        public override void ProduceFromCard(MineOutputCard card) =>
            Storage.ReceiveResources(GenerateResourcesForCard(card));

        public override IEnumerable<Resource> GenerateResourcesForCard(MineOutputCard card) =>
            Enumerable.Range(0, card.ResourceUnitsMined)
                .Select(_ => Activator.CreateInstance<T>());
    }

    public class SaleCard : ICard
    {
        public SaleCard(int requestedInXRounds,
            int requestedCopper,
            int requestedZinc,
            int requestedSilver,
            int requestedIron)
        {
            RequestedInXRounds = requestedInXRounds;
            RequestedCopper = requestedCopper;
            RequestedZinc = requestedZinc;
            RequestedSilver = requestedSilver;
            RequestedIron = requestedIron;
        }
        public int RequestedInXRounds { get; }
        public int RequestedCopper { get; }
        public int RequestedZinc { get; }
        public int RequestedSilver { get; }
        public int RequestedIron { get; }
        public string Description => 
            $"Rounds: {RequestedInXRounds}\n" +
            $"Copper: {RequestedCopper}\n" +
            $"Zinc: {RequestedZinc}\n" +
            $"Silver: {RequestedSilver}\n" +
            $"Iron: {RequestedIron}\n";
    }

    public class SalesDeck : Deck<SaleCard>
    {
        public SalesDeck(Random random) : base(random)
        {
            ResetDeck();
        }

        public override void ResetDeck()
        {
            cards = new Queue<SaleCard>(Enumerable
                .Range(0, 100)
                .Select(i => new SaleCard(
                    (i % GameConfiguration.ModDeliveryTime) + GameConfiguration.MinDeliveryTime,
                    requestedCopper: CalcStartResource(i, GameConfiguration.CopperHeavySale),
                    requestedZinc: CalcStartResource(i, GameConfiguration.ZincHeavySale),
                    requestedSilver: CalcStartResource(i, GameConfiguration.SilverHeavySale),
                    requestedIron: CalcStartResource(i, GameConfiguration.IronHeavySale)
                    ))
                .OrderBy(i => random.Next())
                .ToArray()
            );
        }

        private static int CalcStartResource(int i, int hs)
        {
            return 1 + (i % 4 == hs ? 2 : 0) + (i / 13);
        }
    }

    public class Corporation
    {
        public Corporation(string name)
        {
            Name = name;
        }
        public string Name { get; }
    }

    public struct CorporationCard : ICard
    {
        public CorporationCard(Corporation corporation)
        {
            Corporation = corporation;
        }
        public Corporation Corporation { get; }
        public string Description => Corporation.Name;
    }

    public class CorporationsDeck : Deck<CorporationCard>
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
                        .Select(_ => new CorporationCard(c))
                    )
                    .OrderBy(i => random.Next())
                    .ToArray()
            );
        }
    }


    public class PurchaseOrder
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

    public class GameBoard
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

        private GameBoard(
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
        public PurchaseOrder[] ActivePurchaseOrders { get; } = new PurchaseOrder[3];
        public int ActivePurchaseOrderSlotCount => ActivePurchaseOrders.Length;

        public static GameBoard Create(Random random)
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

            return new GameBoard(mines,
                locations, 
                locationDeck,
                corporations,
                corporationsDeck,
                salesDeck,
                routes);
        }
    }
}
