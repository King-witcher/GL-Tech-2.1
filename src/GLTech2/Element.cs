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
        /// Its correspondent scene. If the element is not bound to any scene, returns null.
        /// </summary>
        public Scene Scene => scene;

        private protected virtual Vector PositionData { get; set; }

        private protected virtual Vector DirectionData { get; set; }

        /// <summary>
        /// Releases unmanaged data, if any.
        /// </summary>
        public virtual void Dispose() { }

        internal unsafe virtual void AddToSScene(SScene* data) { }
    }
}
