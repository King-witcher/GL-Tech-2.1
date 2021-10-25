using System;
using System.Reflection;

using GLTech2.Entities;

namespace GLTech2
{
    public abstract partial class Behaviour
    {
        private Entity entity;

        internal void Assign(Entity e)
        {
            entity = e;
        }

        protected internal Behaviour() { }

        protected internal Entity Entity => entity;

        protected internal Scene Scene => entity.Scene;

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
