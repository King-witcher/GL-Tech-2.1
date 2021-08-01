using GLTech2;
using GLTech2.Behaviours;
using GLTech2.Elements;

namespace Test
{
    partial class Program
    {
        static void AnimatedExample()
        {
            // Firstly, load buffers
            using PixelBuffer background_buffer = (PixelBuffer)DemoTextures.HellSky;
            using PixelBuffer carvedWall_buffer = (PixelBuffer)DemoTextures.CarvedWall;
            using PixelBuffer bricks_buffer = (PixelBuffer)DemoTextures.Bricks;
            using PixelBuffer wood_buffer = (PixelBuffer)DemoTextures.Wood;
            using PixelBuffer grayHexagons_buffer = (PixelBuffer)DemoTextures.GrayHexagons;

            // Scene
            Texture background = new Texture(
                buffer: background_buffer,
                hoffset: 0f,
                hrepeat: 1f);

            using Scene scene = new Scene(background);

            // Pivot
            {
                Empty pivot = new Empty(x: 0, y: 0.2868f);
                pivot.AddBehaviour(new Rotate { AngularSpeed = -20f });

                // Square
                {
                    Texture tex = new Texture(
                        buffer: wood_buffer,
                        hoffset: 0f,
                        hrepeat: 2f);

                    Element e = new RegularPolygon(
                        position: (-0.5f, 0f),
                        vertices: 4,
                        radius: .354f,
                        texture: tex);

                    e.AddBehaviour(new Rotate { AngularSpeed = 180f });
                    e.Parent = pivot;
                }

                // Cylinder
                {
                    Texture tex = new Texture(
                        buffer: bricks_buffer,
                        hoffset: 0f,
                        hrepeat: 4f);

                    Element e = new RegularPolygon(
                        position: (0.5f, 0f),
                        vertices: 100,
                        radius: .318f,
                        texture: tex);

                    e.AddBehaviour(new Rotate { AngularSpeed = 180f });
                    e.Parent = pivot;
                }

                // Triangle
                {
                    Texture tex = new Texture(
                        buffer: carvedWall_buffer,
                        hoffset: 0f,
                        hrepeat: 1f);

                    Element e = new RegularPolygon(
                        position: (0f, 0.866f),
                        vertices: 3,
                        radius: .385f,
                        texture: tex);

                    e.AddBehaviour(new Rotate { AngularSpeed = 180f });
                    e.Parent = pivot;
                }

                scene.AddElement(pivot);
            }

            // Big triangle
            {
                Texture tex = new Texture(
                    buffer: grayHexagons_buffer,
                    hoffset: 0f,
                    hrepeat: 32f);

                Element e = new RegularPolygon(
                    position: Vector.Zero,
                    vertices: 4,
                    radius: -2f, 
                    texture: tex);

                scene.AddElement(e);
            }

            // Observer
            Camera pov = new Camera(position: Vector.Backward, rotation: 0f);
            {

                // pov.AddBehaviour<DebugPosition>();
                pov.AddBehaviour<DebugPerformanceStats>();
                pov.AddBehaviour(new MouseRotation(2.2f));

                scene.AddElement(pov);
            }

            // Setup Renderer
            Renderer.FullScreen = true;
            Renderer.FieldOfView = 110f;
            Renderer.ParallelRendering = true;
            Renderer.DoubleBuffer = false;
            Renderer.CaptureMouse = true;

            // Run!
            Renderer.Start(pov);
        }
    }
}
