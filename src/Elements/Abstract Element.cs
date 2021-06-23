using System;
using System.Collections.Generic;
using System.Reflection;

namespace GLTech2
{
    /// <summary>
    ///     Represents an element that can be part of a Scene.
    /// </summary>
    public abstract class Element : IDisposable
    {
        // Every element MUST call UpdateRelative() after construction. I have to fix it yet.
        private protected Element() { }

        private Element parent = null;
        private List<Behaviour> behaviours = new List<Behaviour>();
        private Vector relativePosition;
        private Vector relativeNormal;
        internal Scene scene;
        internal List<Element> childs = new List<Element>();

        /// <summary>
        ///     Its correspondent scene. If the element is not bound to any scene, returns null.
        /// </summary>
        public Scene Scene => scene; // Maybe not necessary.

        /// <summary>
        ///     How many childs the element has.
        /// </summary>
        public int ChildCount => childs.Count;


        private protected abstract Vector AbsolutePosition { get; set; }
        private protected abstract Vector AbsoluteNormal { get; set; } //Provides rotation and scale of the object.


        internal event Action OnMoveOrRotate;


        /// <summary>
        ///     Gets and sets element's position relatively to it's parent or, if it has no parent, it's absolute position. 
        /// </summary>
        public Vector Position
        {
            get
            {
                if (parent is null)
                    return AbsolutePosition;
                else
                    return relativePosition;
            }
            set
            {
                if (parent is null)
                {
                    AbsolutePosition = value;
                    OnMoveOrRotate?.Invoke();
                }
                else
                {
                    relativePosition = value;
                    UpdateAbsolute();
                }
            }
        }

        /// <summary>
        ///     Gets and sets element's normal relatively to it's parent or, if it has no parent, it's absolute normal. 
        /// </summary>
        /// <remarks>
        ///     Normal vector determines the rotation and the scale of an object and is used due to performance improvements when managing multiple childs.
        ///     <para>
        ///         Use wisely.
        ///     </para>
        /// </remarks>
        public Vector Normal
        {
            get
            {
                if (parent is null)
                    return AbsoluteNormal;
                else
                    return relativeNormal;
            }
            set
            {
                if (parent is null)
                {
                    AbsoluteNormal = value;
                    OnMoveOrRotate?.Invoke();
                }
                else
                {
                    relativeNormal = value;
                    UpdateAbsolute();
                }
            }
        }

        /// <summary>
        ///     Gets and sets directly the element's rotation relative to it's parent or, if it has no parents, it's absolute rotation.
        /// </summary>
        public float Rotation
        {
            get
            {
                if (parent is null)
                    return AbsoluteNormal.Angle;
                else
                    return relativeNormal.Angle;
            }
            set
            {
                if (parent is null)
                {
                    Vector newNormal = AbsoluteNormal;
                    newNormal.Angle = value;
                    AbsoluteNormal = newNormal;
                    OnMoveOrRotate?.Invoke();
                }
                else
                {
                    relativeNormal.Angle = value;
                    UpdateAbsolute();
                }
            }
        }

        /// <summary>
        ///     Gets and sets element's parent. Null is equivalent to no parent.
        /// </summary>
        /// <remarks>
        ///     Setting a parent will make the object to move and rotate relatively to it's parent, and if the parent element moves/rotate, then the child follows.
        /// </remarks>
        public Element Parent
        {
            get => parent;
            set
            {
                if (value != null && scene != value.scene)
                {
                    if (Debug.DebugWarnings)
                    {
                        Console.WriteLine($"Cannot parent {this} to an element that is in other scene. Operation aborted.");
                        return;
                    }
                }

                if (parent != null)
                {
                    parent.OnMoveOrRotate -= UpdateAbsolute;
                    parent.childs.Remove(this);
                }

                if (value != null)
                {
                    value.OnMoveOrRotate += UpdateAbsolute;
                    value.childs.Add(this);
                }
                this.parent = value;

                UpdateRelative();
            }
        }

        // Update relative transform through parent and absolute transform.
        // Called when attaches 
        // Must be called after construction of every subclass.
        private void UpdateRelative()
        {
            if (parent is null)
            {
                relativePosition = AbsolutePosition;
                relativeNormal = AbsoluteNormal;
            }
            else
            {
                relativePosition = AbsolutePosition.Projection(parent.AbsolutePosition, parent.AbsoluteNormal);
                relativeNormal = AbsoluteNormal / parent.AbsoluteNormal;
            }
        }

        // Update absolute position through relative position and parent.
        // Called either when the parent or this element changes its position.
        private void UpdateAbsolute()
        {
            if (parent is null)
            {
                AbsolutePosition = relativePosition;
                AbsoluteNormal = relativeNormal;
            }
            else
            {
                AbsolutePosition = relativePosition.AsProjectionOf(parent.AbsolutePosition, parent.AbsoluteNormal);
                AbsoluteNormal = relativeNormal * parent.AbsoluteNormal;
            }
            OnMoveOrRotate?.Invoke();
        }

