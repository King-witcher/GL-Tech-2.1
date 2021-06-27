using System;
using System.Collections.Generic;

namespace GLTech2
{
    /// <summary>
    /// Represents an element that can be part of a Scene.
    /// </summary>
    public abstract class Element : IDisposable
    {
        /// <summary>
        /// Private protected. Determines how the element stores its position.
        /// </summary>
        /// <remarks>
        /// Remember to set it before parenting any object!
        /// </remarks>
        public abstract Vector AbsolutePosition { get; set; }

        /// <summary>
        /// Private protected. Determines how the element stores its normal.
        /// </summary>
        /// <remarks>
        /// Remember to set it before parenting any object!
        /// </remarks>
        public abstract Vector AbsoluteNormal { get; set; } //Provides rotation and scale of the object.

        public Element() { }

        internal Action StartAction;
        internal Action OnFrameAction;
        private Element referencePoint = null;
        private List<Behaviour> behaviours = new List<Behaviour>();
        private Vector relativePosition;
        private Vector relativeNormal;
        internal Scene scene;
        internal List<Element> childs = new List<Element>();

        /// <summary>
        /// Its correspondent scene. If the element is not bound to any scene, returns null.
        /// </summary>
        public Scene Scene => scene; // Maybe not necessary.

        /// <summary>
        /// How many childs the element has.
        /// </summary>
        public int ChildCount => childs.Count;

        internal event Action OnChangeComponents;

        /// <summary>
        /// Gets and sets element's position relatively to it's parent or, if it has no parent, it's absolute position. 
        /// </summary>
        public Vector Position
        {
            get
            {
                if (referencePoint is null)
                    return AbsolutePosition;
                else
                    return relativePosition;
            }
            set
            {
                if (referencePoint is null)
                {
                    AbsolutePosition = value;
                    OnChangeComponents?.Invoke();
                }
                else
                {
                    relativePosition = value;
                    UpdateAbsolute();
                }
            }
        }

        /// <summary>
        /// Gets and sets element's normal relatively to it's parent or, if it has no parent, it's absolute normal. 
        /// </summary>
        /// <remarks>
        /// Normal vector determines the rotation and the scale of an object and is used due to performance improvements when managing multiple childs.
        ///     <para>
        ///     Use wisely.
        ///     </para>
        /// </remarks>
        public Vector Normal
        {
            get
            {
                if (referencePoint is null)
                    return AbsoluteNormal;
                else
                    return relativeNormal;
            }
            set
            {
                if (referencePoint is null)
                {
                    AbsoluteNormal = value;
                    OnChangeComponents?.Invoke();
                }
                else
                {
                    relativeNormal = value;
                    UpdateAbsolute();
                }
            }
        }

        /// <summary>
        /// Gets and sets directly the element's rotation relative to it's parent or, if it has no parents, it's absolute rotation.
        /// </summary>
        public float Rotation
        {
            get
            {
                if (referencePoint is null)
                    return AbsoluteNormal.Angle;
                else
                    return relativeNormal.Angle;
            }
            set
            {
                if (referencePoint is null)
                {
                    Vector newNormal = AbsoluteNormal;
                    newNormal.Angle = value;
                    AbsoluteNormal = newNormal;
                    OnChangeComponents?.Invoke();
                }
                else
                {
                    relativeNormal.Angle = value;
                    UpdateAbsolute();
                }
            }
        }

        /// <summary>
        /// Gets and sets element's reference point. A null value means that the element will take as reference point the scene.
        /// </summary>
        /// <remarks>
        /// Setting a reference point will make the object to move and rotate relatively to it's reference point, and if the parent element moves/rotate, this element will follow.
        /// </remarks>
        public Element ReferencePoint
        {
            get => referencePoint;
            set
            {
                // Check if the scenes are compatible. Elements cannot take as reference point others that are in differente scnees.
                if (value != null && scene != value.scene)
                {
                    Debug.InternalLog(
                        origin: "Element",
                        message: $"Cannot parent {this} to an element that is in other scene. Operation aborted.",
                        debugOption: Debug.Options.Error);
                    return;
                }

                // If it has a previous parent, unparent it first.
                if (referencePoint != null)
                {
                    referencePoint.OnChangeComponents -= UpdateAbsolute;
                    referencePoint.childs.Remove(this);
                }

                // If it must have a new element as reference point, then
                if (value != null)
                {
                    // Subscribe to its OnChangeComponents so that you can follow the object whenever it changes position.
                    value.OnChangeComponents += UpdateAbsolute;
                    // Add itself to the parent's child list.
                    value.childs.Add(this);
                }
                this.referencePoint = value;

                // Lastly, update your relative components to match the new reference point.
                UpdateRelative();
            }
        }

        // Update relative position/normal info based on parent and absolute components.
        // Must be called when which parent element this element takes as reference point changes.
        private void UpdateRelative()
        {
            // In case the reference point is the scene origin:
            if (referencePoint is null)
            {
                relativePosition = AbsolutePosition;
                relativeNormal = AbsoluteNormal;
            }
            // Otherwise, in case the reference point is another element:
            else
            {
                relativePosition = AbsolutePosition.Projection(referencePoint.AbsolutePosition, referencePoint.AbsoluteNormal);
                relativeNormal = AbsoluteNormal / referencePoint.AbsoluteNormal;
            }
        }

        // Update the real components of the element in the scene based on its reference point and its components
        // relative to the reference.
        // This method is called always when either the reference element or this element tries to change it's position.
        private void UpdateAbsolute()
        {
            // In case the reference point is the scene origin:
            if (referencePoint is null)
            {
                AbsolutePosition = relativePosition;
                AbsoluteNormal = relativeNormal;
            }
            // Otherwise, in case the reference point is another element:
            else
            {
                AbsolutePosition = relativePosition.AsProjectionOf(referencePoint.AbsolutePosition, referencePoint.AbsoluteNormal);
                AbsoluteNormal = relativeNormal * referencePoint.AbsoluteNormal;
            }
            // Then, publish to all children elements that its position has changed so that they can follow you with their respective UpdateAbsolute() methods.
            OnChangeComponents?.Invoke();
        }

        /// <summary>
        /// Moves the object in one direction relatively to it's direction; in other words, the direction of the module vector.
        /// </summary>
        /// <param name="direction">Direction to move</param>
        public void Translate(Vector direction)
        {
            Position += direction * Normal;
        }

        /// <summary>
        /// Rotate the object a specified amount.
        /// </summary>
        /// <param name="rotation">angle in degrees</param>
        public void Rotate(float rotation)
        {
            Rotation += rotation;
        }

        /// <summary>
        /// Dettach all childs and make their reference point equal to null.
        /// </summary>
        /// <remarks>
        /// Not widely tested.
        /// </remarks>
        public void DetachChilds()
        {
            foreach (Element child in childs)
            {
                child.ReferencePoint = null;
                childs.Remove(child);
            }
        }

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
                child.ReferencePoint = null;
        }

        /// <summary>
        /// Gets a child from the element by index.
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Specified children</returns>
        public Element GetChild(int index)
        {
            return childs[index];
        }

        /// <summary>
        /// Releases unmanaged data, if any.
        /// </summary>
        public virtual void Dispose()
        {

        }

        //Subscribe and unsubscribe a behaviour
        private void Subscribe(Behaviour b)
        {
            StartAction += b.StartAction;
            OnFrameAction += b.OnFrameAction;
        }

        private void Unsubscribe(Behaviour b)
        {
            StartAction -= b.StartAction;
            OnFrameAction -= b.OnFrameAction;
        }
    }
}
