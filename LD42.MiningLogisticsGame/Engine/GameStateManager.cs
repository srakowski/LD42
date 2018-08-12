using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace LD42.MiningLogisticsGame.Engine
{
    abstract class GameState
    {
        private List<Entity> _entities = new List<Entity>();

        public GameStateManager GameStateManager { get; internal set; }

        public IEnumerable<Entity> Entities => _entities.ToArray();

        public bool IsDead { get; internal set; }

        public void AddEntity(Entity entity)
        {
            _entities.Add(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            _entities.Remove(entity);
        }
    }

    class GameStateManager : GameComponent
    {
        private List<GameState> _gameStates = new List<GameState>();

        public IEnumerable<GameState> GameStates => _gameStates;

        public GameStateManager(Game game) : base(game)
        {
        }

        public void AddGameState(GameState gameState)
        {
            gameState.GameStateManager = this;
            _gameStates.Add(gameState);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var gameState in _gameStates.Where(gs => gs.IsDead).ToArray())
                _gameStates.Remove(gameState);
        }
    }
}
