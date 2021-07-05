using System;

namespace GLTech2
{
    /// <summary>
    /// Represents an element that can be part of a Scene.
    /// </summary>
    public abstract partial class Element : IDisposable
    {
        internal Scene scene;
        private Vector worldVelocity;

        internal Element() { }

        /// <summary>
        /// Its correspondent scene. If the element is not bound to any scene, returns null.
        /// </summary>
        public Scene Scene => scene;

        /// <summary>
        /// Gets and sets the absolute position of an Element without and allows subclasses to store the Position the way they want.
        /// </summary>
        /// <remarks>
        /// Important: Elements that take this element as reference point will not follow it imediatelly for performance and code health reasons. Changing this property is only recommended if the element is not a reference point to any other and changing positions is a significant performance bottleneck in your application. Otherwise, always use Element.Position property instead.
        /// </remarks>
        public abstract Vector WorldPosition { get; set; }

        /// <summary>
        /// Determines how the element stores its normal.
        /// </summary>
        /// <remarks>
        /// Remember to set it before parenting any object!
        /// </remarks>
        public abstract Vector WorldRotation { get; set; } //Provides rotation and scale of the object.

        public Vector WorldVelocity
        {
            get => worldVelocity;
            set => worldVelocity = value;
        }

        public float WorldSpeed
        {
            get => worldVelocity.Module;
            set => worldVelocity.Module = value;
        }

        /// <summary>
        /// Releases unmanaged data, if any.
        /// </summary>
        public virtual void Dispose() { }

        internal unsafe virtual void AddToSScene(SScene* data) { }
    }
}
