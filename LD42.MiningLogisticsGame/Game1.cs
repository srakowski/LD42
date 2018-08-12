using LD42.MiningLogisticsGame.Engine;
using LD42.MiningLogisticsGame.Gameplay;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD42.MiningLogisticsGame
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            Content.RootDirectory = "Content";

            var gameStateManager = new GameStateManager(this);
            gameStateManager.AddGameState(new GameplayState());
            Components.Add(gameStateManager);

            var bs = new BehaviorSystem(this, gameStateManager);
            Components.Add(bs);

            var rs = new RenderingSystem(this, gameStateManager);
            Components.Add(rs);
        }

        protected override void Initialize() => base.Initialize();

        protected override void LoadContent()
        {
            void Load<T>(string key) where T : class =>
                GameContent.Put(key, Content.Load<T>(key));
            Load<Texture2D>("board");
            Load<Texture2D>("pointer");
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }
    }
}
