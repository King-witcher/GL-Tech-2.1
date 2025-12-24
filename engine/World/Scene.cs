using Engine.Physics;
using GLTech.Scripting;
using GLTech.Structs;

namespace GLTech.World
{
    public unsafe partial class Scene : IDisposable
    {
        public Scene()
        {
            raw = RawScene.Create();

            // Bad smell
            Camera defaultCamera = new();
            Add(defaultCamera);
            camera = defaultCamera;
        }

        public Scene(Texture background) : this()
        {
            raw->background = background;
        }

        internal RawScene* raw;

        private static Logger logger = new(typeof(Scene).Name);
        private List<Entity> entities = new List<Entity>();
        private Dictionary<string, Entity> entityNames = new Dictionary<string, Entity>();
        private Camera camera;
        private CollisionSystem CollisionSystem { get; } = new CollisionSystem();

        private List<Action> starts = new List<Action>();
        private List<Action> updates = new List<Action>();
        private List<Action> fixedUpdates = new List<Action>();

        public int ColliderCount => 0;
        public int EntityCount => entities.Count;
        public int PlaneCount => raw->plane_list.count;
        public Camera Camera => camera;

        public Texture Background
        {
            get => raw->background;
            set => raw->background = value;
        }

        internal void CacheScript(Script script)
        {
            if (script.StartAction != null)
                starts.Add(script.StartAction);
            if (script.UpdateAction != null)
                updates.Add(script.UpdateAction);
            if (script.FixedUpdateAction != null)
                fixedUpdates.Add(script.FixedUpdateAction);
        }

        internal void UnCacheScript(Script script)
        {
            if (script.StartAction != null)
                starts.Remove(script.StartAction);
            if (script.UpdateAction != null)
                updates.Remove(script.UpdateAction);
            if (script.FixedUpdateAction != null)
                fixedUpdates.Remove(script.FixedUpdateAction);
        }

        public Entity? FindByname(string name)
        {
            if (entityNames.TryGetValue(name, out Entity? entity))
                return entity;
            else return null;
        }

        public void Add(Entity entity)
        {
            #region Deffensive programming part
            // Obvious thing
            if (entity == null)
                // Not sure if it's a good idea to make the application.
                throw new ArgumentNullException("Cannot add null elements.");

            // Elements cannot be added twice + elements cannot have already been added to another scene.
            if (entity.Scene != null)
            {
                logger.Error($"The element \"{entity}\" cannot be added to the scene \"{this}\" because it's already bound to \"{entity.Scene}\".");
                return;
            }

            // Only root elements can be added to a Scene, which will add all of it's children.
            // Besides, Element class's referencing system cannot allow elements to be added to different scenes.
            if (entity.Parent != null)
            {
                logger.Error($"The element \"{entity}\" cannot be added to the scene \"{this}\" because it already has a parent element. Only root elements are allowed to be directly added to a scene. Consider adding root element and all the child elements will recursively added.");
                return;
            }
            #endregion

            foreach (Entity node in entity.Traverse())
            {
                entities.Add(entity);
                entityNames.Add(entity.Name, entity);

                entity.AssignScene(this);


                if (entity is Plane plane)
                {
                    raw->Add(plane.unmanaged);
                }
                else if (entity is Floor floor)
                {
                    raw->AddFloor(floor.unmanaged);
                }
                else if (entity is Ceiling ceiling)
                {
                    raw->AddCeiling(ceiling.unmanaged);
                }
                else if (entity is Camera camera)
                {
                    raw->Add(camera.raw);
                }
                else if (entity is Collider collider)
                {
                    CollisionSystem.Add(collider);
                }

                // Scene caches every script for performance reasons.
                // When a new script is added to an Entity, the scene should be told.
                foreach (Script script in entity.Scripts)
                    CacheScript(script);
            }
        }

        public void Add(IEnumerable<Entity> entities)
        {
            foreach (Entity item in entities)
                Add(item);
        }

        public void Add(params Entity[] entities) =>
            Add((IEnumerable<Entity>)entities);

        public void Dispose()
        {
            foreach (Entity item in entities)
                item.Dispose();

            RawScene.Delete(raw);
            raw = null;

            Delete();

            entities.Clear();
        }

        internal void Start()
        {
            foreach (var action in starts)
                action();
        }

        internal void Update()
        {
            foreach (var action in updates)
                action();
        }

        internal void FixedUpdate()
        {
            foreach (var action in fixedUpdates)
                action();
        }

        /// <summary>
        /// Releases unmanaged resources.
        /// </summary>
        protected virtual void Delete()
        {
            // ?
        }
    }
}