using System;
using System.Collections.Generic;

// Colocar simulação física (velocity/collision) dentro de Element?
// Velocity deveria ser implementado em Element ou pode ser abstrato e implementado nas classes base?
// Talvez as World devam atualizar as relativas tambem. Posso criar uma que atualize apenas a interna e usar por quest'oes de performance.

namespace GLTech2
{
    partial class Entity
    {
        internal Action OnStart;
        internal Action OnFrame;
        private List<Behaviour> behaviours = new List<Behaviour>();

        internal IEnumerable<Behaviour> Behaviours => behaviours;

        // Scene should be told when a new event enters an entity so that it can update it's behaviour caches.
        internal event Action<Behaviour> OnAddBehaviour;

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

            Subscribe(behaviour); // Possibly unecessary.
        }

        public void AddBehaviour<BehaviourType>() where BehaviourType : Behaviour, new()
        {
            AddBehaviour(new BehaviourType());
        }

        public void AddBehaviours(IEnumerable<Behaviour> behaviours)
        {
            foreach (Behaviour item in behaviours)
            {
                if (item is null)
                    continue;

                AddBehaviour(item);
            }
        }

        public void AddBehaviours(params Behaviour[] behaviours)
        {
            AddBehaviours((IEnumerable<Behaviour>)behaviours);
        }

        public bool ContainsBehaviour<BehaviourType>() where BehaviourType : Behaviour
        {
            foreach (var behaviour in behaviours)
                if (behaviour is BehaviourType)
                    return true;
            return false;
        }

        public bool ContainsBehaviour(Behaviour b)
        {
            foreach (var item in behaviours)
                if (item == b)
                    return true;
            return false;
        }

        public void RemoveBehaviour(Behaviour b)
        {
            if (scene != null)
            {
                Debug.InternalLog(
                    $"Cannot remove behaviour \"{b}\" from element \"{this}\" because it's already bound to a scene.",
                    Debug.Options.Error);
                return;
            }

            behaviours.Remove(b);
            Unsubscribe(b);
        }

        public void RemoveBehaviour<BehaviourType>() where BehaviourType : Behaviour
        {
            foreach (var behaviour in behaviours.ToArray()) // Provisional solution
                if (behaviour is BehaviourType)
                    RemoveBehaviour(behaviour);
        }

        public void RemoveAllBehaviours()
        {
            if (scene != null)
            {
                Debug.InternalLog(
                    $"Cannot remove behaviours from element \"{this}\" because it already was bound to a scene.",
                    Debug.Options.Error);
                return;
            }

            foreach (Behaviour b in behaviours)
                RemoveBehaviour(b);
        }

        public void DetachChildren() // Not tested
        {
            foreach (Entity child in childs)
                child.Parent = null;
        }

        //Subscribe and unsubscribe a behaviour
        private void Subscribe(Behaviour b)
        {
            OnStart += b.StartAction;
            OnFrame += b.OnFrameAction;
        }

        private void Unsubscribe(Behaviour b)
        {
            OnStart -= b.StartAction;
            OnFrame -= b.OnFrameAction;
        }
    }
}
