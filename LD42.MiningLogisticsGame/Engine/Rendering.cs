using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace LD42.MiningLogisticsGame.Engine
{
    class Sprite : Component
    {
        public Sprite(
            Lazy<Texture2D> texture,
            Color? color = null,
            Vector2? origin = null,
            SpriteEffects? spriteEffects = null,
            float? layerDepth = null)
        {
            Texture = texture;
            Color = color ?? Color.White;
            Origin = origin;
            SpriteEffects = spriteEffects ?? SpriteEffects.None;
            LayerDepth = layerDepth ?? 0;
        }
        public Lazy<Texture2D> Texture { get; set; }
        public Color Color { get; set; }
        public Vector2? Origin { get; set; }
        public SpriteEffects SpriteEffects { get; set; }
        public float LayerDepth { get; set; }
    }

    class RenderingSystem : DrawableGameComponent
    {
        private GameStateManager gameStateManager;

        private SpriteBatch spriteBatch;

        public RenderingSystem(Game game, GameStateManager gameStateManager) : base(game)
        {
            this.gameStateManager = gameStateManager;
        }

        public override void Initialize()
        {
            base.Initialize();
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            gameStateManager
                .GameStates
                .SelectMany(s => s.Entities)
                .SelectMany(e => e.Components)
                .OfType<Sprite>()
                .ToList()
                .ForEach(s =>
                {
                    var t = s.Entity.Components.OfType<Transform>().FirstOrDefault() ?? new Transform();

                    spriteBatch.Draw(
                        s.Texture.Value,
                        t.Position,
                        null,
                        s.Color,
                        t.Rotation,
                        s.Origin ?? s.Texture.Value.Bounds.Center.ToVector2(),
                        t.Scale,
                        s.SpriteEffects,
                        s.LayerDepth);
                });

            spriteBatch.End();
        }
    }
}
