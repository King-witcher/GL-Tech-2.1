using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Engine.World
{
    partial class Entity
    {
        private static Logger logger = new(typeof(Entity).Name);
        private Vector relativePosition;
        private Vector relativeDirection;
        private List<Entity> children = new();
        private Entity parent;
        private Action onMove;

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
                    logger.Error($"The entity \"{value}\" cannot be set parent of \"{this}\" because they are bound to different scenes.");
                    return;
                }

                // If it has a previous parent, unparent it first.
                if (parent != null)
                {
                    parent.onMove -= FollowParent;    // Temos um erro aqui
                    parent.children.Remove(this);       // Spaguetti?
                }

                // If it must have a new entity as parent, then
                if (value != null)
                {
                    // Subscribe to its OnChangeComponents so that you can follow the object whenever it changes position.
                    value.onMove += FollowParent;

                    // Add itself to the parent's child list.
                    value.children.Add(this);
                }
                this.parent = value;

                // Lastly, update your relative components to match the new reference point.
                UpdateRelative();
            }
        }

        // Internal because i'm angry that the user can just use Children as List<Entity> and mess up everything.
        internal IEnumerable<Entity> Children => children;

        public int ChildCount => children.Count;

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

        // Update relative position/normal info based on parent and absolute components.
        // Must be called when which parent element changes.
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
            return children[index];
        }

        public void DetachChildren()
        {
            foreach (Entity child in children)
                child.Parent = null;
        }
    }
}
