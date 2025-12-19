namespace GLTech.World
{
    public abstract partial class Entity : IDisposable
    {
        private Scene scene;
        private static int entityCount = 0;

        internal Entity()
        {
            name = $"Entity {entityCount++}";
        }

        public const int MAX_NAME_LENGTH = 63;
        private string name;
        public string Name
        {
            get => name;
            set
            {
                if (value.Length > MAX_NAME_LENGTH)
                    Debug.Log("Maximum recommended name lenght was exceeded. You may have problems" +
                        "when trying to save this map in a file.", "Entity", Debug.Options.Warning);
                name = value;
            }
        }

        public IEnumerable<Entity> GetNodes()
        {
            Queue<Entity> queue = new();
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

        internal virtual IEnumerable<Horizontal> GetFloors()
        {
            foreach (Entity entity in GetNodes())
                if (entity is Horizontal floor)
                    yield return floor;
        }

        private protected virtual Vector PositionData { get; set; }

        private protected virtual Vector DirectionData { get; set; } = Vector.Forward;

        public Scene Scene => scene;

        public virtual void Dispose() { }

        internal void AssignScene(Scene scene)
        {
            this.scene = scene;
        }
    }
}