using System;

namespace Engine.World
{
    public abstract partial class Entity : IDisposable
    {
        private Scene scene;

        internal Entity()
        {
            name = "unnamed";
        }

        public const int MAX_NAME_LENGTH = 63;
        private string name;
        public string Name
        {
            get => name;
            set
            {
                if (value.Length > MAX_NAME_LENGTH)
                    Debug.InternalLog("Maximum recommended name lenght was exceeded. You may have problems" +
                        "when trying to save this map in a file.", Debug.Options.Warning);
                name = value;
            }
        }

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
