using LD42.MiningLogisticsGame.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD42.MiningLogisticsGame.Gameplay
{
    class GameplayState : GameState
    {
        public GameplayState()
        {
            Entity
                .Empty
                .AddComponent(new Transform())
                .AddComponent(new Sprite(GameContent.LazyGet<Texture2D>("board"), origin: Vector2.Zero))
                .AddToState(this);

            Entity
                .Empty
                .AddComponent(new Transform())
                .AddComponent(new Sprite(GameContent.LazyGet<Texture2D>("pointer"), origin: Vector2.Zero))
                .AddComponent(new PointerBehavior())
                .AddToState(this);
        }
    }

    static class StateBuilderEx
    {
        public static Entity AddToState(this Entity self, GameState state)
        {
            state.AddEntity(self);
            return self;
        }
    }
}
