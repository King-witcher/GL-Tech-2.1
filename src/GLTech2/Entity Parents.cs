using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace GLTech2
{
    partial class Entity
    {
        internal Action onMove;
        internal List<Entity> childs = new List<Entity>();
        private Entity parent;
        private Vector relativePosition;
        private Vector relativeDirection;

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

        public Entity Parent
        {
            get => parent;
            set
            {
                // Check if the scenes are compatible. Elements cannot take as reference point others that are in differente scnees.
                if (value != null && scene != value.scene)
                {
                    Debug.InternalLog(
                        message: $"The element \"{value}\" cannot be set parent of \"{this}\" because they are bound to different scenes.",
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

        public Entity RootParent
        {
            get
            {
                Entity current = this;
                while (current.parent != null)
                    current = current.parent;
                return current;
            }
        }

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

        public void Rotate(float rotation)
        {
            RelativeRotation += rotation;
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

        public Entity GetChild(int index)
        {
            return childs[index];
        }

        public void ReleaseChilds()
        {
            foreach (Entity child in childs)
            {
                child.Parent = null;
                childs.Remove(child);
            }
        }
    }
}
