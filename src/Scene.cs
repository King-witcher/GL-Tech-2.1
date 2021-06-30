using System;
using System.Drawing;
using System.Collections.Generic;

namespace GLTech2
{
    /// <summary>
    /// Represents a scene, which stores a set of elements that can be or not rendered and, at least, one observer.
    /// </summary>
    public unsafe sealed partial class Scene : IDisposable
    {
        internal Action StartAction;
        internal Action OnFrameAction;
        internal SceneData* unmanaged;
        private Observer activeObserver;    //Provisional
        private List<Element> elements = new List<Element>();
        private List<PhysicalPlane> physicalPlanes = new List<PhysicalPlane>(); // Provisional

        /// <summary>
        /// Gets a new instance of Scene.
        /// </summary>
        public Scene()
        {
            Texture background = new Texture((PixelBuffer)new Bitmap(1, 1));
            unmanaged = SceneData.Create(background);
        }

        /// <summary>
        /// Gets a new instance of Scene.
        /// </summary>
        /// <param name="background">Background material rendered behind everything</param>
        public Scene(Texture background) =>
            unmanaged = SceneData.Create(background);


        /// <summary>
        /// Gets and sets the current observer from where the scene will be rendered.
        /// </summary>
        public Observer ActiveObserver
        {
            // Seems spaguetti!
            get => activeObserver;
            set
            {
                if (value is null || value.scene is null)   // null pointer
                {
                    activeObserver = value;
                    unmanaged->activeObserver = value.unmanaged;
                }
                else
                    Debug.InternalLog(
                        origin: "Scene",
                        message: "Can\'t set a camera that is not in this scene.",
                        debugOption: Debug.Options.Error);
            }
        }

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
            if (element is null)
                throw new ArgumentNullException("Cannot add null elements.");

            if (element.scene != null)
            {
                Debug.InternalLog(
                    origin: "Scene",
                    message: $"\"{element}\" is already bound to scene {element.scene}. Adding operation will be aborted.",
                    debugOption: Debug.Options.Error);
                return;
            }

            if (element.ReferencePoint != null && element.ReferencePoint.scene != this)       // Must be tested
            {
                element.ReferencePoint = null;
            }

            // Spaguetti!
            if (element is VisualPlane visualPlane)
                UnmanagedAddWall(visualPlane);
            else if (element is Sprite sprite)
                UnmanagedAddSprite(sprite);
            else if (element is Observer observer)
                UnmanagedAddObserver(observer);
            else if (element is PhysicalPlane physicalPlane)
                physicalPlanes.Add(physicalPlane);

            StartAction += element.StartAction;
            OnFrameAction += element.OnFrameAction;

            elements.Add(element);
            element.scene = this;

            foreach (var item in element.childs)
                AddElement(item);
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
        public void AddElements(params Element[] elements)
        {
            AddElements((IEnumerable<Element>) elements);
        }

        private void UnmanagedAddWall(VisualPlane w)
        {
            unmanaged->Add(w.unmanaged);
        }
        private void UnmanagedAddSprite(Sprite s) => throw new NotImplementedException();

        private void UnmanagedAddObserver(Observer p)
        {
            ActiveObserver = p;
        }

        public static bool RayTest(Ray ray, PhysicalPlane plane, out float distance)
        {
            float
                drx = plane.WorldNormal.x,
                dry = plane.WorldNormal.y;

            float det = ray.direction.x * dry - ray.direction.y * drx;

            if (det == 0)
            {
                distance = float.PositiveInfinity;
                return false;
            }

            float spldet = ray.direction.x * (ray.start.y - plane.WorldPosition.y) - ray.direction.y * (ray.start.x - plane.WorldPosition.x);
            float dstdet = drx * (ray.start.y - plane.WorldPosition.y) - dry * (ray.start.x - plane.WorldPosition.x);
            float spltmp = spldet / det;
            float dsttmp = dstdet / det;
            if (spltmp < 0 || spltmp >= 1 || dsttmp <= 0) // dsttmp = 0 means column height = x/0.
            {
                distance = float.PositiveInfinity;
                return false;
            }
            distance = dsttmp;
            return true;
        }

        private static bool CircleTest(Ray ray, PhysicalPlane plane, out float distance)
        {
            throw new NotImplementedException();
        }

        // Provisional; must be improved
        public PhysicalPlane RayCast(Ray ray, out float distance)
        {
            PhysicalPlane nearest = null;
            distance = float.PositiveInfinity;

            foreach (PhysicalPlane current in physicalPlanes)
            {
                RayTest(ray, current, out float currentDistance);

                if (currentDistance < distance)
                {
                    nearest = current;
                    distance = currentDistance;
                }
            }
            return nearest;
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            foreach(Element item in elements)
                item.Dispose();

            SceneData.Delete(unmanaged);
            unmanaged = null;
            activeObserver = null;

            elements.Clear();
        }
    }
}
