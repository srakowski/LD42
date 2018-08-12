using System;
using System.Collections.Generic;
using System.Linq;

namespace LD42
{
    static public class GameConfiguration
    {
        public const int WeeksToWin = 9;
        public const int WarehouseCapacityInResourceUnits = 16;
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

        public const int TruckMaxResources = 4;
        public const int TrainMaxResources = 8;
        public const int FreighterShipMaxResources = 12;

        public const int InitialCorporationFavor = 5;
        public const int CorporationFavorToLandContract = 10;
        public const int MaxSellOffPile = 16;
        public const int MaxCumulativeMineShutdowns = 5;
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
        private List<Route> _routes = new List<Route>();

        public Location(string name, ILocationOccupant occupant)
        {
            Name = name;
            Occupant = occupant;
        }
        public string Name { get; }
        public ILocationOccupant Occupant { get; private set; }
        public IEnumerable<Route> Routes => _routes;

        public IEnumerable<Location> Destinations => _routes.Select(r => r.Location1 == this ? r.Location2 : r.Location1).ToArray();

        internal void SetOccupant(ILocationOccupant occupant)
        {
            if (!(this.Occupant is EmptyLocation)) throw new Exception("can't place over existing occupant");
            Occupant = occupant;
        }

        internal void AddRoute(Route route) =>
            _routes.Add(route);
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

    public abstract class ResourceTransport
    {
        protected ResourceTransport(
            Route route,
            Location to, Location from, 
            IEnumerable<Resource> resourceUnits,
            int roundsToComplete)
        {
            Route = route;
            ToLocation = to;
            FromLocation = from;
            ResourceUnits = resourceUnits;
            RoundsToArrive = roundsToComplete;
            RoundsCompleted = 0;
        }
        public Route Route { get; }
        public Location ToLocation { get; }
        public Location FromLocation { get; }
        public IEnumerable<Resource> ResourceUnits { get; }
        public int RoundsToArrive { get; }
        public int RoundsCompleted { get; private set; }
        public bool IsAtDestination => RoundsCompleted >= RoundsToArrive;
        internal void ProcessRound()
        {
            RoundsCompleted++;
        }
    }

    public class Truck : ResourceTransport
    {
        public Truck(Route route, Location to, Location from, IEnumerable<Resource> resourceUnits) : base(route, to, from, resourceUnits, 1)
        {
            if (this.ResourceUnits.Count() > GameConfiguration.FreighterShipMaxResources)
                throw new Exception("this is carr ying too much");
        }
    }

    public class Train : ResourceTransport
    {
        public Train(Route route, Location to, Location from, IEnumerable<Resource> resourceUnits) : base(route, to, from, resourceUnits, 2)
        {
            if (this.ResourceUnits.Count() > GameConfiguration.TrainMaxResources)
                throw new Exception("this is carr ying too much");
        }
    }

    public class FreighterShip : ResourceTransport
    {
        public FreighterShip(Route route, Location to, Location from, IEnumerable<Resource> resourceUnits) : base(route, to, from, resourceUnits, 3)
        {
            if (this.ResourceUnits.Count() > GameConfiguration.TruckMaxResources)
                throw new Exception("this is carr ying too much");
        }
    }


    [Flags]
    public enum ShippingMethods
    {
        Ship,
        Train,
        Truck,
    }

    public class Route
    {
        private List<ResourceTransport> _transports = new List<ResourceTransport>();

        public Route(ShippingMethods routeType, Location l1, Location l2)
        {
            EligibleShippingMethods = routeType;
            Location1 = l1;
            Location2 = l2;
        }

        public ShippingMethods EligibleShippingMethods { get; }
        public Location Location1 { get; }
        public Location Location2 { get; }

        public IEnumerable<ResourceTransport> Transports => _transports;
        public IEnumerable<Truck> TruckTransports => _transports.OfType<Truck>();
        public IEnumerable<Train> TrainTransports => _transports.OfType<Train>();
        public IEnumerable<FreighterShip> FreighterShipTransports => _transports.OfType<FreighterShip>();

        internal void BindLocationRoutes()
        {
            Location1.AddRoute(this);
            Location2.AddRoute(this);
        }

        internal void AddTransport(ResourceTransport transport)
        {
            _transports.Add(transport);
        }

        internal void RemoveTransport(ResourceTransport transport)
        {
            _transports.Remove(transport);
        }
    }

