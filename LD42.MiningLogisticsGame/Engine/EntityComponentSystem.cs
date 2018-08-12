using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;

namespace LD42.MiningLogisticsGame.Engine
{
    class Entity
    {
        public IEnumerable<Entity> Children => children.ToArray();

        private List<Entity> children = new List<Entity>();

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

        public void AddChild(Entity entity)
        {
            entity.Parent = this;
            children.Add(entity);
        }

        public static Entity Empty => new Entity();

        public Entity Parent { get; private set; }
    }

    abstract class Component
    {
        public Entity Entity { get; set; }
    }

    class Transform : Component
    {
        public Vector2 Position { get; set; } = Vector2.Zero;
        public float Rotation { get; set; } = 0f;
        public float Scale { get; set; } = 1f;
        public Transform GetRelativeTo(Transform t)
        {
            var m = Matrix.Identity *
                Matrix.CreateRotationZ(t.Rotation) *
                Matrix.CreateScale(t.Scale) *
                Matrix.CreateTranslation(t.Position.X, t.Position.Y, 0f);

            return new Transform
            {
                Position = Vector2.Transform(Position, m),
                Rotation = Rotation + t.Rotation,
                Scale = Scale + (t.Scale - 1f),
            };
        }
    }
}
