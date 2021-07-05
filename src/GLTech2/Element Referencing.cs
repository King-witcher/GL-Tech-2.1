using System;
using System.Collections.Generic;

namespace GLTech2
{
    partial class Element
    {
        internal Action OnChangeComponents;
        internal List<Element> childs = new List<Element>();
        private Element referencePoint;
        private Vector relativePosition;
        private Vector relativeNormal;

        /// <summary>
        /// Gets and sets the absolute position of an Element without and allows subclasses to store the Position the way they want.
        /// </summary>
        /// <remarks>
        /// Important: Elements that take this element as reference point will not follow it imediatelly for performance and code health reasons. Changing this property is only recommended if the element is not a reference point to any other and changing positions is a significant performance bottleneck in your application. Otherwise, always use Element.Position property instead.
        /// </remarks>
        public Vector WorldPosition     // Fase de testes!
        {
            get => PositionData;
            set
            {
                PositionData = value;
                UpdateRelative();
                OnChangeComponents?.Invoke();
            }
        }

        /// <summary>
        /// Determines how the element stores its direction.
        /// </summary>
        /// <remarks>
        /// Remember to set it before parenting any object!
        /// </remarks>
        public Vector WorldDirection    // Fase de testes!
        {
            get => DirectionData;
            set
            {
                DirectionData = value;
                UpdateRelative();
                OnChangeComponents?.Invoke();
            }
        }

        /// <summary>
        /// Gets and sets element's position relatively to it's parent or, if it has no parent, it's absolute position. 
        /// </summary>
        public Vector Translation
        {
            get
            {
                if (referencePoint is null)
                    return PositionData;
                else
                    return relativePosition;
            }
            set
            {
                if (referencePoint is null)
                {
                    PositionData = value;
                    OnChangeComponents?.Invoke();
                }
                else
                {
                    relativePosition = value;
                    UpdateAbsolute();
                    OnChangeComponents?.Invoke();
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
        public Vector Rotation
        {
            get
            {
                if (referencePoint is null)
                    return DirectionData;
                else
                    return relativeNormal;
            }
            set
            {
                if (referencePoint is null)
                {
                    DirectionData = value;
                    OnChangeComponents?.Invoke();
                }
                else
                {
                    relativeNormal = value;
                    UpdateAbsolute();
                    OnChangeComponents?.Invoke();
                }
            }
        }

        /// <summary>
        /// Gets and sets directly the element's rotation relative to it's parent or, if it has no parents, it's absolute rotation.
        /// </summary>
        public float Angle
        {
            get
            {
                if (referencePoint is null)
                    return DirectionData.Angle;
                else
                    return relativeNormal.Angle;
            }
            set
            {
                if (referencePoint is null)
                {
                    Vector newNormal = DirectionData;
                    newNormal.Angle = value;
                    DirectionData = newNormal;
                    OnChangeComponents?.Invoke();
                }
                else
                {
                    relativeNormal.Angle = value;
                    UpdateAbsolute();
                    OnChangeComponents?.Invoke();
                }
            }
        }

        /// <summary>
        /// Gets and sets element's reference point. A null value means that the element will take as reference point the world.
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

        /// <summary>
        /// Gets the root reference point for this object.
        /// </summary>
        public Element RootReferencePoint
        {
            get
            {
                Element current = this;
                while (current.referencePoint != null)
                    current = current.referencePoint;
                return current;
            }
        }

        /// <summary>
        /// How many childs the element has.
        /// </summary>
        public int ChildCount => childs.Count;

        // Update relative position/normal info based on parent and absolute components.
        // Must be called when which parent element this element takes as reference point changes.
        private void UpdateRelative()
        {
            // In case the reference point is the scene origin:
            if (referencePoint is null)
            {
                relativePosition = PositionData;
                relativeNormal = DirectionData;
            }
            // Otherwise, in case the reference point is another element:
            else
            {
                relativePosition = PositionData.Projection(referencePoint.PositionData, referencePoint.DirectionData);
                relativeNormal = DirectionData / referencePoint.DirectionData;
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
                PositionData = relativePosition;
                DirectionData = relativeNormal;
            }
            // Otherwise, in case the reference point is another element:
            else
            {
                PositionData = relativePosition.Disprojection(referencePoint.PositionData, referencePoint.DirectionData);
                DirectionData = relativeNormal * referencePoint.DirectionData;
            }
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
        /// Release all childs and make their reference point equal to null.
        /// </summary>
        /// <remarks>
        /// Not widely tested.
        /// </remarks>
        public void ReleaseChilds()
        {
            foreach (Element child in childs)
            {
                child.ReferencePoint = null;
                childs.Remove(child);
            }
        }
    }
}
