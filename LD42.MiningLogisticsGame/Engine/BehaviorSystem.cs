using Microsoft.Xna.Framework;
using System.Linq;

namespace LD42.MiningLogisticsGame.Engine
{
    abstract class Behavior : Component
    {
        public GameTime GameTime { get; internal set; }
        public virtual void Update() { }
    }

    class BehaviorSystem : GameComponent
    {
        private GameStateManager gameStateManager;

        public BehaviorSystem(Game game, GameStateManager gameStateManager) : base(game)
        {
            this.gameStateManager = gameStateManager;
        }

        public override void Update(GameTime gameTime)
        {
            GameInput.Update();
            gameStateManager
                .GameStates
                .SelectMany(s => s.Entities)
                .SelectMany(e => e.Components)
                .OfType<Behavior>()
                .ToList()
                .ForEach(b =>
                {
                    b.GameTime = gameTime;
                    b.Update();
                });
        }
    }
}
