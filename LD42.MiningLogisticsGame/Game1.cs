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
            IsMouseVisible = true;

            Content.RootDirectory = "Content";

            var gameStateManager = new GameStateManager(this);
            //gameStateManager.AddGameState(new GameplayState());
            gameStateManager.AddGameState(new LastMinuteGame());

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
            Load<Texture2D>("emptycard");
            Load<Texture2D>("ironcard");
            Load<Texture2D>("ironpip");
            Load<Texture2D>("locationcard");
            Load<Texture2D>("ludarecard");
            Load<Texture2D>("resourcepip");
            Load<Texture2D>("salecard");
            Load<Texture2D>("ship");
            Load<Texture2D>("shutdownpip");
            Load<Texture2D>("silvercard");
            Load<Texture2D>("silverpip");
            Load<Texture2D>("sparktechcard");
            Load<Texture2D>("train");
            Load<Texture2D>("truck");
            Load<Texture2D>("warehouse");
            Load<Texture2D>("zinccard");
            Load<Texture2D>("zincpip");
            Load<Texture2D>("adicocard");
            Load<Texture2D>("coppercard");
            Load<Texture2D>("copperpip");
            Load<Texture2D>("cbtrue");
            Load<Texture2D>("cbfalse");
            Load<Texture2D>("uparrow");
            Load<Texture2D>("cu");
            Load<Texture2D>("fe");
            Load<Texture2D>("ag");
            Load<Texture2D>("zn");
            Load<SpriteFont>("large");
            Load<SpriteFont>("location");
            Load<SpriteFont>("message");
            Load<SpriteFont>("status");
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(22, 22, 27));
            base.Draw(gameTime);
        }
    }

    class LastMinuteGame : GameState
    {
        public LastMinuteGame()
        {
            var grid = new Entity[4, 4];

            var fe = GameContent.LazyGet<Texture2D>("fe");
            var ag = GameContent.LazyGet<Texture2D>("ag");
            var cu = GameContent.LazyGet<Texture2D>("cu");
            var zn = GameContent.LazyGet<Texture2D>("zn");

            for (int x = 0; x < 4; x++)
                for (int y = 0; y < 4; y++)
                {
                    grid[x, y] =
                        Entity.
                            Empty
                            .AddComponent(new Transform { Position = new Vector2(100 + (x * 150), 100 + (y * 150)) })
                            .AddComponent(new Sprite(fe));

                    this.AddEntity(grid[x, y]);
                }

            Entity
                .Empty
                .AddComponent(new Transform { Position = new Vector2(1000, 150) })
                .AddComponent(new Sprite(GameContent.LazyGet<Texture2D>("ludare")));
        }
    }
}
