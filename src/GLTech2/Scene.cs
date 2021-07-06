using System;
using System.Drawing;
using System.Collections.Generic;

// Implementar culling de colisão

namespace GLTech2
{
    /// <summary>
    /// Represents a scene, which stores a set of elements that can be or not rendered and, at least, one observer.
    /// </summary>
    public unsafe sealed partial class Scene : IDisposable
    {
        internal Action OnStart;
        internal Action OnFrame;
        internal SScene* unmanaged;
        private List<Element> elements = new List<Element>();
        private List<Collider> colliders = new List<Collider>();

        /// <summary>
        /// Gets a new instance of Scene.
        /// </summary>
        public Scene()
        {
            Texture background = new Texture((PixelBuffer)new Bitmap(1, 1));
            unmanaged = SScene.Create(background);
        }

        /// <summary>
        /// Gets a new instance of Scene.
        /// </summary>
        /// <param name="background">Background texture rendered behind everything</param>
        public Scene(Texture background) =>
            unmanaged = SScene.Create(background);

        /// <summary>
        /// Gets how many walls the scene fits.
        /// </summary>
        public int WallCount => unmanaged->plane_count;

        /// <summary>
        /// Gets and sets the background texture of the Scene.
        /// </summary>
        public Texture Background
		{
            get => unmanaged->background;
            set => unmanaged->background = value;
        }

        /// <summary>
        ///     Add a new element and every child it has to the scene.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Every element can only be added once to a scene. Trying to add an element twice or an element that is already bound to another scene will generate command line warning.
        ///     </para>
        ///     <para>
        ///         This method was not yet fully tested!
        ///     </para>
        /// </remarks>
        /// <param name="element">An element to be added</param>
        public void AddElement(Element element)
        {
            #region Verifications
            // Obvious thing
            if (element == null)
                throw new ArgumentNullException("Cannot add null elements.");

            // Elements cannot be added twice + elements cannot have already been added to another scene.
            if (element.scene != null)
            {
                Debug.InternalLog(
                    origin: "Scene",
                    message: $"\"{element}\" is already bound to scene {element.scene}. Adding operation will be aborted.",
                    debugOption: Debug.Options.Error);
                return;
            }

            // Only root elements can be added to a Scene, which will add all of it's children.
            // Besides, Element class's referencing system cannot allow elements to be added to different scenes.
            if (element.Parent != null)
            {
                Debug.InternalLog(
                    origin: "Scene",
                    message: $"The element \"{element}\" you are trying to add to the scene has a ReferencePoint. {element.scene}. Only elements with no ReferencePoint are allowed to be directly added to a scene. If you want to add this element, add its root element and all it's child elements will recursively added.",
                    debugOption: Debug.Options.Error);
                return;
            }
            #endregion

            Add(element);

            void Add(Element element)
            {
                // Add element to element cache list.
                elements.Add(element);

                // In case it's a collider, add to collider cache too.
                if (element is Collider collider)
                    colliders.Add(collider);

                // Set element.scene.
                element.scene = this;

                // Add element to scene unmanaged data. Each base element can be added on it's on way.
                element.AddToSScene(unmanaged);

                // Add element's behaviours.
                OnStart += element.OnStart;
                OnFrame += element.OnFrame;

                // Make it recursively to all childs.
                foreach (var item in element.childs)
                    Add(item);
            }
        }

        /// <summary>
        ///     Adds a whole set of elements.
        /// </summary>
        /// <param name="elements">Set of elements</param>
        public void AddElements(IEnumerable<Element> elements)
        {
            foreach (Element item in elements)
            {
                if (item is null)
                    break;

                AddElement(item);
            }
        }

        /// <summary>
        ///     Add an array of elements via params.
        /// </summary>
        /// <param name="elements">Array of elements</param>
        public void AddElements(params Element[] elements) =>
            AddElements((IEnumerable<Element>) elements);

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            foreach(Element item in elements)
                item.Dispose();

            SScene.Delete(unmanaged);
            unmanaged = null;

            elements.Clear();
            colliders.Clear();

            OnStart = null;
            OnFrame = null;
        }
    }
}
