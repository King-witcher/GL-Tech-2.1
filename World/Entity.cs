﻿using System;
using System.Collections.Generic;

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

        public IEnumerable<Entity> GetNodes()
        {
            Queue<Entity> queue = new ();
            queue.Enqueue(this);

            while (queue.TryDequeue(out Entity current))
            {
                yield return current;
                foreach (Entity entity in current.Children)
                    queue.Enqueue(entity);
            }
        }

        internal virtual IEnumerable<Plane> GetPlanes()
        {
            foreach (Entity entity in GetNodes())
                if (entity is Plane plane)
                    yield return plane;
        }

        internal virtual IEnumerable<Collider> GetColliders()
        {
            foreach (Entity entity in GetNodes())
                if (entity is Collider collider)
                    yield return collider;
        }

        internal virtual IEnumerable<Floor> GetFloors()
        {
            foreach (Entity entity in GetNodes())
                if (entity is Floor floor)
                    yield return floor;
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

interface Implementada
{
    static void Test()
    {
        Console.WriteLine("asdf");
    }
}

class Teste : Implementada
{
    public static void Test()
    {

    }
}