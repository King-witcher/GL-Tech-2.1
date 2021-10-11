using System;

namespace GLTech2
{
    /// <summary>
    /// Represents an element that can be part of a Scene.
    /// </summary>
    public abstract partial class Element : IDisposable
    {
        internal Scene scene;

        internal Element() { }

        /// <summary>
        /// Allows subclasses to store the Position.
        /// </summary>
        /// <remarks>
        /// Important: Elements that parent this will not follow it imediatelly for performance and code health reasons.
        /// Changing this property is only recommended if the element is not a reference point to any other and changing positions is a significant performance bottleneck in your application.
        /// Otherwise, always use Element.WorldPosition property instead.
        /// </remarks>
        private protected virtual Vector PositionData { get; set; }

        /// <summary>
        /// Allows subclasses to store the Direction.
        /// </summary>
        /// <remarks>
        /// Important: Elements that parent this will not follow it imediatelly for performance and code health reasons.
        /// Changing this property is only recommended if the element is not a reference point to any other and changing positions is a significant performance bottleneck in your application.
        /// Otherwise, always use Element.WorldDirection property instead.
        /// </remarks>
        /// <remarks>
        /// Remember to set it before parenting any object!
        /// </remarks>
        private protected virtual Vector DirectionData { get; set; }

        /// <summary>
        /// Its correspondent scene. If the element is not bound to any scene, returns null.
        /// </summary>
        public Scene Scene => scene;

        /// <summary>
        /// Releases unmanaged data, if any.
        /// </summary>
        public virtual void Dispose() { }

        internal unsafe virtual void AddToSScene(SScene* data) { }
    }
}
