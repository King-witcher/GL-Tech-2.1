using System;
using System.Collections.Generic;

// Colocar simulação física (velocity/collision) dentro de Element?
// Velocity deveria ser implementado em Element ou pode ser abstrato e implementado nas classes base?
// Talvez as World devam atualizar as relativas tambem. Posso criar uma que atualize apenas a interna e usar por quest'oes de performance.

namespace GLTech2
{
    partial class Element
    {
        internal Action OnStart;
        internal Action OnFrame;
        private List<Behaviour> behaviours = new List<Behaviour>();

        /// <summary>
        /// Adds a behaviour script to the element by instance.
        /// </summary>
        /// <param name="behaviour">Instance of Behaviour to be added</param>
        /// <remarks>
        /// I made id just a copy of how MonoBehaviours works in Unity3D =D
        /// </remarks>
        public void AddBehaviour(Behaviour behaviour)
        {
            if (behaviour is null)
                return;

            if (ContainsBehaviour(behaviour))
            {
                Debug.InternalLog(
                    origin: "Element",
                    message: $"Cannot add same behaviour twice. {typeof(Behaviour).Name} second instance will be ignored.",
                    debugOption: Debug.Options.Warning);
                return;
            }

            if (behaviour.element is null is false)
            {
                Debug.InternalLog(
                    origin: "Element", 
                    message: $"Cannot add the same instance of behaviour in two different elements. Element without behaviour: {this}.",
                    debugOption: Debug.Options.Error);
                return;
            }

            behaviours.Add(behaviour); // Isso pode estar obsoleto, visto que agora os métodos a serem chamados são salvos em eventos.
            behaviour.element = this;

            Subscribe(behaviour);
        }

        /// <summary>
        /// Add a new behaviour to the element by type.
        /// </summary>
        /// <typeparam name="BehaviourType">Behaviour type</typeparam>
        public void AddBehaviour<BehaviourType>() where BehaviourType : Behaviour, new()
        {
            AddBehaviour(new BehaviourType());
        }

        /// <summary>
        /// Adds a set of behaviours.
        /// </summary>
        /// <param name="behaviours">Set of behaviours</param>
        public void AddBehaviours(IEnumerable<Behaviour> behaviours)
        {
            foreach (Behaviour item in behaviours)
            {
                if (item is null)
                    continue;

                AddBehaviour(item);
            }
        }

        /// <summary>
        /// Adds a set of behaviours.
        /// </summary>
        /// <param name="behaviours">Array of behaviours</param>
        public void AddBehaviours(params Behaviour[] behaviours)
        {
            AddBehaviours((IEnumerable<Behaviour>) behaviours);
        }

        /// <summary>
        /// Checks if the element contains an especific behaviour by type.
        /// </summary>
        /// <typeparam name="BehaviourType">Behaviour type</typeparam>
        /// <returns>true if it contains; otherwise, false</returns>
        public bool ContainsBehaviour<BehaviourType>() where BehaviourType : Behaviour
        {
            foreach (var behaviour in behaviours)
                if (behaviour is BehaviourType)
                    return true;
            return false;
        }

        /// <summary>
        /// Checks if the element contains an especific behaviour by instance.
        /// </summary>
        /// <param name="b">Behaviour</param>
        /// <returns>true if it contains; otherwise, false</returns>
        public bool ContainsBehaviour(Behaviour b)
        {
            foreach (var item in behaviours)
                if (item == b)
                    return true;
            return false;
        }

        /// <summary>
        /// Removes an instance of behaviour from b.
        /// </summary>
        /// <param name="b">Behaviour</param>
        public void RemoveBehaviour(Behaviour b)
        {
            behaviours.Remove(b);
            Unsubscribe(b);
        }

        /// <summary>
        /// Removes all instances of Behaviour from a given type from the element.
        /// </summary>
        /// <typeparam name="BehaviourType"></typeparam>
        public void RemoveBehaviour<BehaviourType>() where BehaviourType : Behaviour
        {
           foreach (var behaviour in behaviours.ToArray()) // Provisional solution
                if (behaviour is BehaviourType)
                    RemoveBehaviour(behaviour);
        }

        /// <summary>
        /// Remove all behaviours from the element.
        /// </summary>
        public void RemoveAllBehaviours()
        {
            foreach(Behaviour b in behaviours)
            {
                RemoveBehaviour(b);
            }
        }

        /// <summary>
        /// Detach all children from the element.
        /// </summary>
        public void DetachChildren() // Not tested
        {
            foreach (Element child in childs)
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
