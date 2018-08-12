using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace LD42.MiningLogisticsGame.Engine
{
    interface ISprite
    {
        bool Render { get; set; }
        Entity Entity { get; set; }
    }

    class TextSprite : Component, ISprite
    {
        public TextSprite(
            Lazy<SpriteFont> spriteFont,
            string text = "",
            Color? color = null,
            Vector2? origin = null,
            SpriteEffects? spriteEffects = null,
            float? layerDepth = null)
        {
            SpriteFont = spriteFont;
            Text = text;
            Color = color ?? Color.White;
            Origin = origin;
            SpriteEffects = spriteEffects ?? SpriteEffects.None;
            LayerDepth = layerDepth ?? 0;
        }
        public Lazy<SpriteFont> SpriteFont { get; set; }
        public string Text { get; set; }
        public Color Color { get; set; }
        public Vector2? Origin { get; set; }
        public SpriteEffects SpriteEffects { get; set; }
        public float LayerDepth { get; set; }
        public bool Render { get; set; } = true;
    }

    class Sprite : Component, ISprite
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
        public bool Render { get; set; } = true;
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
                .OfType<ISprite>()
                .Where(s => s.Render)
                .ToList()
                .ForEach(s =>
                {
                    var t = s.Entity.Components.OfType<Transform>().FirstOrDefault() ?? new Transform();
                    if (s.Entity.Parent != null)
                        t = TransformToParent(t, s.Entity);

                    if (s is Sprite spr)
                    {
                        spriteBatch.Draw(
                            spr.Texture.Value,
                            t.Position,
                            null,
                            spr.Color,
                            t.Rotation,
                            spr.Origin ?? Vector2.Zero,
                            t.Scale,
                            spr.SpriteEffects,
                            spr.LayerDepth);
                    }
                    else if (s is TextSprite ts)
                    {
                        spriteBatch.DrawString(
                            ts.SpriteFont.Value,
                            ts.Text,
                            t.Position,
                            ts.Color,
                            t.Rotation,
                            ts.Origin ?? Vector2.Zero,
                            t.Scale,
                            ts.SpriteEffects,
                            ts.LayerDepth);
                    }
                });

            spriteBatch.End();
        }

        private Transform TransformToParent(Transform t, Entity entity)
        {
            if (entity.Parent == null) return t;

            var pT = entity.Parent.GetComponent<Transform>();
            if (entity.Parent.Parent != null)
                pT = TransformToParent(pT, entity.Parent);

            return t.GetRelativeTo(pT);

        }
    }
}
