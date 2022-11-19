using System;
using System.Collections.Generic;
using System.Collections;
using Engine.Data;

namespace Engine.World
{
    public abstract partial class Entity : IDisposable
    {
        private Scene scene;
        private static int entityCount = 0;

        internal Entity()
        {
            name = $"Entity {entityCount}";
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

        internal abstract unsafe IEnumerable<PlaneStruct*> GetPlaneStructs();

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
