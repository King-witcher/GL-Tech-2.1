using System;

namespace Engine.World
{
    public abstract partial class Entity : IDisposable
    {
        private Scene scene;

        internal Entity() { }

        private protected virtual Vector PositionData { get; set; }

        private protected virtual Vector DirectionData { get; set; }

        public Scene Scene => scene;

        public virtual void Dispose() { }

        internal void AssignScene(Scene scene)
        {
            this.scene = scene;
        }
    }
}
