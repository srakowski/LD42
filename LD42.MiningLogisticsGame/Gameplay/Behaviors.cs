using LD42.MiningLogisticsGame.Engine;

namespace LD42.MiningLogisticsGame.Gameplay
{
    class PointerBehavior : Behavior
    {
        public override void Update()
        {
            var t = Entity.GetComponent<Transform>();
            t.Position = GameInput.CurrMouseState.Position.ToVector2();
        }
    }

}
