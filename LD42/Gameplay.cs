using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LD42
{
    class MiningLogisticsGame
    {
        public IEnumerable Play()
        {
            var random = new Random();
            var gameBoardMap = GameBoardMap.Create(random);

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


        }
    }
}