    public class Warehouse : ILocationOccupant
    {
        private List<Resource> unitsOfResources = new List<Resource>();
        public int ResourceCapacityInUnits { get; } = GameConfiguration.WarehouseCapacityInResourceUnits;
        public int TotalUnits => unitsOfResources.Count;
        public IEnumerable<Resource> UnitsOfResources => unitsOfResources;
        public bool IsFull => TotalUnits >= ResourceCapacityInUnits;
        public void ReceiveResources(IEnumerable<Resource> resources, SellOffPile sellOffPile)
        {
            var resourceQueue = new Queue<Resource>(resources);
            while (resourceQueue.Any() && !IsFull)
                unitsOfResources.Add(resourceQueue.Dequeue());
            if (sellOffPile != null)
            {
                while (resourceQueue.Any())
                    sellOffPile.AddToPile(resourceQueue.Dequeue());
            }
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
        public abstract bool ShutdownTooManyTimes { get; }

        public abstract void ProcessRound();
        public abstract void ProduceFromCard(MineOutputCard card);
        public abstract IEnumerable<Resource> GenerateResourcesForCard(MineOutputCard card);
    }

    public class Mine<T> : Mine where T : Resource, new()
    {
        public int _daysShutDown = 0;

        public Mine(string name, Random random)
            : base(random)
        {
            Name = name;
            Storage = new Warehouse();
        }

        public string Name { get; }

        public override Warehouse Storage { get; }

        public override string ResourceTypeDescription => typeof(T).Name;

        public int DaysShutDown => _daysShutDown;
        public override bool ShutdownTooManyTimes => _daysShutDown >= GameConfiguration.MaxCumulativeMineShutdowns;

        public override void ProcessRound()
        {
            if (Storage.IsFull)
            {
                _daysShutDown++;
            }
            else
            {
                ProduceFromCard(Deck.DrawOne());
            }
        }

        public override void ProduceFromCard(MineOutputCard card) =>
            Storage.ReceiveResources(GenerateResourcesForCard(card), null);

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
            RoundsRequestedIn = requestedInXRounds;
            RequestedCopper = requestedCopper;
            RequestedZinc = requestedZinc;
            RequestedSilver = requestedSilver;
            RequestedIron = requestedIron;
        }
        public int RoundsRequestedIn { get; }
        public int RequestedCopper { get; }
        public int RequestedZinc { get; }
        public int RequestedSilver { get; }
        public int RequestedIron { get; }
        public string Description => 
            $"Rounds: {RoundsRequestedIn}\n" +
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
        private int _favor = GameConfiguration.InitialCorporationFavor;

        public Corporation(string name)
        {
            Name = name;
        }
        public string Name { get; }
        public int Favor => _favor;

        public bool IsUnderContract => _favor >= GameConfiguration.CorporationFavorToLandContract;

        public bool WillNoLongerDoBusinessWithYou => _favor == 0;

        public void GainFavor()
        {
            _favor++;
        }

        public void LoseFavor()
        {
            _favor--;
        }
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
        private List<Resource> _resources = new List<Resource>();
        private int _roundsSinceStart = 0;
        private bool _creditGiven = false;

        public PurchaseOrder(
            CorporationCard corporationCard,
            SaleCard saleCard,
            LocationCard shipToLocation)
        {
            Corporation = corporationCard;
            Sale = saleCard;
            ShipToLocation = shipToLocation;
        }

        public IEnumerable<Resource> Resources => _resources;
        public CorporationCard Corporation { get; }
        public SaleCard Sale { get; }
        public LocationCard ShipToLocation { get; }
        public int DueInRounds => Sale.RoundsRequestedIn - _roundsSinceStart;

        public bool HasBeenFulfilled =>
            AllResourcesFilled && _creditGiven;

        private bool AllResourcesFilled =>
            Sale.RequestedIron == _resources.OfType<Resource.Iron>().Count() &&
            Sale.RequestedSilver == _resources.OfType<Resource.Silver>().Count() &&
            Sale.RequestedCopper == _resources.OfType<Resource.Copper>().Count() &&
            Sale.RequestedZinc == _resources.OfType<Resource.Zinc>().Count();

        public void ProcessRound()
        {
            if (HasBeenFulfilled) return;
            if (AllResourcesFilled && DueInRounds >= 0)
            {
                Corporation.Corporation.GainFavor();
                _creditGiven = true;
                return;
            }
            if (DueInRounds < 0)
            {
                Corporation.Corporation.LoseFavor();
            }
            _roundsSinceStart++;
        }

        public void Progress(IEnumerable<Resource> resourceDeposit, SellOffPile sellOffPile)
        {
            _resources.AddRange(resourceDeposit);
            SellOffExcess<Resource.Iron>(sellOffPile, Sale.RequestedIron);
            SellOffExcess<Resource.Copper>(sellOffPile, Sale.RequestedCopper);
            SellOffExcess<Resource.Silver>(sellOffPile, Sale.RequestedSilver);
            SellOffExcess<Resource.Zinc>(sellOffPile, Sale.RequestedZinc);
        }

        private void SellOffExcess<T>(SellOffPile sellOffPile, int needed) where T : Resource
        {
            var extra = _resources.OfType<T>().Count() - needed;
            if (extra > 0)
            {
                var extras = _resources.OfType<T>().Take(extra);
                foreach (var ex in extras)
                {
                    _resources.Remove(ex);
                    sellOffPile.AddToPile(ex);
                }
            }
        }
    }

