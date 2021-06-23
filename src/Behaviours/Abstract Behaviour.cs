using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace GLTech2
{
    /// <summary>
    ///     Represents a behaviour script that can be attached to an element. Similar to MonoBehaviours on Unity3D.
    /// </summary>
    /// <remarks>
    ///     When a scene starts running, the engine is responsible for finding and executing Start() and Update() method properly.
    ///     <para>
    ///         The Start() method, if any, will be executed once the scene is loaded. Update() method, otherwise, is called after every frame is rendered.
    ///     </para>
    /// </remarks>
    public abstract class Behaviour
    {
        internal Element element;

        /// <summary>
        ///     Makes possible the creation of derived classes.
        /// </summary>
        protected internal Behaviour() { }

        /// <summary>
        ///     Element to wich the script is attached.
        /// </summary>
        protected internal Element Element { get => element; }

        /// <summary>
        ///     Scene to which the element is attached.
        /// </summary>
        protected internal Scene Scene { get => element.scene; }

        private Action startMethod = null;
        internal Action StartMethod
        {
            get
            {
                if (startMethod is null)
                {
                    MethodInfo startInfo = GetType().GetMethod("Start",
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                        null,
                        new Type[0],
                        null);

                    startMethod = () => startInfo?.Invoke(this, null);
                }

                return startMethod;
            }
        }

        private Action updateMethod = null;
        internal Action UpdateMethod
        {
            get
            {
                if (updateMethod is null)
                {
                    MethodInfo startInfo = GetType().GetMethod("Update",
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                        null,
                        new Type[0],
                        null);

                    updateMethod = () => startInfo?.Invoke(this, null);
                }

                return updateMethod;
            }
        }

        // Start()
        // Update()
        // BeginCollide(Element collisor)
        // EndCollide(Element collisor)
    }
}
