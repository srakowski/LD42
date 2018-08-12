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
        private object awaiting = null;

        public GameStepBehavior(IEnumerator play, GameState state)
        {
            this.play = play;
            this.state = state;
        }

        public override void Update()
        {
            if (awaiting != null) return;

            if (GameInput.CurrKBState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right) &&
                GameInput.PrevKBState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Right))
            {
                DoNext();
            }
        }

        private void DoNext()
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
            else if (play.Current is PickPurchaseOrders ppo)
            {
                AddMessage(ppo.Message);
                var x = 0;
                var set = 0;
                var pairs = new List<Tuple<Entity, PurchaseOrder>>();
                foreach (var po in ppo.PurchaseOrderOptions)
                {
                    var cbPosX = (30 + (set * 75)) + (set * 300);

                    var checkbox = Entity.Empty
                            .AddComponent(new Transform
                            {
                                Position = new Vector2(cbPosX, 924)
                            })
                            .AddComponent(new BoolBehavior())
                            .AddComponent(new Sprite(Texture2D("cbfalse")))
                            .AddToState(state);

                    pairs.Add(new Tuple<Entity, PurchaseOrder>(checkbox, po));

                    prevDisplayEntities.Add(checkbox);

                    var offset = (70 + (set * 75)) + (set * 300);

                    foreach (var card in new ICard[]
                    {
                            po.Corporation,
                            po.Sale,
                            po.ShipToLocation,
                    })
                    {
                        AddCard(card, x, offset);
                        x++;
                    }
                    x = 0;
                    set++;
                }
                prevDisplayEntities.Add(Entity.Empty
                    .AddComponent(new Transform())
                    .AddComponent(new PoSelectorBehavior(pairs.ToArray(), ppo, () => {
                        awaiting = null;
                        DoNext();
                    }))
                    .AddComponent(new Sprite(Texture2D("uparrow")))
                    .AddToState(state));
                awaiting = ppo;
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

        private void AddCard(ICard card, int x, int offset = 30)
        {
            var c = Entity.Empty
                .AddComponent(new Transform
                {
                    Position = new Vector2(offset + (x * 100), 924),
                });

            c = PopulateCardEntity(card, c);

            c.AddToState(state);
            prevDisplayEntities.Add(c);
        }

        public static Entity PopulateCardEntity(ICard card, Entity c)
        {
            if (card is LocationCard)
            {
                c = c.AddComponent(new Sprite(Texture2D("locationcard")));
            }
            else if (card is MineOutputCard resource)
            {
                c = c.AddComponent(
                    new Sprite(Texture2D($"{resource.Mine.ResourceTypeDescription.ToLower()}card")));
            }
            else if (card is CorporationCard corp)
            {
                c = c.AddComponent(
                    new Sprite(Texture2D($"{corp.Corporation.Name.Split().First().ToLower()}card")));
            }
            else if (card is SaleCard saleCard)
            {
                c = c.AddComponent(
                    new Sprite(Texture2D($"salecard")));
            }

            return c;
        }
    }

    class PoSelectorBehavior : Behavior
    {
        private Tuple<Entity, PurchaseOrder>[] cbsAndPos;
        private PickPurchaseOrders ppo;
        private Action done;
        public List<int> indicesSelected = new List<int>();
        public int currentIndex = 0;

        public PoSelectorBehavior(Tuple<Entity, PurchaseOrder>[] cbsAndPos, PickPurchaseOrders ppo, Action done)
        {
            this.cbsAndPos = cbsAndPos;
            this.ppo = ppo;
            this.done = done;
        }

        public override void Update()
        {
            base.Update();

            var selectedCount = cbsAndPos.Select(i => i.Item1.GetComponent<BoolBehavior>().Value).Count(b => b);

            if (GameInput.CurrKBState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space) &&
                GameInput.PrevKBState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Space))
            {
                var bb = cbsAndPos[currentIndex].Item1.GetComponent<BoolBehavior>();
                if (selectedCount < ppo.SelectCount || bb.Value)
                {
                    bb.Value = !bb.Value;
                    var s = cbsAndPos[currentIndex].Item1.GetComponent<Sprite>();
                    s.Texture = Texture2D($"cb{bb.Value.ToString().ToLower()}");
                }
            }
            if (GameInput.CurrKBState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left) &&
                GameInput.PrevKBState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Left))
            {
                currentIndex--;
                if (currentIndex < 0)
                    currentIndex = cbsAndPos.Length - 1;
            }
            if (GameInput.CurrKBState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right) &&
                GameInput.PrevKBState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Right))
            {
                currentIndex++;
                if (currentIndex >= cbsAndPos.Length)
                    currentIndex = 0;
            }

            if (GameInput.CurrKBState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter) &&
                 GameInput.PrevKBState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Enter) &&
                 selectedCount == ppo.SelectCount)
            {
                var selected = cbsAndPos.Where(x => x.Item1.GetComponent<BoolBehavior>().Value)
                    .Select(x => x.Item2);
                selected.ToList().ForEach(s => ppo.AddSelection(s));
                done();
            }

                var t = Entity.GetComponent<Transform>();
            var c = cbsAndPos[currentIndex].Item1.GetComponent<Transform>().Position;
            t.Position = c + new Vector2(4, 32);
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

    class BoolBehavior : Behavior
    {
        public bool Value;
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

    class ShowPurchaseOrdersBehavior : Behavior
    {
        private GameBoard gameBoard;
        private GameState state;

        public ShowPurchaseOrdersBehavior(GameBoard gameBoard, GameState state)
        {
            this.gameBoard = gameBoard;
            this.state = state;
        }

        public override void Update()
        {
            for (int i = 0; i < gameBoard.PurchaseOrderSlots.Length; i++)
            {
                var po = gameBoard.PurchaseOrderSlots[i];
                if (po == null)
                    continue;

                if (po.Established) continue;

                var poRow = Entity
                    .Empty
                    .AddComponent(new Transform
                    {
                        Position = new Vector2(1434, 73 + (160 * i)),
                    });
                    
                var x = 0;
                foreach (var card in new ICard[] { po.Corporation, po.Sale, po.ShipToLocation })
                {
                    var cardEntity = Entity.Empty.AddComponent(
                        new Transform { Position = new Vector2((x * 113) + 12, 9) });

                    GameStepBehavior.PopulateCardEntity(card, cardEntity);

                    poRow.AddChild(cardEntity);
                    x++;
                }

                poRow.AddToState(state);

                po.Established = true;
            }
        }
    }

}
