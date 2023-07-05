using System;
using System.Collections.Generic;
using Engine.Structs;
using Engine.Imaging;
using Engine.Scripting;

namespace Engine.World
{
    public unsafe partial class Scene : IDisposable
    {
        internal SceneStruct* unmanaged;

        private List<Entity> entities = new List<Entity>();
        private List<Collider> colliders = new List<Collider>();
        private Camera camera;

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

        internal Action Start { get; private set; }
        internal Action OnFrame { get; private set; }
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
            OnKeyDown += script.OnKeyDownAction;
            OnKeyUp += script.OnKeyUpAction;
        }

        internal void UnubscribeScript(Script script)
        {
            Start -= script.StartAction;
            OnFrame -= script.OnFrameAction;
            OnKeyDown -= script.OnKeyDownAction;
            OnKeyUp -= script.OnKeyUpAction;
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
                Debug.InternalLog(
                    message: $"The element \"{entity}\" cannot be added to the scene \"{this}\" because it's already bound to \"{entity.Scene}\".",
                    debugOption: Debug.Options.Error);
                return;
            }

            // Only root elements can be added to a Scene, which will add all of it's children.
            // Besides, Element class's referencing system cannot allow elements to be added to different scenes.
            if (entity.Parent != null)
            {
                Debug.InternalLog(
                    message: $"The element \"{entity}\" cannot be added to the scene \"{this}\" because it already has a parent element. Only root elements are allowed to be directly added to a scene. Consider adding root element and all the child elements will recursively added.",
                    debugOption: Debug.Options.Error);
                return;
            }
            #endregion

            foreach (Entity node in entity.GetNodes())
                add(node);

            void add(Entity entity)
            {
                entities.Add(entity);
                if (entity is Collider collider)
                    colliders.Add(collider);

                entity.AssignScene(this);

                unmanaged->Add(entity);

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

        /// <summary>
        /// Releases unmanaged resources.
        /// </summary>
        protected virtual void Delete()
        {

        }
    }
}