    public class SellOffPile
    {
        private List<Resource> resourceUnits = new List<Resource>();
        public IEnumerable<Resource> ResourceUnits => resourceUnits;

        public bool IsFull => ResourceUnits.Count() >= GameConfiguration.MaxSellOffPile;

        internal void AddToPile(Resource ex) => resourceUnits.Add(ex);
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
            Corporations = corporations;
            CorporationsDeck = corporationsDeck;
            SalesDeck = salesDeck;
            Routes = routes;
            SellOffPile = new SellOffPile();
        }

        public IEnumerable<Mine> Mines { get; }
        public IEnumerable<Location> Locations { get; }
        public LocationsDeck LocationsDeck { get; }
        public IEnumerable<Corporation> Corporations { get; }
        public CorporationsDeck CorporationsDeck { get; }
        public SalesDeck SalesDeck { get; }
        public IEnumerable<Route> Routes { get; }
        public PurchaseOrder[] PurchaseOrderSlots { get; } = new PurchaseOrder[3];
        public int ActivePurchaseOrderSlotCount => PurchaseOrderSlots.Length;
        public SellOffPile SellOffPile { get; }

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
                new Route(ShippingMethods.Ship, ll[Seattle], ll[LosAngeles]),
                new Route(ShippingMethods.Truck | ShippingMethods.Train, ll[Seattle], ll[Reno]),
                new Route(ShippingMethods.Truck | ShippingMethods.Train, ll[Seattle], ll[SaltLakeCity]),
                new Route(ShippingMethods.Truck | ShippingMethods.Train, ll[Seattle], ll[Billings]),
                new Route(ShippingMethods.Truck | ShippingMethods.Train, ll[Reno], ll[SaltLakeCity]),
                new Route(ShippingMethods.Truck | ShippingMethods.Train, ll[Reno], ll[LosAngeles]),
                new Route(ShippingMethods.Truck | ShippingMethods.Train, ll[LosAngeles], ll[SaltLakeCity]),
                new Route(ShippingMethods.Truck | ShippingMethods.Train, ll[LosAngeles], ll[Tuscon]),
                new Route(ShippingMethods.Truck | ShippingMethods.Train, ll[SaltLakeCity], ll[Billings]),
                new Route(ShippingMethods.Truck | ShippingMethods.Train, ll[SaltLakeCity], ll[Tuscon]),
                new Route(ShippingMethods.Truck | ShippingMethods.Train, ll[SaltLakeCity], ll[Denver]),
                new Route(ShippingMethods.Truck | ShippingMethods.Train, ll[Tuscon], ll[Denver]),
                new Route(ShippingMethods.Truck | ShippingMethods.Train, ll[Billings], ll[Denver]),
                new Route(ShippingMethods.Train, ll[Tuscon], ll[Houston]),
                new Route(ShippingMethods.Train, ll[Billings], ll[Duluth]),
                new Route(ShippingMethods.Truck | ShippingMethods.Train, ll[Denver], ll[KansasCity]),
                new Route(ShippingMethods.Truck | ShippingMethods.Train, ll[Duluth], ll[KansasCity]),
                new Route(ShippingMethods.Ship, ll[Duluth], ll[Detroit]),
                new Route(ShippingMethods.Truck | ShippingMethods.Train, ll[Duluth], ll[Indianapolis]),
                new Route(ShippingMethods.Truck | ShippingMethods.Train, ll[KansasCity], ll[Indianapolis]),
                new Route(ShippingMethods.Truck | ShippingMethods.Train, ll[KansasCity], ll[NewOrleans]),
                new Route(ShippingMethods.Truck | ShippingMethods.Train, ll[KansasCity], ll[Houston]),
                new Route(ShippingMethods.Truck | ShippingMethods.Train, ll[Houston], ll[NewOrleans]),
                new Route(ShippingMethods.Ship, ll[NewOrleans], ll[Jacksonville]),
                new Route(ShippingMethods.Truck | ShippingMethods.Train, ll[NewOrleans], ll[Knoxville]),
                new Route(ShippingMethods.Truck | ShippingMethods.Train, ll[Jacksonville], ll[Knoxville]),
                new Route(ShippingMethods.Truck | ShippingMethods.Train, ll[Indianapolis], ll[Knoxville]),
                new Route(ShippingMethods.Train, ll[Boston], ll[Indianapolis]),
                new Route(ShippingMethods.Ship, ll[Detroit], ll[Boston]),
                new Route(ShippingMethods.Ship, ll[Jacksonville], ll[Boston]),
                new Route(ShippingMethods.Truck, ll[Detroit], ll[Indianapolis]),
            };

            foreach (var route in routes) route.BindLocationRoutes();

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
