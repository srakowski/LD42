using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;

namespace LD42.MiningLogisticsGame.Engine
{
    class Entity
    {
        private List<Component> _components = new List<Component>();

        internal T GetComponent<T>() => Components.OfType<T>().FirstOrDefault();

        public IEnumerable<Component> Components => _components.ToArray();

        public Entity AddComponent(Component component)
        {
            component.Entity = this;
            _components.Add(component);
            return this;
        }

        public void RemoveComponent(Component component)
        {
            _components.Remove(component);
        }

        public static Entity Empty => new Entity();
    }

    abstract class Component
    {
        public Entity Entity { get; set; }
    }

    class Transform : Component
    {
        public Vector2 Position { get; set; } = Vector2.Zero;
        public float Rotation { get; set; } = 0f;
        public Vector2 Scale { get; set; } = Vector2.One;
    }
}
