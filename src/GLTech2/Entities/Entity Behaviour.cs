using System;
using System.Collections.Generic;
using GLTech2.Scripting;

// Talvez as World devam atualizar as relativas tambem. Posso criar uma que atualize apenas a interna e usar por quest'oes de performance.

namespace GLTech2.Entities
{
    partial class Entity
    {
        private List<Behaviour> behaviours = new List<Behaviour>();

        internal IEnumerable<Behaviour> Behaviours => behaviours;

        // Scene should be told when a new event enters an entity so that it can update it's behaviour caches.
        internal event Action<Behaviour> OnAddBehaviour;
        internal event Action<Behaviour> OnRemoveBehaviour;

        // Just a sintax sugar
        internal Action OnStart
        {
            get
            {
                Action action = null;
                foreach (Behaviour behaviour in behaviours)
                    action += behaviour.StartAction;
                return action;
            }
        }

        internal Action OnFrame
        {
            get
            {
                Action action = null;
                foreach (Behaviour behaviour in behaviours)
                    action += behaviour.OnFrameAction;
                return action;
            }
        }

        public void AddBehaviour(Behaviour behaviour)
        {
            #region Ensures that the entry is not null and was not already added to any entity.
            if (behaviour is null)
            {
                Debug.InternalLog(
                    message: $"Cannot add a null behaviour to entity \"{this}\".",
                    debugOption: Debug.Options.Warning);
                return;
            }

            if (ContainsBehaviour(behaviour))
            {
                Debug.InternalLog(
                    message: $"Cannot add behaviour \"{behaviour}\" twice to the same entity \"{this}\".",
                    debugOption: Debug.Options.Warning);
                return;
            }

            if (behaviour.Entity != null)
            {
                Debug.InternalLog(
                    message: $"Cannot behaviour \"{behaviour}\" to \"{this}\" becuase it's already bound to \"{behaviour.Entity}\".",
                    debugOption: Debug.Options.Error);
                return;
            }
            #endregion

            behaviour.Assign(this);
            this.behaviours.Add(behaviour);
            OnAddBehaviour?.Invoke(behaviour);
        }

        public void AddBehaviour<BehaviourType>() where BehaviourType : Behaviour, new()
        {
            AddBehaviour(new BehaviourType());
        }

        public void AddBehaviours(IEnumerable<Behaviour> behaviours)
        {
            foreach (Behaviour item in behaviours)
                AddBehaviour(item);
        }

        public void AddBehaviours(params Behaviour[] behaviours)
        {
            AddBehaviours((IEnumerable<Behaviour>)behaviours);
        }

        public bool ContainsBehaviour<BehaviourType>() where BehaviourType : Behaviour
        {
            if (behaviours.TrueForAll((each) => !(each is BehaviourType)))
                return false;
            return true;
        }

        public bool ContainsBehaviour(Behaviour b)
        {
            if (behaviours.TrueForAll((each) => each != b))
                return false;
            return true;
        }
    }
}
