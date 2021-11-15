using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using GLTech2.Imaging;
using GLTech2.Unmanaged;

namespace GLTech2
{
    // This is the part really renders.
    partial class Engine
    {
        private unsafe static void DrawPlanes(PixelBuffer screen, SScene* scene)
        {
            // Checks if the code should be run in all cores or just one.
            if (ParallelRendering)
                Parallel.For(fromInclusive: 0, toExclusive: screen.width, body: DrawColumn);
            else
                for (int i = 0; i < screen.width; i++)
                    DrawColumn(i);

            // Render a vertical column of the screen.
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void DrawColumn(int screen_column)
            {
                // Caching frequently used variables
                float ray_cos = cache->cosines[screen_column];
                float ray_angle = cache->angles[screen_column] + scene->camera->rotation;
                Texture background = scene->background;
                Ray ray = new Ray(scene->camera->position, ray_angle);

                // Cast the ray towards every plane.
                SPlane* nearest = scene->NearestPlane(ray, out float nearest_dist, out float nearest_ratio);

                // Found out that optimizing this part by separing the case when it hits and not a wall is unecessary.
                #region Render the plane

                // Height that the current column should have on the screen.
                float columnHeight = (cache->colHeight1 / (ray_cos * nearest_dist)); // Wall column size in pixels

                // Where the column starts and ends relative to the screen.
                float column_start = (screen.height_float - columnHeight) / 2f;
                float column_end = (screen.height_float + columnHeight) / 2f;

                // Wall rendering bounds on the screen...
                int draw_column_start = screen.height - (int)(screen.height - column_start);    // Inclusive
                int draw_column_end = screen.height - (int)(screen.height - column_end);        // Exclusive

                // Which cannot exceed the full screen bounds.
                if (draw_column_start < 0)
                    draw_column_start = 0;
                if (draw_column_end > screen.height)
                    draw_column_end = screen.height;

                // Draws the background before the wall.
                // Critical performance impact.
                if (scene->background.buffer.uint0 != null)
                    for (int line = 0; line < draw_column_start; line++)
                        drawBackground(line);

                // Draw the wall
                // Critical performance impact.
                for (int line = draw_column_start; line < draw_column_end; line++)
                {
                    float vratio = (line - column_start) / columnHeight;
                    Color color = nearest->texture.MapPixel(nearest_ratio, vratio);
                    screen[screen_column, line] = color;
                }

                // Draw the other side of the background
                // Critical performance impact.
                if (scene->background.buffer.uint0 != null)
                    for (int line = draw_column_end; line < screen.height; line++)
                        drawBackground(line);
                #endregion

                // Draws background
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                void drawBackground(int line)
                {
                    float background_hratio = ray_angle / 360 + 1; //Temporary bugfix to avoid hratio being < 0
                    float screenVratio = line / screen.height_float;
                    float background_vratio = (1 - ray_cos) / 2 + ray_cos * screenVratio;
                    uint color = background.MapPixel(background_hratio, background_vratio);
                    screen[screen_column, line] = color;
                }
            }
        }

        // This method is not used anymore.
        private unsafe static void DrawPlanesLegacy(PixelBuffer target, SScene* scene)        // Must be changed
        {
            // Caching frequently used values.
            uint* buffer = target.uint0;
            int width = target.width;
            int height = target.height;
            Texture background = scene->background;

            // 
            if (ParallelRendering)
            {
                Parallel.For(0, width, Loop);
            }
            else
                for (int i = 0; i < width; i++)
                    Loop(i);

            void Loop(int ray_id)
            //for (int ray_id = 0; ray_id < rendererData->bitmap_width; ray_id++)
            {
                // Caching
                float ray_cos = cache->cosines[ray_id];
                float ray_angle = cache->angles[ray_id] + scene->camera->rotation;
                Ray ray = new Ray(scene->camera->position, ray_angle);

                // Cast the ray towards every wall.
                SPlane* nearest = scene->NearestPlane(ray, out float nearest_dist, out float nearest_ratio);
                if (nearest_ratio != 2f)
                {
                    float columnHeight = (cache->colHeight1 / (ray_cos * nearest_dist)); //Wall column size in pixels
                    float fullColumnRatio = height / columnHeight;
                    float topIndex = -(fullColumnRatio - 1f) / 2f;
                    for (int line = 0; line < height; line++)
                    {
                        // Critical performance impact.
                        float vratio = topIndex + fullColumnRatio * line / height;
                        if (vratio < 0f || vratio >= 1f)
                        {
                            //PURPOSELY REPEATED CODE!
                            float background_hratio = ray_angle / 360 + 1; //Temporary bugfix to avoid hratio being < 0
                            float screenVratio = (float)line / height;
                            float background_vratio = (1 - ray_cos) / 2 + ray_cos * screenVratio;
                            uint color = background.MapPixel(background_hratio, background_vratio);
                            buffer[width * line + ray_id] = color;
                        }
                        else
                        {
                            uint pixel = nearest->texture.MapPixel(nearest_ratio, vratio);
                            buffer[width * line + ray_id] = pixel;
                        }
                    }
                }
                else
                {
                    for (int line = 0; line < height; line++)
                    {
                        //Critical performance impact.
                        //PURPOSELY REPEATED CODE!
                        float background_hratio = ray_angle / 360 + 1;
                        float screenVratio = (float)line / height;
                        float background_vratio = (1 - ray_cos) / 2 + ray_cos * screenVratio;
                        uint color = background.MapPixel(background_hratio, background_vratio);
                        buffer[width * line + ray_id] = color;
                    }
                }
            }
        }
    }
}