        /// <summary>
        ///     Moves the object in one direction relatively to it's direction; in other words, the direction of the module vector.
        /// </summary>
        /// <param name="direction">Direction to move</param>
        public void Translate(Vector direction)
        {
            Position += direction * Normal;
        }

        /// <summary>
        ///     Rotate the object a specified amount.
        /// </summary>
        /// <param name="rotation">angle in degrees</param>
        public void Rotate(float rotation)
        {
            Rotation += rotation;
        }

        /// <summary>
        ///     Dettach all childs and make their parents = null.
        /// </summary>
        /// <remarks>
        ///     Not widely tested.
        /// </remarks>
        public void DetachChilds()
        {
            foreach (Element child in childs)
            {
                child.Parent = null;
                childs.Remove(child);
            }
        }

        /// <summary>
        ///     Adds a behaviour script to the element by instance.
        /// </summary>
        /// <param name="behaviour">Instance of Behaviour to be added</param>
        /// <remarks>
        ///     I made id just a copy of how MonoBehaviours works in Unity3D =D
        /// </remarks>
        public void AddBehaviour(Behaviour behaviour)
        {
            if (behaviour is null)
                return;

            if (ContainsBehaviour(behaviour))
            {
                Debug.LogWarning($"Cannot add same behaviour twice. {typeof(Behaviour).Name} second instance will be ignored.");
                return;
            }

            if (behaviour.element is null is false)
            {
                Debug.LogWarning($"Cannot add same behaviour instance to two different elements. Element without behaviour: {this}.");
                return;
            }

            behaviours.Add(behaviour); // Isso pode estar obsoleto, visto que agora os métodos a serem chamados são salvos em eventos.
            behaviour.element = this;

            Subscribe(behaviour);
        }

        /// <summary>
        ///     Add a new behaviour to the element by type.
        /// </summary>
        /// <typeparam name="BehaviourType">Behaviour type</typeparam>
        public void AddBehaviour<BehaviourType>() where BehaviourType : Behaviour, new()
        {
            AddBehaviour(new BehaviourType());
        }

        /// <summary>
        ///     Adds a set of behaviours.
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
        ///     Adds a set of behaviours.
        /// </summary>
        /// <param name="behaviours">Array of behaviours</param>
        public void AddBehaviours(params Behaviour[] behaviours)
        {
            AddBehaviours((IEnumerable<Behaviour>) behaviours);
        }

        /// <summary>
        ///     Checks if the element contains an especific behaviour by type.
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
        ///     Checks if the element contains an especific behaviour by instance.
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
        ///     Removes an instance of behaviour from b.
        /// </summary>
        /// <param name="b">Behaviour</param>
        public void RemoveBehaviour(Behaviour b)
        {
            behaviours.Remove(b);
            Unsubscribe(b);
        }

        /// <summary>
        ///     Removes all instances of Behaviour from a given type from the element.
        /// </summary>
        /// <typeparam name="BehaviourType"></typeparam>
        public void RemoveBehaviour<BehaviourType>() where BehaviourType : Behaviour
        {
           foreach (var behaviour in behaviours.ToArray()) // Provisional solution
                if (behaviour is BehaviourType)
                    RemoveBehaviour(behaviour);
        }

        /// <summary>
        ///     Remove all behaviours from the element.
        /// </summary>
        public void RemoveAllBehaviours()
        {
            foreach(Behaviour b in behaviours)
            {
                RemoveBehaviour(b);
            }
        }

        /// <summary>
        ///     Detach all children from the element.
        /// </summary>
        public void DetachChildren() // Not tested
        {
            foreach (Element child in childs)
                child.Parent = null;
        }

        /// <summary>
        ///     Gets a child from the element by index.
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Specified children</returns>
        public Element GetChild(int index)
        {
            return childs[index];
        }

        internal void InvokeStart()
        {
            StartEvent?.Invoke();
        }

        internal void InvokeUpdate()
        {
            UpdateEvent?.Invoke();
        }

        /// <summary>
        ///     Releases unmanaged data, if any.
        /// </summary>
        public virtual void Dispose()
        {

        }

        //Subscribe and unsubscribe a behaviour
        private void Subscribe(Behaviour b)
        {
            StartEvent += b.StartMethod;
            UpdateEvent += b.UpdateMethod;
        }
        private void Unsubscribe(Behaviour b)
        {
            StartEvent -= b.StartMethod;
            UpdateEvent -= b.UpdateMethod;
        }

        internal event Action StartEvent;
        internal event Action UpdateEvent;
    }
}
