using System;
using System.Collections.Generic;
using Engine.Scripting;

// Talvez as World devam atualizar as relativas tambem. Posso criar uma que atualize apenas a interna e usar por quest'oes de performance.

namespace Engine.World
{
    partial class Entity
    {
        private List<Script> scripts = new List<Script>();

        internal IEnumerable<Script> Scripts => scripts;

        // Just a sintax sugar
        internal Action OnStart
        {
            get
            {
                Action action = null;
                foreach (Script script in scripts)
                    action += script.StartAction;
                return action;
            }
        }

        internal Action OnFrame
        {
            get
            {
                Action action = null;
                foreach (Script script in scripts)
                    action += script.OnFrameAction;
                return action;
            }
        }

        public void AddScript(Script script)
        {
            #region Ensures that the entry is not null and the script was not added to any entity.
            if (script is null)
            {
                Debug.InternalLog(
                    message: $"Cannot add a null script to entity \"{this}\".",
                    debugOption: Debug.Options.Warning);
                return;
            }

            if (ContainsScript(script))
            {
                Debug.InternalLog(
                    message: $"Cannot add script \"{script}\" twice to the same entity \"{this}\".",
                    debugOption: Debug.Options.Warning);
                return;
            }

            if (script.Entity != null)
            {
                Debug.InternalLog(
                    message: $"Cannot add script \"{script}\" to \"{this}\" becuase it's already bound to \"{script.Entity}\".",
                    debugOption: Debug.Options.Error);
                return;
            }
            #endregion

            script.Assign(this);
            scripts.Add(script);

            // Guarantees that the script will still work even if added after the element was added to a scene.
            // This is because the renderer runs scripts directly from the Scene's Script cache for performance reasons.
            if (Scene != null)
                Scene.SubscribeScript(script);
        }

        public void AddScript<ScriptClass>() where ScriptClass : Script, new()
        {
            AddScript(new ScriptClass());
        }

        public void AddScripts(IEnumerable<Script> scripts)
        {
            foreach (Script item in scripts)
                AddScript(item);
        }

        public void AddScripts(params Script[] scripts)
        {
            AddScripts((IEnumerable<Script>)scripts);
        }

        public ScriptClass GetScript<ScriptClass>() where ScriptClass : Script
        {
            TryGetScript(out ScriptClass found);
            return found;
        }

        public bool TryGetScript<ScriptClass>(out ScriptClass script) where ScriptClass : Script
        {
            script = null;

            foreach (Script current in scripts)
            {
                if (current is ScriptClass match)
                {
                    script = match;
                    return true;
                }
            }

            return false;
        }

        public bool ContainsScript(Script script)
        {
            if (scripts.TrueForAll(each => each != script))
                return false;
            return true;
        }
    }
}
