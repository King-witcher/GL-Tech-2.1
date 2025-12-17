using GLTech.Input;
using GLTech.World;
using System;
using System.Reflection;

namespace GLTech.Scripting
{
    public abstract partial class Script
    {
        private Entity entity;

        private static Action Scheduled;

        internal static void Schedule(Action action)
        {

        }

        internal static void RunScheduled()
        {
            Scheduled?.Invoke();
            Scheduled = null;
        }

        internal void Assign(Entity e)
        {
            entity = e;
        }

        protected internal Script() { }

        protected internal Entity Entity => entity;

        protected internal Scene Scene => entity.Scene;

        // Gets the method called void Start()
        private Action? start = null;
        internal Action? StartAction
        {
            get
            {
                if (start == null)
                {
                    var method = GetMethod("Start");
                    if (method != null)
                        start = () => method.Invoke(this, null);
                }

                return start;
            }
        }

        // Gets the method called void OnFrame()
        private Action? onFrame = null;
        internal Action? OnFrameAction
        {
            get
            {
                if (onFrame is null)
                {
                    var method = GetMethod("OnFrame");
                    if (method != null)
                        onFrame = () => method.Invoke(this, null);
                }

                return onFrame;
            }
        }

        private Action? onFixedTick = null;
        internal Action? OnFixedTickAction
        {
            get
            {
                if (onFixedTick is null)
                {
                    var method = GetMethod("OnFixedTick");
                    if (method != null)
                        onFixedTick = () => method.Invoke(this, null);
                }

                return onFixedTick;
            }
        }

        private Action<ScanCode> onKeyDown = null;
        internal Action<ScanCode> OnKeyDownAction
        {
            get
            {
                if (onKeyDown == null)
                {
                    var method = GetMethod("OnKeyDown", typeof(ScanCode));
                    if (method != null)
                    {
                        onKeyDown = inputKey =>
                        {
                            var param = new object[] { inputKey };
                            method.Invoke(this, param);
                        };
                    }
                }

                return onKeyDown;
            }
        }

        private Action<ScanCode> onKeyUp = null;
        internal Action<ScanCode> OnKeyUpAction
        {
            get
            {
                if (onKeyUp == null)
                {
                    var method = GetMethod("OnKeyUp", typeof(ScanCode));
                    if (method != null)
                    {
                        onKeyUp = inputKey =>
                        {
                            var param = new object[] { inputKey };
                            method.Invoke(this, param);
                        };
                    }
                }

                return onKeyUp;
            }
        }

        private MethodInfo GetMethod(string methodName)
        {
            return GetType().GetMethod(methodName,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                null,
                new Type[0],
                null);
        }

        private MethodInfo GetMethod(string methodName, params Type[] parameterTypes)
        {
            return GetType().GetMethod(methodName,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                null,
                parameterTypes,
                null);
        }
    }
}
