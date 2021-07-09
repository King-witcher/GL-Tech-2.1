using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace GLTech2
{
    partial class Element
    {
        internal Action onMove;
        internal List<Element> childs = new List<Element>();
        private Element parent;
        private Vector relativePosition;
        private Vector relativeDirection;

        /// <summary>
        /// Gets and sets the absolute position of an Element without and allows subclasses to store the Position the way they want.
        /// </summary>
        /// <remarks>
        /// Important: Elements that take this element as reference point will not follow it imediatelly for performance and code health reasons. Changing this property is only recommended if the element is not a reference point to any other and changing positions is a significant performance bottleneck in your application. Otherwise, always use Element.Position property instead.
        /// </remarks>
        public Vector WorldPosition
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => PositionData;
            set
            {
                PositionData = value;
                UpdateRelative();
                onMove?.Invoke();
            }
        }

        /// <summary>
        /// Determines how the element stores its direction.
        /// </summary>
        /// <remarks>
        /// Remember to set it before parenting any object!
        /// </remarks>
        public Vector WorldDirection
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => DirectionData;
            set
            {
                DirectionData = value;
                UpdateRelative();
                onMove?.Invoke();
            }
        }

        /// <summary>
        /// The scale of this BlockMap. Cannot be zero.
        /// </summary>
        public float WorldScale
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => WorldDirection.Module;
            set
            {
                if (value != 0)
                {
                    WorldDirection = WorldDirection * value / WorldDirection.Module;
                    UpdateRelative();
                    onMove?.Invoke();
                }
            }
        }

        /// <summary>
        /// Gets and sets element's position relatively to it's parent or, if it has no parent, it's absolute position. 
        /// </summary>
        public Vector RelativePosition
        {
            get
            {
                if (parent == null)
                    return PositionData;
                else
                    return relativePosition;
            }
            set
            {
                relativePosition = value;
                FollowParent();
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
        public Vector RelativeDirection
        {
            get
            {
                if (parent is null)
                    return DirectionData;
                else
                    return relativeDirection;
            }
            set
            {
                relativeDirection = value;
                FollowParent();
            }
        }

        /// <summary>
        /// Gets and sets directly the element's rotation relative to it's parent or, if it has no parents, it's absolute rotation.
        /// </summary>
        public float RelativeRotation
        {
            get
            {
                if (parent is null)
                    return DirectionData.Angle;
                else
                    return relativeDirection.Angle;
            }
            set
            {
                if (parent is null)
                {
                    Vector newNormal = DirectionData;
                    newNormal.Angle = value;
                    DirectionData = newNormal;
                    onMove?.Invoke();
                }
                else
                {
                    relativeDirection.Angle = value;
                    FollowParent();
                }
            }
        }

        /// <summary>
        /// Gets and sets element's reference point. A null value means that the element will take as reference point the world.
        /// </summary>
        /// <remarks>
        /// Setting a reference point will make the object to move and rotate relatively to it's reference point, and if the parent element moves/rotate, this element will follow.
        /// </remarks>
        public Element Parent
        {
            get => parent;
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
                if (parent != null)
                {
                    parent.onMove -= FollowParent;    // Temos um erro aqui
                    parent.childs.Remove(this);
                }

                // If it must have a new element as reference point, then
                if (value != null)
                {
                    // Subscribe to its OnChangeComponents so that you can follow the object whenever it changes position.
                    value.onMove += FollowParent;

                    // Add itself to the parent's child list.
                    value.childs.Add(this);
                }
                this.parent = value;

                // Lastly, update your relative components to match the new reference point.
                UpdateRelative();
            }
        }

        /// <summary>
        /// Gets the root reference point for this object.
        /// </summary>
        public Element RootParent
        {
            get
            {
                Element current = this;
                while (current.parent != null)
                    current = current.parent;
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
            if (parent == null)
            {
                relativePosition = PositionData;
                relativeDirection = DirectionData;
            }
            // Otherwise, in case the reference point is another element:
            else
            {
                relativePosition = PositionData.Projection(parent.PositionData, parent.DirectionData);
                relativeDirection = DirectionData / parent.DirectionData;
            }
        }

        // Update the real components of the element in the scene based on its reference point and its components
        // relative to the parent.
        // This method is called always when either the reference element or this element tries to change it's position.
        private void FollowParent()
        {
            // In case the reference point is the scene origin:
            if (parent == null)
            {
                PositionData = relativePosition;
                DirectionData = relativeDirection;
            }
            // Otherwise, in case the reference point is another element:
            else
            {
                PositionData = relativePosition.Disprojection(parent.PositionData, parent.DirectionData);
                DirectionData = relativeDirection * parent.DirectionData;
            }
            onMove?.Invoke();
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
                child.Parent = null;
                childs.Remove(child);
            }
        }
    }
}
