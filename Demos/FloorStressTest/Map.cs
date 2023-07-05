﻿using Engine;
using Engine.Imaging;
using Engine.World;
using Engine.World.Composed;
using Engine.Scripting.Debugging;
using Engine.Scripting.Prefab;
using Engine.Scripting.Physics;
using Engine.Demos;

using System.Collections.Generic;

namespace Engine.Demos.FloorStressTest
{
    // Um pequeno mapa usando BlockMap
    public class Map : Scene
    {
        Image floors_buffer = new(Resources.golden);

        protected override void Delete()
        {
            floors_buffer.Dispose();
        }

        public Map()
        {
            Background = Texture.FromColor(Color.Black, out _);

            // Floors
            {
                var texture = new Texture(floors_buffer);
                const int size = 3;
                for (float i = 0; i < size; i++)
                {
                    for (float j = 0; j < size; j++)
                    {
                        Floor f = new Floor((i, j), (i + 1, j + 1), texture);
                        Ceiling c = new Ceiling((i, j), (i + 1, j + 1), texture);
                        Add(f, c);
                    }
                }
            }

            // Wall
            {
                var texture = new Texture(floors_buffer);
                var wall = new Wall((1, 2), (2, 2), texture);
                Add(wall);
            }

            // Camera
            {
                Camera camera = Camera;

                //camera.AddScript<DebugPerformanceStats>();
                //camera.AddScript<DebugSceneInfo>();
                //camera.AddScript<DebugComponents>();

                PointCollider collider = new PointCollider();
                SoftMovement movement = new SoftMovement(collider);
                MouseLook mouseLook = new MouseLook(2.2f);

                camera.AddScript(collider);
                camera.AddScript(movement);
                camera.AddScript(mouseLook);

                Add(camera);
            }
        }
    }
}