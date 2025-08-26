using Engine.Imaging;
using Engine.World;
using Engine.World.Composed;
using Engine.Scripting.Debugging;
using Engine.Scripting.Physics;
using Engine.Scripting.Prefab;

using System.Threading.Tasks;
using System.Collections.Generic;
using Engine.Scripting;
using System.Diagnostics;

namespace Engine.Demos.Wolfenstein
{
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

        void Start()
        {
            Entity.WorldPosition = checkpoints[0];
            Entity? highscore = Scene.FindByname("highscore");
            trajectoryRecorder = highscore?.GetScript<TrajectoryRecorder>();
        }

        void OnFrame()
        {
            float distance = (Entity.WorldPosition - Scene.Camera.WorldPosition).Module;
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
        Image textures = new(Resources.textures);
        Image textures_censored = new(Resources.textures_censored);
        Image background_buffer = new(Resources.bg);
        Image lula_buffer = new(Resources.lula);
        Image pt_buffer = new(Resources.pt);
        Image bolsonaro_buffer = new(Resources.bolsonaro);

        protected override void Delete()
        {
            textures.Dispose();
            textures_censored.Dispose();
            background_buffer.Dispose();
            lula_buffer.Dispose();
            pt_buffer.Dispose();
            bolsonaro_buffer.Dispose();
        }

        public Map()
        {
            Background = new Texture(background_buffer);

            // BlockMap
            {
                using Image blockmap_buffer = new Image(Resources.MapGrid);
                Dictionary<Color, Texture> dict = new();
                {
                    Texture blueStone1 = new Texture(
                        source: textures,
                        hrepeat: 1 / 6f,
                        vrepeat: 1 / 19f,
                        hoffset: 2 / 6f,
                        voffset: 2 / 19f);

                    Texture blueStone2 = new Texture(
                        source: textures,
                        hrepeat: 1 / 6f,
                        vrepeat: 1 / 19f,
                        hoffset: 4 / 6f,
                        voffset: 2 / 19f);

                    Texture bluestoneCell = new Texture(
                        source: textures,
                        hrepeat: 1 / 6f,
                        vrepeat: 1 / 19f,
                        hoffset: 2 / 6f,
                        voffset: 1 / 19f);

                    Texture bluestoneCellSkeleton = new Texture(
                        source: textures,
                        hrepeat: 1 / 6f,
                        vrepeat: 1 / 19f,
                        hoffset: 0 / 6f,
                        voffset: 2 / 19f);

                    Texture grayStone1 = new Texture(
                        source: textures,
                        hrepeat: 1 / 6f,
                        vrepeat: 1 / 19f,
                        hoffset: 0 / 6f,
                        voffset: 0 / 19f);

                    Texture grayStone2 = new Texture(
                        source: textures,
                        hrepeat: 1 / 6f,
                        vrepeat: 1 / 19f,
                        hoffset: 2 / 6f,
                        voffset: 0 / 19f);

                    //Texture gs_naziFlag = new Texture(
                    //    source: textures,
                    //    hrepeat: 1 / 6f,
                    //    vrepeat: 1 / 19f,
                    //    hoffset: 4 / 6f,
                    //    voffset: 0 / 19f);

                    Texture gs_naziFlag = new(
                        source: textures_censored,
                        hrepeat: 1f / 4f,
                        vrepeat: 1f / 4f,
                        hoffset: 2f / 4f,
                        voffset: 2f / 4f);

                    //Texture gs_hitler = new Texture(
                    //    source: textures,
                    //    hrepeat: 1 / 6f,
                    //    vrepeat: 1 / 19f,
                    //    hoffset: 0 / 6f,
                    //    voffset: 1 / 19f);

                    Texture gs_hitler = new(
                        source: textures_censored,
                        hrepeat: 1f / 4f,
                        vrepeat: 1f / 4f,
                        hoffset: 1f / 4f,
                        voffset: 2f / 4f);

                    //Texture gs_goldEagle = new Texture(
                    //    source: textures,
                    //    hrepeat: 1 / 6f,
                    //    vrepeat: 1 / 19f,
                    //    hoffset: 4 / 6f,
                    //    voffset: 1 / 19f);

                    Texture gs_goldEagle = new(
                        source: textures_censored,
                        hrepeat: 1f / 4f,
                        vrepeat: 1f / 4f,
                        hoffset: 0f / 4f,
                        voffset: 2f / 4f);

                    Texture woodPanelling = new Texture(
                        source: textures,
                        hrepeat: 1 / 6f,
                        vrepeat: 1 / 19f,
                        hoffset: 4 / 6f,
                        voffset: 3 / 19f);

                    //Texture wp_whiteEagle = new Texture(
                    //    source: textures,
                    //    hrepeat: 1 / 6f,
                    //    vrepeat: 1 / 19f,
                    //    hoffset: 0 / 6f,
                    //    voffset: 3 / 19f);

                    Texture wp_whiteEagle = new(
                        source: textures_censored,
                        hrepeat: 1f / 4f,
                        vrepeat: 1f / 4f,
                        hoffset: 3f / 4f,
                        voffset: 1f / 4f);

                    //Texture wp_hitler = new Texture(
                    //    source: textures,
                    //    hrepeat: 1 / 6f,
                    //    vrepeat: 1 / 19f,
                    //    hoffset: 2 / 6f,
                    //    voffset: 3 / 19f);

                    Texture wp_hitler = new(
                        source: textures_censored,
                        hrepeat: 1f / 4f,
                        vrepeat: 1f / 4f,
                        hoffset: 2f / 4f,
                        voffset: 1f / 4f);

                    Texture elevator = new Texture(
                        source: textures,
                        hrepeat: 1 / 6f,
                        vrepeat: 1 / 19f,
                        hoffset: 0 / 6f,
                        voffset: 17 / 19f);

                    dict[(0, 0, 255)] = blueStone1;
                    dict[(0, 0, 128)] = blueStone2;
                    dict[(0, 0, 64)] = bluestoneCell;
                    dict[(0, 128, 0)] = bluestoneCellSkeleton;
                    dict[(128, 128, 128)] = grayStone1;
                    dict[(64, 64, 64)] = grayStone2;
                    dict[(255, 0, 0)] = gs_naziFlag;
                    dict[(128, 64, 0)] = gs_hitler;
                    dict[(255, 255, 0)] = gs_goldEagle;
                    dict[(255, 128, 0)] = woodPanelling;
                    dict[(255, 192, 128)] = wp_whiteEagle;
                    dict[(80, 40, 0)] = wp_hitler;
                    dict[(128, 255, 128)] = elevator;
                }

                // BlockMap BlockMap = new BlockMap(map: grid, textureBindings: binds);

                BlockMap blockMap = new(
                    map: blockmap_buffer,
                    mapTexture: color =>
                    {
                        if (dict.TryGetValue(color, out Texture texture))
                            return texture;
                        else return Texture.NullTexture;
                    },
                    textureFilling: BlockMap.TextureFilling.Side,
                    optimize: true,
                    colliders: true);
                Add(blockMap);
            }

            // Floor and ceiling
            {
                Texture bricks = new Texture(
                    source: textures,
                    hrepeat: 1f / 6f,
                    vrepeat: 1f / 19f,
                    hoffset: 3f / 6f,
                    voffset: 11f / 19f);

                Texture brownBricks = new Texture(
                    source: textures,
                    hrepeat: 1f / 6f,
                    vrepeat: 1f / 19f,
                    hoffset: 2f / 6f,
                    voffset: 9f / 19f);

                Texture checkered = new Texture(
                    source: textures,
                    hrepeat: 1f / 6f,
                    vrepeat: 1f / 19f,
                    hoffset: 4f / 6f,
                    voffset: 13f / 19f);

                var floor = new Floor((0, 0), (64, 64), checkered);
                var ceiling = new Ceiling((0, 0), (64, 64), bricks);
                Add(floor, ceiling);
            }

            // Flag
            {
                var tex = new Texture(
                    source: textures,
                    hrepeat: 1 / 6f,
                    vrepeat: 1 / 19f,
                    hoffset: 0 / 6f,
                    voffset: 6 / 19f);

                var polygon = new RegularPolygon(
                    position: Vector.Zero, // To be defined by flag behavior
                    radius: 0.2f,
                    vertices: 3,
                    texture: tex);

                var flagBehavior = new FlagBehavior(new Vector[] {
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
                });
                var rotate = new Rotate(360f);
                polygon.AddScripts(flagBehavior, rotate);
                Add(polygon);
            }

            // Highscore
            {
                Texture tex = Texture.FromColor(Color.Green, out _);
                Entity highscore = new RegularPolygon(Vector.Zero, 16, 0.02f, tex);
                highscore.Name = "highscore";
                highscore.AddScript(new TrajectoryRecorder(1f / 20f));
                Add(highscore);
            }

            // Camera
            {
                Camera.WorldPosition = (57.5f, 29.5f);
                Camera.AddScript<DebugScene>();
                Camera.AddScript(new MouseLook(2.2f));
                //Camera.AddScript<DebugEntity>();

                PointCollider pc = new();
                Q1Movement movement = new(pc);

                Camera.AddScript(pc);
                Camera.AddScript(movement);
            }

            // Renderer customization
            // FIXME: Create a setup method.
            global::Engine.Renderer.FieldOfView = 93.93f;
        }

        static Image Resize(Image pb, int scale)
        {
            Image resized = new Image(pb.Width * scale, pb.Height * scale);

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

                    Color top = pb[matchcol, matchline].Mix(pb[matchcol + 1, matchline], rest_col);
                    Color bot = pb[matchcol, matchline + 1].Mix(pb[matchcol + 1, matchline + 1], rest_col);

                    Color center = top.Mix(bot, rest_line);
                    resized[column, line] = center;
                }
            });

            return resized;
        }
    }
}