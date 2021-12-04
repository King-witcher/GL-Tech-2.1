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

        // Scene should be told when a new event enters an entity so that it can update it's script caches.
        internal event Action<Script> OnAddScript;
        internal event Action<Script> OnRemoveScript;

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
            #region Ensures that the entry is not null and was not already added to any entity.
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
                    message: $"Cannot behaviour \"{script}\" to \"{this}\" becuase it's already bound to \"{script.Entity}\".",
                    debugOption: Debug.Options.Error);
                return;
            }
            #endregion

            script.Assign(this);
            this.scripts.Add(script);
            OnAddScript?.Invoke(script);
        }

        public void AddScript<BehaviourType>() where BehaviourType : Script, new()
        {
            AddScript(new BehaviourType());
        }

        public void AddBehaviours(IEnumerable<Script> scripts)
        {
            foreach (Script item in scripts)
                AddScript(item);
        }

        public void AddBehaviours(params Script[] scripts)
        {
            AddBehaviours((IEnumerable<Script>)scripts);
        }

        public ScriptType GetBehaviour<ScriptType>() where ScriptType : Script
        {
            TryGetBehaviour(out ScriptType found);
            return found;
        }

        public bool TryGetBehaviour<ScriptType>(out ScriptType script) where ScriptType : Script
        {
            script = null;

            foreach (Script current in scripts)
            {
                if (current is ScriptType match)
                {
                    script = match;
                    return true;
                }
            }

            return false;
        }

        public bool ContainsScript(Script b)
        {
            if (scripts.TrueForAll(each => each != b))
                return false;
            return true;
        }
    }
}
