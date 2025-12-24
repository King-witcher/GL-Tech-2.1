using GLTech.Scripting;

// Talvez as World devam atualizar as relativas tambem. Posso criar uma que atualize apenas a interna e usar por quest'oes de performance.

namespace GLTech.World
{
    partial class Entity
    {
        private List<Script> scripts = new List<Script>();

        // Cache for faster execution
        //internal List<Action> starts = new List<Action>();
        //internal List<Action> updates = new List<Action>();
        //internal List<Action> fixedUpdates = new List<Action>();

        internal IEnumerable<Script> Scripts => scripts;

        public void AddScript(Script script)
        {
            #region Ensures that the entry is not null and the script was not added to any entity.
            if (script is null)
            {
                logger.Error($"Cannot add a null script to entity \"{this}\".");
                return;
            }

            if (ContainsScript(script))
            {
                logger.Error($"Cannot add script \"{script}\" twice to the same entity \"{this}\".");
                return;
            }

            if (script.Entity != null)
            {
                logger.Error($"Cannot add script \"{script}\" to \"{this}\" becuase it's already bound to \"{script.Entity}\".");
                return;
            }
            #endregion

            script.Entity = this;
            scripts.Add(script);
            //if (script.StartAction != null)
            //    starts.Add(script.StartAction);
            //if (script.UpdateAction != null)
            //    updates.Add(script.UpdateAction);
            //if (script.FixedUpdateAction != null)
            //    fixedUpdates.Add(script.FixedUpdateAction);

            // Ensures that the script will still work even if added after the element was added to a scene.
            // This is because the renderer runs scripts directly from the Scene's Script cache for performance reasons.
            if (Scene != null)
                Scene.CacheScript(script);
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

        public ScriptClass? GetScript<ScriptClass>() where ScriptClass : Script
        {
            TryGetScript(out ScriptClass? found);
            return found;
        }

        public bool TryGetScript<ScriptClass>(out ScriptClass? script) where ScriptClass : Script
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
