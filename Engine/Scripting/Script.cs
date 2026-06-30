using GLTech.Input;
using GLTech.World;
using System.Reflection;

namespace GLTech.Scripting
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ScriptStartAttribute : Attribute
    {
        public ScriptStartAttribute() { }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ScriptUpdateAttribute : Attribute
    {
        public ScriptUpdateAttribute() { }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ScriptFixedUpdateAttribute : Attribute
    {
        public ScriptFixedUpdateAttribute() { }
    }

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
                    var method = GetMethodByAttribute<ScriptStartAttribute>() ?? GetMethodByName("Start");

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
                    var method = GetMethodByAttribute<ScriptUpdateAttribute>() ?? GetMethodByName("Update");
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
                    var method = GetMethodByAttribute<ScriptFixedUpdateAttribute>() ?? GetMethodByName("FixedUpdate");
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

        private MethodInfo? GetMethodByAttribute<T>() where T : new()
        {
            var methods = GetType().GetMethods();
            foreach (var method in methods)
                if (method.GetCustomAttribute(typeof(T)) != null && method.GetParameters().Length == 0)
                    return method;
            return null;
        }

        private MethodInfo? GetMethodByName(string methodName)
        {
            return GetType().GetMethod(methodName,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy,
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
