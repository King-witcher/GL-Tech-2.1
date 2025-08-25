﻿using System;
using Struct = Engine.Structs.CameraStruct;

namespace Engine.World
{
    public unsafe sealed class Camera : Entity, IDisposable
    {
        #region What should happen to the unmanaged data if its position/direction changes? Here's where the class answers it.
        internal Struct* unmanaged;

        private protected override Vector PositionData
        {
            get => unmanaged->position;
            set => unmanaged->position = value;
        }

        private protected override Vector DirectionData
        {
            get => new Vector(unmanaged->rotation);
            set => unmanaged->rotation = value.Angle;
        }
        #endregion

        public float Z
        {
            get => unmanaged->z;
            set => unmanaged->z = value;
        }

        internal Camera()
        {
            unmanaged = Struct.Create(Vector.Zero, 0f);
        }

        public override void Dispose()
        {
            Struct.Delete(unmanaged);
            unmanaged = null;
        }
    }
}
