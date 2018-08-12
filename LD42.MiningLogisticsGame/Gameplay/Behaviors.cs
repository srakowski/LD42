using LD42.MiningLogisticsGame.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static LD42.MiningLogisticsGame.Gameplay.StateBuilderEx;
using System;

namespace LD42.MiningLogisticsGame.Gameplay
{
    class GameStepBehavior : Behavior
    {
        private IEnumerator play;
        private GameState state;
        private List<Entity> prevDisplayEntities = new List<Entity>();

        public GameStepBehavior(IEnumerator play, GameState state)
        {
            this.play = play;
            this.state = state;
        }

        public override void Update()
        {
            if (GameInput.CurrKBState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right) &&
                GameInput.PrevKBState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Right))
            {
                play.MoveNext();
                prevDisplayEntities.ForEach(f => state.RemoveEntity(f));
                prevDisplayEntities.Clear();
                if (play.Current is PhaseIndicator pi)
                {
                    AddMessage(pi.Message);
                }
                else if (play.Current is CardsMessage cm)
                {
                    AddMessage(cm.Message);
                    for (int x = 0; x < cm.Cards.Count(); x++)
                        AddCard(cm.Cards.ElementAt(x), x);
                }
            }
        }

        private void AddMessage(string text)
        {
            prevDisplayEntities.Add(
                Entity.Empty
                    .AddComponent(new Transform
                    {
                        Position = new Vector2(30, 890)
                    })
                    .AddComponent(new TextSprite(
                        SpriteFont("message"),
                        text, color: Color.Black))
                    .AddToState(state));
        }

        private void AddCard(ICard card, int x)
        {
            var c = Entity.Empty
                .AddComponent(new Transform
                {
                    Position = new Vector2(30 + (x * 100), 920),
                });

            if (card is LocationCard)
            {
                c = c.AddComponent(new Sprite(Texture2D("locationcard")));
            }
            else if (card is MineOutputCard resource)
            {
                c = c.AddComponent(
                    new Sprite(Texture2D($"{resource.Mine.ResourceTypeDescription.ToLower()}card")));
            }

            c.AddToState(state);
            prevDisplayEntities.Add(c);
        }
    }


    class PointerBehavior : Behavior
    {
        public override void Update()
        {
            var t = Entity.GetComponent<Transform>();
            t.Position = GameInput.CurrMouseState.Position.ToVector2();
        }
    }


    class WarehouseResourcePipBehavior : Behavior
    {
        private int pipNumber;
        private Warehouse warehouse;
        private Sprite sprite;

        public WarehouseResourcePipBehavior(int pipNumber, Warehouse warehouse)
        {
            this.pipNumber = pipNumber;
            this.warehouse = warehouse;
        }

        public override void Initialize()
        {
            this.sprite = this.Entity.GetComponent<Sprite>();
            var transform = this.Entity.GetComponent<Transform>();
            transform.Position = new Vector2(
                ((pipNumber / 4) * 18.75f) + 6.75f,
                ((pipNumber % 4) * 18.75f) + 6.75f);
        }

        public override void Update()
        {
            var rs = warehouse.UnitsOfResources.ElementAtOrDefault(pipNumber);
            if (rs == null)
            {
                sprite.Render = false;
                return;
            }

            sprite.Render = true;
            if (rs is Resource.Iron) sprite.Texture = GameContent.LazyGet<Texture2D>("ironpip");
            else if (rs is Resource.Copper) sprite.Texture = GameContent.LazyGet<Texture2D>("copperpip");
            else if (rs is Resource.Silver) sprite.Texture = GameContent.LazyGet<Texture2D>("silverpip");
            else if (rs is Resource.Zinc) sprite.Texture = GameContent.LazyGet<Texture2D>("zincpip");
        }
    }

    class FavorBehavior : Behavior
    {
        private Corporation corporation;
        private TextSprite ts;

        public FavorBehavior(Corporation corporation)
        {
            this.corporation = corporation;
        }

        public override void Initialize()
        {
            ts = Entity.GetComponent<TextSprite>();
        }

        public override void Update()
        {
            ts.Text = corporation.Favor.ToString();
        }
    }

    class ShowWarehousesBehavior : Behavior
    {
        private GameBoard gameBoard;
        private GameState state;

        public ShowWarehousesBehavior(GameBoard gameBoard, GameState state)
        {
            this.gameBoard = gameBoard;
            this.state = state;
        }

        public override void Update()
        {
            foreach (var warehouse in gameBoard.Locations.Select(l => l.Occupant).OfType<Warehouse>()
                .Concat(gameBoard.Locations.Select(l => l.Occupant).OfType<Mine>().Select(m => m.Storage))
                .Where(w => !w.Established))
            {
                Prefabs
                    .Warehouse(warehouse)
                    .AddToState(state);
                warehouse.Established = true;
            }
        }
    }

}
