using System;
using System.Reflection;

namespace GLTech2
{
    /// <summary>
    ///     Represents a behaviour script that can be attached to an element. Similar to MonoBehaviours from Unity3D.
    /// </summary>
    /// <remarks>
    ///     When a scene starts running, the engine is responsible for finding and executing Start() and Update() method properly.
    ///     <para>
    ///     The Start() method, if any, will be executed once the scene is loaded. Update() method, otherwise, is called after every frame is rendered.
    ///     </para>
    ///     <para>
    ///     Important note: all behaviours must be added to it's elements before adding the Element to a Scene. Otherwise, the Behaviour won't work properly.
    ///     </para>
    /// </remarks>
    public abstract partial class Behaviour
    {
        internal Element element;

        /// <summary>
        /// Makes possible the creation of derived classes.
        /// </summary>
        protected internal Behaviour() { }

        /// <summary>
        /// Element to wich the script is attached.
        /// </summary>
        protected internal Element Element { get => element; }

        /// <summary>
        /// Scene to which the element is attached.
        /// </summary>
        protected internal Scene Scene { get => element.scene; }

        // Gets the method called void Start()
        private Action start = null;
        internal Action StartAction
        {
            get
            {
                if (start is null)
                {
                    MethodInfo startInfo = GetType().GetMethod("Start",
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                        null,
                        new Type[0],
                        null);

                    if (startInfo != null)
                        start = () => startInfo?.Invoke(this, null);
                }

                return start;
            }
        }

        // Gets the method called void OnFrame()
        private Action onFrame = null;
        internal Action OnFrameAction
        {
            get
            {
                if (onFrame is null)
                {
                    MethodInfo updateInfo = GetType().GetMethod("OnFrame",
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                        null,
                        new Type[0],
                        null);

                    if (updateInfo != null)
                        onFrame = () => updateInfo?.Invoke(this, null);
                }

                return onFrame;
            }
        }
    }
}
