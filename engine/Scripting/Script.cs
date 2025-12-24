using GLTech.Input;
using GLTech.World;
using System.Reflection;

namespace GLTech.Scripting
{
    public abstract partial class Script
    {
        private Entity? entity;

        protected internal Script() { }

        protected internal Entity? Entity
        {
            get
            {
                return entity;
            }
            internal set
            {
                entity = value;
            }
        }

        protected internal Scene Scene
        {
            get
            {
                if (entity == null)
                    throw new InvalidOperationException("Script is not attached to an entity.");
                if (entity.Scene == null)
                    throw new InvalidOperationException("Entity is not attached to a scene.");
                return entity.Scene;
            }
        }

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
        private Action? update = null;
        internal Action? UpdateAction
        {
            get
            {
                if (update is null)
                {
                    var method = GetMethod("Update");
                    if (method != null)
                        update = () => method.Invoke(this, null);
                }

                return update;
            }
        }

        private Action? fixedUpdate = null;
        internal Action? FixedUpdateAction
        {
            get
            {
                if (fixedUpdate is null)
                {
                    var method = GetMethod("FixedUpdate");
                    if (method != null)
                        fixedUpdate = () => method.Invoke(this, null);
                }

                return fixedUpdate;
            }
        }

        private Action<ScanCode>? onKeyDown = null;
        internal Action<ScanCode>? OnKeyDownAction
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

        private Action<ScanCode>? onKeyUp = null;
        internal Action<ScanCode>? OnKeyUpAction
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

        private MethodInfo? GetMethod(string methodName)
        {
            return GetType().GetMethod(methodName,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static,
                null,
                [],
                null);
        }

        private MethodInfo? GetMethod(string methodName, params Type[] parameterTypes)
        {
            return GetType().GetMethod(methodName,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static,
                null,
                parameterTypes,
                null);
        }
    }
}
