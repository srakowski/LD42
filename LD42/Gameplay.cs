using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LD42
{
    public class MiningLogisticsGameplay
    {
        public GameBoard GameBoard { get; private set; }

        public bool GameIsWon =>
            GameBoard.Corporations.All(c => c.IsUnderContract);

        public bool GameIsLost =>
            GameBoard.SellOffPile.IsFull ||
            GameBoard.Corporations.Any(c => c.WillNoLongerDoBusinessWithYou) ||
            GameBoard.Mines.Any(m => m.ShutdownTooManyTimes);

        public IEnumerable Play()
        {
            var random = new Random();
            var gameBoardMap = GameBoard.Create(random);

            GameBoard = gameBoardMap;

            yield return null;
            yield return new PhaseIndicator("Setup phase.");

            // draw cards from location deck until you have one that is not occupied by a mine
            LocationCard initialWarehouseCard;
            do
            {
                initialWarehouseCard = gameBoardMap.LocationsDeck.DrawOne();

            } while (!(initialWarehouseCard.Location.Occupant is EmptyLocation));

            yield return new CardsMessage(
                "You pulled this card from the locations deck. A storage yard will be added here.",
                new ICard[] { initialWarehouseCard }
            );

            // once you get a location place the warehouse & reset the location deck
            var warehouse = new Warehouse();
            initialWarehouseCard.Location.SetOccupant(warehouse);
            gameBoardMap.LocationsDeck.ResetDeck();

            // draw X cards from each mine resource pile
            var cardQueue = new Queue<MineOutputCard>(gameBoardMap.Mines
                .SelectMany(m => m.Deck.Draw(GameConfiguration.ResourceMiningCardsDrawnToStartTheGame))
                .OrderBy(_ => random.Next())
                .ToArray());

            yield return new CardsMessage(
                "These cards were pulled and will be added to respective Mines.",
                cardQueue.Cast<ICard>().Take(GameConfiguration.InitialResourcesToMines).ToArray()
            );

            // the first y cards drawn, add resources to the respective mines
            for (int y = 0; y < GameConfiguration.InitialResourcesToMines; y++)
            {
                var nextCard = cardQueue.Dequeue();
                nextCard.Mine.ProduceFromCard(nextCard);
            }

            yield return new CardsMessage(
                "These cards were pulled and will be added to your storage yard.",
                cardQueue.Cast<ICard>()
            );

            // generate resources for the remaining z cards and put them in the warehouse
            for (int z = 0; z < GameConfiguration.InitialResourcesToWarehouse; z++)
            {
                var nextCard = cardQueue.Dequeue();
                var resources = nextCard.Mine.GenerateResourcesForCard(nextCard);
                warehouse.ReceiveResources(resources, gameBoardMap.SellOffPile);
            }

            while (!GameIsWon && !GameIsLost)
            {
                var resolveTransports = gameBoardMap.Routes.SelectMany(r => r.Transports).Where(t => t.IsAtDestination);
                if (resolveTransports.Any())
                {
                    yield return new PhaseIndicator("Transport resolution phase.");
                    // resolve transports
                    foreach (var transport in resolveTransports)
                    {
                        transport.Route.RemoveTransport(transport);
                        var trar = new TransitResolutionActionRequest(transport);
                        yield return trar;
                        trar.Action.Execute(gameBoardMap);
                    }
                }

                // select purchase orders if needed
                if (gameBoardMap.PurchaseOrderSlots.Any(s => s == null))
                {
                    yield return new PhaseIndicator("Purchase order selection phase.");

                    // draw 5 corporation cards and place them in a row
                    // foreach corporation card draw a sale card and place it next to the corporation
                    // foreach corporation card fraw a location card and place it next to the sale card
                    // these are 5 prospective sales, you must select up to 3 to fill active PO slots
                    var poOptions = Enumerable
                        .Range(0, GameConfiguration.PurchaseOrderOptionCount)
                        .Select(_ => new PurchaseOrder(
                            gameBoardMap.CorporationsDeck.DrawOne(),
                            gameBoardMap.SalesDeck.DrawOne(),
                            gameBoardMap.LocationsDeck.DrawOne()
                        ))
                        .ToArray();

                    var picks = new PickPurchaseOrders(poOptions, gameBoardMap.PurchaseOrderSlots.Count(s => s == null));
                    yield return picks;

                    if (picks.SelectedPurchaseOrders.Count() != gameBoardMap.PurchaseOrderSlots.Count(s => s == null))
                        throw new Exception("did not make enough PO selections");

                    // move each purchase order to one of the active PO slots
                    var poQueue = new Queue<PurchaseOrder>(picks.SelectedPurchaseOrders);
                    for (int i = 0; i < gameBoardMap.PurchaseOrderSlots.Length; i++)
                        if (gameBoardMap.PurchaseOrderSlots[i] == null)
                            gameBoardMap.PurchaseOrderSlots[i] = poQueue.Dequeue();
                }

                yield return new PhaseIndicator("Actions phase.");

                // run 5 actions
                for (int round = 0; round < GameConfiguration.ActionPointsPerRound; round++)
                {
                    var playerActionRequest = new PlayerActionRequest($"Round {round + 1}: initiate resource moves", round == 0);
                    yield return playerActionRequest;

                    var action = playerActionRequest.Action;
                    if (action == null)
                        throw new Exception("no player action was passed back");

                    action.Execute(GameBoard);

                    if (action is BuildWarehouse) break;
                }

                // board actions
                yield return new PhaseIndicator("Process board phase.");

                // close out any fulfilled purchase orders
                for (var i = 0; i < gameBoardMap.PurchaseOrderSlots.Length; i++)
                {
                    gameBoardMap.PurchaseOrderSlots[i].ProcessRound();
                    if (gameBoardMap.PurchaseOrderSlots[i].HasBeenFulfilled)
                        gameBoardMap.PurchaseOrderSlots[i] = null;
                }

                // produce mine resources
                var cards = new List<ICard>();
                foreach (var mine in gameBoardMap.Mines)
                {
                    var c = mine.ProcessRound();
                    if (c != null) cards.Add(c);
                }
                yield return new CardsMessage(
                    $"You pulled these cards. Your mines have added the amounts to their storage yards.",
                    cards);

                // increment transports
                foreach (var transport in gameBoardMap.Routes.SelectMany(r => r.Transports))
                    transport.ProcessRound();
            }
        }
    }

    public abstract class PlayerInteraction { }

    public class CardsMessage
    {
        public CardsMessage(string message,  IEnumerable<ICard> cards)
        {
            Message = message;
            Cards = cards;
        }
        public string Message { get; }
        public IEnumerable<ICard> Cards { get; }
    }

    public class PhaseIndicator
    {
        public PhaseIndicator(string message)
        {
            Message = message;
        }
        public string Message { get; }
    }

    public class ShowCard
    {
        public ShowCard(ICard card)
        {
            Card = card;
        }
        public ICard Card { get; }
    }

    public class ShowRejectedCard : ShowCard
    {
        public ShowRejectedCard(ICard card) : base(card) { }
    }

    public class ShowAcceptedCard : ShowCard
    {
        public ShowAcceptedCard(ICard card) : base(card) { }
    }

    public class PickPurchaseOrders : PlayerInteraction
    {
        private List<PurchaseOrder> _options;
        private List<PurchaseOrder> _selected;
        public PickPurchaseOrders(IEnumerable<PurchaseOrder> purchaseOrders, int selectCount)
        {
            SelectCount = selectCount;
            _options = new List<PurchaseOrder>(purchaseOrders);
            _selected = new List<PurchaseOrder>();
        }
        public string Message => "Pick 3 purchase orders to start the game with.";
        public int SelectCount { get; }
        public IEnumerable<PurchaseOrder> PurchaseOrderOptions => _options;
        public IEnumerable<PurchaseOrder> SelectedPurchaseOrders => _selected;
        public void AddSelection(PurchaseOrder purchaseOrder) => _selected.Add(purchaseOrder);
        public void RemoveSelection(PurchaseOrder purchaseOrder) => _selected.Remove(purchaseOrder);
    }

    public abstract class PlayerAction
    {
        public abstract void Execute(GameBoard gameBoard);
    }

    public class PlayerActionRequest : PlayerInteraction
    {
        public PlayerActionRequest(string description, bool mayBuildWarehouse)
        {
            MayBuildWarehouse = mayBuildWarehouse;
            Description = description;
        }
        public bool MayBuildWarehouse { get; }
        public string Description { get; }
        public PlayerAction Action { get; set; }
    }

    public class Pass : PlayerAction
    {
        public override void Execute(GameBoard gameBoard)
        {
        }
    }


    public class ShipResourceAction : PlayerAction
    {
        public ShipResourceAction(Route route, IEnumerable<Resource> resourceUnits, Location from, Location to, ShippingMethods method)
        {
            Route = route;
            ResourceUnits = resourceUnits;
            FromLocation = from;
            ToLocation = to;
            Method = method;
        }
        public Route Route { get; }
        public IEnumerable<Resource> ResourceUnits { get; }
        public Location FromLocation { get; }
        public Location ToLocation { get; }
        public ShippingMethods Method { get; }

        public override void Execute(GameBoard gameBoard)
        {
            if (!Route.EligibleShippingMethods.HasFlag(Method))
                throw new Exception("Route does not have selected shipping method");

            var transport =
                Method == ShippingMethods.Ship ? new FreighterShip(Route, ToLocation, FromLocation, ResourceUnits) :
                Method == ShippingMethods.Train ? new Train(Route, ToLocation, FromLocation, ResourceUnits) :
                Method == ShippingMethods.Truck ? (ResourceTransport)new Truck(Route, ToLocation, FromLocation, ResourceUnits) :
                throw new Exception("invalid shipping method");

            Route.AddTransport(transport);
        }
    }

    public class ProgressPurchaseOrder : PlayerAction
    {
        public ProgressPurchaseOrder(PurchaseOrder purchaseOrder, IEnumerable<Resource> resourceDeposit)
        {
            PurchaseOrder = purchaseOrder;
            ResourceDeposits = resourceDeposit;
        }
        public PurchaseOrder PurchaseOrder { get; }
        public IEnumerable<Resource> ResourceDeposits { get; }

        public override void Execute(GameBoard gameBoard)
        {
            PurchaseOrder.Progress(ResourceDeposits, gameBoard.SellOffPile);
        }
    }

    public class BuildWarehouse : PlayerAction
    {
        BuildWarehouse(Location location)
        {
            Location = location;
        }

        public Location Location { get; set; }

        public override void Execute(GameBoard gameBoard)
        {
            if (!(Location.Occupant is EmptyLocation)) throw new Exception("trying to place warehouse where there already is something");
            Location.SetOccupant(new Warehouse());
        }
    }

    public abstract class TransitResolutionAction
    {
        public abstract void Execute(GameBoard gameBoard);
    }

    public class TransitResolutionActionRequest
    {
        public TransitResolutionActionRequest(ResourceTransport transport)
        {
            Transport = transport;
        }
        public ResourceTransport Transport { get; }
        public TransitResolutionAction Action { get; set; }
    }

    public class ForwardTransitResolutionAction : TransitResolutionAction
    {
        public ForwardTransitResolutionAction(Route route, IEnumerable<Resource> resourceUnits, Location from, Location to, ShippingMethods method)
        {
            Route = route;
            ResourceUnits = resourceUnits;
            FromLocation = from;
            ToLocation = to;
            Method = method;
        }

        public Route Route { get; }
        public IEnumerable<Resource> ResourceUnits { get; }
        public Location FromLocation { get; }
        public Location ToLocation { get; }
        public ShippingMethods Method { get; }

        public override void Execute(GameBoard gameBoard)
        {
            if (!Route.EligibleShippingMethods.HasFlag(Method))
                throw new Exception("Route does not have selected shipping method");

            var transport =
                Method == ShippingMethods.Ship ? new FreighterShip(Route, ToLocation, FromLocation, ResourceUnits) :
                Method == ShippingMethods.Train ? new Train(Route, ToLocation, FromLocation, ResourceUnits) :
                Method == ShippingMethods.Truck ? (ResourceTransport)new Truck(Route, ToLocation, FromLocation, ResourceUnits) :
                throw new Exception("invalid shipping method");

            Route.AddTransport(transport);
        }
    }

    public class DeliverToWarehouseTransitResolutionAction : TransitResolutionAction
    {
        public DeliverToWarehouseTransitResolutionAction(Warehouse warehouse, IEnumerable<Resource> resourceDeposit)
        {
            ResourceDeposit = resourceDeposit;
        }

        public Warehouse Warehouse { get; }
        public IEnumerable<Resource> ResourceDeposit { get; }

        public override void Execute(GameBoard gameBoard)
        {
            Warehouse.ReceiveResources(ResourceDeposit, gameBoard.SellOffPile);
        }
    }

    public class ProgressPurchaseOrderTransitResolutionAction : TransitResolutionAction
    {
        public ProgressPurchaseOrderTransitResolutionAction(PurchaseOrder purchaseOrder, IEnumerable<Resource> resourceDeposit)
        {
            PurchaseOrder = purchaseOrder;
            ResourceDeposits = resourceDeposit;
        }

        public PurchaseOrder PurchaseOrder { get; }
        public IEnumerable<Resource> ResourceDeposits { get; }

        public override void Execute(GameBoard gameBoard)
        {
            PurchaseOrder.Progress(ResourceDeposits, gameBoard.SellOffPile);
        }
    }
}
