using Engine;
using GLTech;
using GLTech.Scripting;
using GLTech.Scripting.Physics;
using GLTech.Scripting.Prefab;
using GLTech.Space;
using GLTech.World;
using GLTech.World.Composed;
using System.Diagnostics;

namespace wolf3d;

public class FlagBehavior : Script
{
    Logger logger = new(typeof(FlagBehavior).Name);
    bool started = false;
    int nextCheckpoint = 0;
    Stopwatch sw = new Stopwatch();
    Vector[] checkpoints;
    TrajectoryRecorder? trajectoryRecorder;

    public float Radius { get; set; } = 0.707f;

    public FlagBehavior(params Vector[] checkpoints)
    {
        if (checkpoints.Length < 2)
            throw new System.ArgumentException("You must provide at least two checkpoints (start and end).");

        this.checkpoints = checkpoints;
    }

    public void Start()
    {
        Entity.WorldPosition = checkpoints[0];
        Entity? highscore = Scene.FindByname("highscore");
        trajectoryRecorder = highscore?.GetScript<TrajectoryRecorder>();
    }

    public void OnFrame()
    {
        float distance = (Entity.WorldPosition - Scene.Player.WorldPosition).Module;
        bool colliding = distance < Radius;

        if (colliding)
        {
            var time = sw.ElapsedMilliseconds / 1000f;

            if (!started)
            {
                trajectoryRecorder?.StartRecording();
                sw.Restart();
                started = true;
                Entity.WorldPosition = checkpoints[++nextCheckpoint];
                logger.Success("You have started the run! Go to the next checkpoint.");
                return;
            }

            if (nextCheckpoint < checkpoints.Length - 1)
            {
                logger.Success($"Checkpoint #{nextCheckpoint}: {time}s");
                Entity.WorldPosition = checkpoints[++nextCheckpoint];
                return;
            }


            sw.Stop();
            trajectoryRecorder?.FinishRecording();
            logger.Success($"You finished in {time}s!");
            Entity.WorldPosition = checkpoints[0];
            started = false;
            nextCheckpoint = 0;
            return;
        }
    }
}

// Wolfenstein 3D's first level
public class Map : Scene
{
    readonly List<TextureBuffer> buffers = new();

    protected override void Delete()
    {
        foreach (TextureBuffer buffer in buffers)
            buffer.Dispose();
    }

    TextureBuffer LoadBuffer(string path)
    {
        TextureBuffer buffer = ResourceManager.LoadTextureBuffer(path);
        buffers.Add(buffer);
        return buffer;
    }

    Texture LoadTexture(string path) => new(LoadBuffer(path));

