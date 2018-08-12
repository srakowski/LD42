using LD42.MiningLogisticsGame.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using static LD42.MiningLogisticsGame.Gameplay.StateBuilderEx;
using System;

namespace LD42.MiningLogisticsGame.Gameplay
{
    class GameplayState : GameState
    {
        public GameplayState()
        {
            var game = new MiningLogisticsGameplay();
            var play = game.Play().GetEnumerator();
            play.MoveNext();

            Entity
                .Empty
                .AddComponent(new GameStepBehavior(play, this))
                .AddToState(this);

            Entity
                .Empty
                .AddComponent(new Transform())
                .AddComponent(new Sprite(Texture2D("board"), origin: Vector2.Zero))
                .AddToState(this);

            Entity
                .Empty
                .AddComponent(new ShowWarehousesBehavior(game.GameBoard, this))
                .AddToState(this);

            foreach (var corp in game.GameBoard.Corporations)
            {
                Entity.Empty
                    .AddComponent(new Transform
                    {
                        Position = corp.Name == "LuDare" ? new Vector2(1640, 630) :
                            corp.Name == "Adico" ? new Vector2(1640, 715) :
                            corp.Name == "SparkTech Industries" ? new Vector2(1640, 800) :
                            Vector2.Zero,
                    })
                    .AddComponent(new FavorBehavior(corp))
                    .AddComponent(new TextSprite(SpriteFont("large"), color: Color.Black))
                    .AddToState(this);

            }

            //foreach (var mine in game.GameBoard.Locations.Select(l => l.Occupant).OfType<Mine>())
            //{

            //}



            //Entity
            //    .Empty
            //    .AddComponent(new Transform())
            //    .AddComponent(new Sprite(GameContent.LazyGet<Texture2D>("pointer"), origin: Vector2.Zero))
            //    .AddComponent(new PointerBehavior())
            //    .AddToState(this);
        }

        
    }

    static class StateBuilderEx
    {
        public static Entity AddToState(this Entity self, GameState state)
        {
            state.AddEntity(self);
            foreach (var child in self.Children) child.AddToState(state);
            return self;
        }
        public static System.Lazy<Texture2D> Texture2D(string texture) => GameContent.LazyGet<Texture2D>(texture);
        public static System.Lazy<SpriteFont> SpriteFont(string texture) => GameContent.LazyGet<SpriteFont>(texture);
    }

    static class Prefabs
    {
        public static Entity Warehouse(Warehouse warehouse)
        {
            var wh = 
                Entity
                    .Empty
                    .AddComponent(new Transform
                    {
                        Position = GetWarehousePosition(warehouse),
                    })
                    .AddComponent(new Sprite(Texture2D("warehouse"), origin: Vector2.Zero));

            for (int i = 0; i < GameConfiguration.WarehouseCapacityInResourceUnits; i++)
            {
                wh.AddChild(Entity
                    .Empty
                    .AddComponent(new Transform())
                    .AddComponent(new WarehouseResourcePipBehavior(i, warehouse))
                    .AddComponent(new Sprite(Texture2D("ironpip")))
                );
            }

            return wh;
        }

        private static Vector2 GetWarehousePosition(Warehouse warehouse)
        {
            var ln = warehouse.Location.Name;
            if (ln == "Seattle") return new Vector2(114, 90);
            if (ln == "Reno") return new Vector2(126, 285);
            if (ln == "Los Angeles") return new Vector2(88, 456);
            if (ln == "Salt Lake City") return new Vector2(278, 340);
            if (ln == "Billings") return new Vector2(372, 173);
            if (ln == "Denver") return new Vector2(441, 389);
            if (ln == "Tuscon") return new Vector2(303, 560);
            if (ln == "Kansas City") return new Vector2(702, 414);
            if (ln == "Houston") return new Vector2(665, 671);
            if (ln == "Duluth") return new Vector2(756, 176);
            if (ln == "New Orleans") return new Vector2(823, 663);
            if (ln == "Knoxville") return new Vector2(962, 528);
            if (ln == "Jacksonville") return new Vector2(1078, 627);
            if (ln == "Indianapolis") return new Vector2(885, 385);
            if (ln == "Detroit") return new Vector2(974, 268);
            if (ln == "Boston") return new Vector2(1263, 224);
            return Vector2.Zero;
        }
    }
}
