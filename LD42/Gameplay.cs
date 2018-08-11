using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LD42
{
    public class MiningLogisticsGame
    {
        public GameBoard GameBoard { get; private set; }

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
            } while (!(initialWarehouseCard.Location.Occupant is EmptyLocation));

            yield return initialWarehouseCard;

            // once you get a location place the warehouse & reset the location deck
            var warehouse = new Warehouse();
            initialWarehouseCard.Location.SetOccupant(warehouse);
            gameBoardMap.LocationsDeck.ResetDeck();

            // draw X cards from each mine resource pile
            var cardQueue = new Queue<MineOutputCard>(gameBoardMap.Mines
                .SelectMany(m => m.Deck.Draw(GameConfiguration.ResourceMiningCardsDrawnToStartTheGame))
                .OrderBy(_ => random.Next())
                .ToArray());

            yield return cardQueue;

            // the first y cards drawn, add resources to the respective mines
            for (int y = 0; y < GameConfiguration.InitialResourcesToMines; y++)
            {
                var nextCard = cardQueue.Dequeue();
                nextCard.Mine.ProduceFromCard(nextCard);
            }

            yield return cardQueue;

            // generate resources for the remaining z cards and put them in the warehouse
            for (int z = 0; z < GameConfiguration.InitialResourcesToWarehouse; z++)
            {
                var nextCard = cardQueue.Dequeue();
                var resources = nextCard.Mine.GenerateResourcesForCard(nextCard);
                warehouse.ReceiveResources(resources);
            }

            yield return warehouse;

            // draw 5 corporation cards and place them in a row
            // foreach corporation card draw a sale card and place it next to the corporation
            // foreach corporation card fraw a location card and place it next to the sale card
            // these are 5 prospective sales, you must select 3 to fill active PO slots
            var initialPOOptions = Enumerable
                .Range(0, GameConfiguration.PurchaseOrderOptionCount)
                .Select(_ => new PurchaseOrder(
                    gameBoardMap.CorporationsDeck.DrawOne(),
                    gameBoardMap.SalesDeck.DrawOne(),
                    gameBoardMap.LocationsDeck.DrawOne()
                ))
                .ToArray();

            var picks = new PickPurchaseOrders(initialPOOptions, gameBoardMap.ActivePurchaseOrderSlotCount);
            yield return picks;

            if (picks.SelectedPurchaseOrders.Count() != gameBoardMap.ActivePurchaseOrderSlotCount)
                throw new Exception("did not make enough PO selections");

            // move each purchase order to one of the active PO slots
            var poQueue = new Queue<PurchaseOrder>(picks.SelectedPurchaseOrders);
            for (int i = 0; i < gameBoardMap.ActivePurchaseOrderSlotCount; i++)
                gameBoardMap.ActivePurchaseOrders[i] = poQueue.Dequeue();
        }
    }

    public abstract class PlayerInteraction { }

    class PickPurchaseOrders : PlayerInteraction
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
    }
}