    public Map()
    {
        Background = LoadTexture("bg.bmp");

        // BlockMap
        {
            using TextureBuffer blockmap_buffer = ResourceManager.LoadTextureBuffer("maps/e1m1.bmp");

            Dictionary<Color, Texture> dict = new();
            dict[(0, 0, 255)] = LoadTexture("blue_bricks/1.png");
            dict[(0, 0, 128)] = LoadTexture("blue_bricks/2.png");
            dict[(0, 0, 64)] = LoadTexture("blue_bricks/cell.png");
            dict[(0, 128, 0)] = LoadTexture("blue_bricks/cell_skull.png");
            dict[(128, 128, 128)] = LoadTexture("stones/1.png");
            dict[(64, 64, 64)] = LoadTexture("stones/2.png");
            dict[(255, 0, 0)] = LoadTexture("stones/flag.png");
            dict[(128, 64, 0)] = LoadTexture("stones/hitler.png");
            dict[(255, 255, 0)] = LoadTexture("stones/eagle.png");
            dict[(255, 128, 0)] = LoadTexture("wood/l.png");
            dict[(255, 192, 128)] = LoadTexture("wood/eagle.png");
            dict[(80, 40, 0)] = LoadTexture("wood/hitler.png");
            dict[(128, 255, 128)] = LoadTexture("control/elevator_btn.png");

            // BlockMap BlockMap = new BlockMap(map: grid, textureBindings: binds);

            BlockMap blockMap = new(
                map: blockmap_buffer,
                mapTexture: color =>
                {
                    if (dict.TryGetValue(color, out Texture texture))
                        return texture;
                    return null;
                },
                textureFilling: BlockMap.TextureFilling.Side,
                optimize: true,
                colliders: true
            );
            Add(blockMap);
        }

        // Floor and ceiling
        {
            Texture bricks = LoadTexture("bricks/normal.png");
            Texture checkered = LoadTexture("tiles/1.png");

            var floor = new Floor((0, 0), (64, 64), checkered);
            var ceiling = new Ceiling((0, 0), (64, 64), bricks);
            Add(floor, ceiling);
        }

        // Flag
        {
            Texture tex = LoadTexture("stones/flag.png");

            RegularPolygon polygon = new(
                position: Vector.Zero, // To be defined by flag behavior
                radius: 0.2f,
                vertices: 3,
                texture: tex
            );

            FlagBehavior flagBehavior = new([
                (57.5f, 31.5f),
                    (50.5f, 34.5f),
                    (38.5f, 34.5f),
                    (33.5f, 43.5f),
                    (33.5f, 53.5f),
                    (36.5f, 60.5f),
                    (30.5f, 60.5f),
                    (33.5f, 53.5f),
                    (33.5f, 43.5f),
                    (29.5f, 34.5f),
                    (15.5f, 34.5f),
                    (11.5f, 29.5f),
                    (16.5f, 18.5f),
                    (20.5f, 10.5f),
                    (33.5f, 6.5f),
                    (35.5f, 2.5f),
                    (46.5f, 3.5f),
                    (38.5f, 10.5f),
                    (21.5f, 10.5f),
                    (17.5f, 15.5f),
                    (14.5f, 34.5f),
                    (38.5f, 34.5f),
                    (57.5f, 33.5f),
                    (57.5f, 28.5f),
                ]);
            var rotate = new Rotate(360f);
            polygon.AddScripts(flagBehavior, rotate);
            Add(polygon);
        }

        // Highscore
        {
            Texture tex = Texture.FromColor(Color.Green, out TextureBuffer highscoreBuffer);
            buffers.Add(highscoreBuffer);
            Entity highscore = new RegularPolygon(Vector.Zero, 16, 0.02f, tex);
            highscore.Name = "highscore";
            highscore.AddScript(new TrajectoryRecorder(1f / 20f));
            Add(highscore);
        }

        // Camera
        {
            Player.AddScript<Q1Controller>();
            Player.AddScript(new KinematicBody
            {
                StartPosition = (57.5f, 29.5f),
                Radius = 0.15f,
            });
        }

        // Renderer customization
        // FIXME: Create a setup method.
        //global::GLTech.Engine.FieldOfView = 93.93f;
    }

    static TextureBuffer Resize(TextureBuffer pb, int scale)
    {
        TextureBuffer resized = new TextureBuffer(pb.Width * scale, pb.Height * scale);

        Parallel.For(0, resized.Height, line =>
        {
            float float_match_line = (float)line / scale;

            int matchline = (int)float_match_line;
            float rest_line = float_match_line - matchline;

            for (int column = 0; column < resized.Width; column++)
            {
                float float_match_col = (float)column / scale;

                int matchcol = (int)float_match_col;
                float rest_col = float_match_col - matchcol;

                Color top = pb[matchcol, matchline].Lerp(pb[matchcol + 1, matchline], rest_col);
                Color bot = pb[matchcol, matchline + 1].Lerp(pb[matchcol + 1, matchline + 1], rest_col);

                Color center = top.Lerp(bot, rest_line);
                resized[column, line] = center;
            }
        });

        return resized;
    }
}
