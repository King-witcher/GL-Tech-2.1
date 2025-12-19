using GLTech.Scripting;
using GLTech.Structs;

namespace GLTech.World
{
    public unsafe partial class Scene : IDisposable
    {
        public Scene()
        {
            unmanaged = SceneStruct.Create();

            // Bad smell
            Camera defaultCamera = new();
            Add(defaultCamera);
            camera = defaultCamera;
        }

        public Scene(Texture background) : this()
        {
            unmanaged->background = background;
        }

        internal SceneStruct* unmanaged;

        private static Logger logger = new(typeof(Scene).Name);
        private List<Entity> entities = new List<Entity>();
        private List<Collider> colliders = new List<Collider>();
        private Dictionary<string, Entity> entityNames = new Dictionary<string, Entity>();
        private Camera camera;

        internal Action Start { get; private set; }
        internal Action OnFrame { get; private set; }
        internal Action OnFixedTick { get; private set; }
        internal Action<Input.ScanCode> OnKeyDown { get; private set; }
        internal Action<Input.ScanCode> OnKeyUp { get; private set; }
        public int ColliderCount => colliders.Count;
        public int EntityCount => entities.Count;
        public int PlaneCount => unmanaged->plane_list.count;
        public Camera Camera => camera;

        public Texture Background
        {
            get => unmanaged->background;
            set => unmanaged->background = value;
        }

        internal void SubscribeScript(Script script)
        {
            Start += script.StartAction;
            OnFrame += script.OnFrameAction;
            OnFixedTick += script.OnFixedTickAction;
            OnKeyDown += script.OnKeyDownAction;
            OnKeyUp += script.OnKeyUpAction;
        }

        internal void UnubscribeScript(Script script)
        {
            Start -= script.StartAction;
            OnFrame -= script.OnFrameAction;
            OnFixedTick -= script.OnFixedTickAction;
            OnKeyDown -= script.OnKeyDownAction;
            OnKeyUp -= script.OnKeyUpAction;
        }

        public Entity? FindByname(string name)
        {
            if (entityNames.TryGetValue(name, out Entity entity))
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

            foreach (Entity node in entity.GetNodes())
                add_node(node);

            void add_node(Entity entity)
            {
                entities.Add(entity);
                entityNames.Add(entity.Name, entity);
                if (entity is Collider collider)
                    colliders.Add(collider);

                entity.AssignScene(this);

                if (entity is Plane plane)
                {
                    unmanaged->Add(plane.unmanaged);
                }
                else if (entity is Floor floor)
                {
                    unmanaged->AddFloor(floor.unmanaged);
                }
                else if (entity is Ceiling ceiling)
                {
                    unmanaged->AddCeiling(ceiling.unmanaged);
                }

                //unmanaged->Add(entity);

                // Scene caches every script for performance reasons.
                // When a new script is added to an Entity, the scene should be told.
                foreach (Script script in entity.Scripts)
                    SubscribeScript(script);
            }
        }

        public void Add(IEnumerable<Entity> entities)
        {
            foreach (Entity item in entities)
                Add(item);
        }

        public void AddOnFrame(Action action)
        {
            this.OnFrame += action;
        }

        public void Add(params Entity[] entities) =>
            Add((IEnumerable<Entity>)entities);

        public void Dispose()
        {
            foreach (Entity item in entities)
                item.Dispose();

            SceneStruct.Delete(unmanaged);
            unmanaged = null;

            Delete();

            entities.Clear();
            colliders.Clear();

            Start = null;
            OnFrame = null;
        }

        ~Scene()
        {
            if (unmanaged != null) Dispose();
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