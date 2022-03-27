﻿using System;
using System.Collections.Generic;
using System.Drawing;
using Engine.Imaging;
using Engine.Data;
using Engine.Scripting;

namespace Engine.World
{
    public unsafe partial class Scene : IDisposable
    {
        internal SScene* unmanaged;

        private List<Entity> entities = new List<Entity>();
        private List<Collider> colliders = new List<Collider>();
        private Camera camera;

        public Scene()
        {
            unmanaged = SScene.Create();

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
        public int ColliderCount => colliders.Count;
        public int EntityCount => entities.Count;
        public int PlaneCount => unmanaged->plane_count;
        public Camera Camera => camera;

        public Texture Background
        {
            get => unmanaged->background;
            set => unmanaged->background = value;
        }

        public void Add(Entity entity)
        {
            #region Entry vefifications
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

            Queue<Entity> queue = new Queue<Entity>();
            queue.Enqueue(entity);

            while (queue.Count > 0)
            {
                Entity current = queue.Dequeue();
                addSingle(current);
                current.childs.ForEach(child => queue.Enqueue(child));
            }

            void addSingle(Entity entity)
            {
                entities.Add(entity);

                if (entity is Collider collider)
                    colliders.Add(collider);

                entity.AssignScene(this);
                unmanaged->Add(entity);

                #region Caches every script and subscribe to see when new scripts are added.
                foreach (Script s in entity.Scripts)
                {
                    Start += s.StartAction;
                    OnFrame += s.OnFrameAction;
                }
                entity.OnAddScript += script =>
                {
                    Start += script.StartAction;
                    OnFrame += script.OnFrameAction;
                };
                #endregion
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

            SScene.Delete(unmanaged);
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