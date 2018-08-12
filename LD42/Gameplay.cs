using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LD42
{
    public class MiningLogisticsGame
    {
        public GameBoard GameBoard { get; private set; }

        public bool GameIsWon { get; private set; }
        public bool GameIsLost { get; private set; }

        public IEnumerable Play()
        {
            var random = new Random();
            var gameBoardMap = GameBoard.Create(random);

            GameBoard = gameBoardMap;
            yield return gameBoardMap;

            // draw cards from location deck until you have one that is not occupied by a mine
            LocationCard initialWarehouseCard;
            do
            {
                initialWarehouseCard = gameBoardMap.LocationsDeck.DrawOne();
                if (!(initialWarehouseCard.Location.Occupant is EmptyLocation))
                    yield return new ShowRejectedCard(initialWarehouseCard);

            } while (!(initialWarehouseCard.Location.Occupant is EmptyLocation));

            yield return new ShowAcceptedCard(initialWarehouseCard);

            // once you get a location place the warehouse & reset the location deck
            var warehouse = new Warehouse();
            initialWarehouseCard.Location.SetOccupant(warehouse);
            gameBoardMap.LocationsDeck.ResetDeck();

            // draw X cards from each mine resource pile
            var cardQueue = new Queue<MineOutputCard>(gameBoardMap.Mines
                .SelectMany(m => m.Deck.Draw(GameConfiguration.ResourceMiningCardsDrawnToStartTheGame))
                .OrderBy(_ => random.Next())
                .ToArray());

            // the first y cards drawn, add resources to the respective mines
            for (int y = 0; y < GameConfiguration.InitialResourcesToMines; y++)
            {
                var nextCard = cardQueue.Dequeue();
                yield return new ShowAcceptedCard(nextCard);
                nextCard.Mine.ProduceFromCard(nextCard);
            }

            // generate resources for the remaining z cards and put them in the warehouse
            for (int z = 0; z < GameConfiguration.InitialResourcesToWarehouse; z++)
            {
                var nextCard = cardQueue.Dequeue();
                yield return new ShowAcceptedCard(nextCard);
                var resources = nextCard.Mine.GenerateResourcesForCard(nextCard);
                warehouse.ReceiveResources(resources);
            }

            while (!GameIsWon && !GameIsLost)
            {
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

                if (poOptions.Any())
                {
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
            }
        }
    }

    public abstract class PlayerInteraction { }

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
                Method == ShippingMethods.Ship ? new FreighterShip(ToLocation, FromLocation, ResourceUnits) :
                Method == ShippingMethods.Train ? new Train(ToLocation, FromLocation, ResourceUnits) :
                Method == ShippingMethods.Truck ? (ResourceTransport)new Truck(ToLocation, FromLocation, ResourceUnits) :
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

    public class TransitResolutionActionRequest
    {
    }
}
