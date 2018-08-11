using System;

namespace LD42
{
    class MiningLogisticsGame
    {
        private GameBoardMap gameBoardMap;

        public MiningLogisticsGame()
        {
            gameBoardMap = GameBoardMap.Create(new Random());
        }

        public static bool GameOver { get; private set; }
    }
}
