namespace GLTech.World
{
    public abstract partial class Entity : IDisposable
    {
        private Scene? scene;
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

        public IEnumerable<Entity> Traverse()
        {
            Queue<Entity> queue = new();
            queue.Enqueue(this);

            while (queue.TryDequeue(out Entity? current))
            {
                yield return current;
                foreach (Entity entity in current.Children)
                    queue.Enqueue(entity);
            }
        }

        private protected virtual Vector PositionData { get; set; }

        private protected virtual Vector DirectionData { get; set; } = Vector.North;

        public Scene? Scene => scene;

        public virtual void Dispose() { }

        internal void AssignScene(Scene scene)
        {
            this.scene = scene;
        }
    }
}