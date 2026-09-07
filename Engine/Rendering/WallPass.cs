using GLTech;
using System.Runtime.CompilerServices;

namespace Engine.Rendering
{
    internal unsafe static class WallPass
    {

        /// <summary>
        /// Transforms a list of segments to view space and culls by front face, near plane and frustrum sides.
        /// +y means front, +x means right
        /// </summary>
        /// <remarks>
        /// <paramref name="outSegments"/> and <paramref name="outTextures"/> must have, at least, <paramref name="inCount"/> of capacity.
        /// </remarks>
        public static void GatherViewspaceWalls(
            CameraInfo camera,
            in Projection projection,
            int inCount,
            Segment* inSegments,
            Texture* inTextures,
            out int outCount,
            Segment* outSegments,
            Texture* outTextures
        )
        {

            float slope = projection.frustrumSlope;

            outCount = 0;
            for (int i = 0; i < inCount; i++)
            {
                var segment = inSegments[i];

                Vector relative = segment.start - camera.position;

                // Cull backfaces
                if (Vector.CrossProduct(relative, segment.direction) >= 0)
                    continue;

                // Project the segment to view space
                var newStart = relative / camera.direction;
                var newDir = segment.direction / camera.direction;
                var newEnd = newStart + newDir;

                // Cull by frustrum
                if ((Outcode(newStart, slope) & Outcode(newEnd, slope)) != 0)
                    continue;

                outSegments[outCount] = new Segment(newStart, newDir);
                outTextures[outCount] = inTextures[i];
                outCount++;
            }
        }

        public static void Render(in Frame frame, int from, int to)
        {
            // L1-ing/register-ing variables for performance
            var target = frame.image;
            var walls = frame.walls;
            int* colStarts = frame.spans.start, colEnds = frame.spans.end;
            float z = frame.camera.z,
                horizon = frame.projection.horizon,
                colHeight1 = frame.projection.colHeight1,
                columnSlope = frame.projection.columnSlope,
                halfWidth = frame.projection.halfWidth;

            // unchecked is load-bearing: the float to int casts below go out of range
            // when a wall gets very close to the camera, and checked would throw.
            unchecked
            {
                for (int column = from; column < to; column++)
                {
                    var delta = column - halfWidth;
                    var ray = new Segment(
                        Vector.Zero,
                        new Vector(delta * columnSlope, 1f)
                    );

                    int closestIndex = NearestWall(walls, ray, out float depth, out float textureU);

                    // No segment found
                    if (closestIndex == -1)
                    {
                        colStarts[column] = target.height;
                        colEnds[column] = 0;
                        continue;
                    }

                    // Height the current column should have on the screen...
                    float colHeightF = colHeight1 / depth;

                    // ...centered on the horizon and shifted by the eye height.
                    float colCenterF = horizon + colHeightF * (z - 0.5f);
                    float colHalfF = colHeightF * 0.5f;
                    float colStartF = colCenterF - colHalfF;
                    float colEndF = colCenterF + colHalfF;

                    // Wall rendering bounds on the screen...
                    int colStartI = target.height - (int)(target.heightf - colStartF); // Inclusive
                    if (colStartI < 0) colStartI = 0;

                    int colEndI = target.height - (int)(target.heightf - colEndF); // Exclusive
                    if (colEndI > target.height) colEndI = target.height;

                    colStarts[column] = colStartI;
                    colEnds[column] = colEndI;

                    // Draw the wall
                    float iColH = 1f / colHeightF;
                    Texture tex = walls.textures[closestIndex];
                    uint* texCol = tex.ResolveColumn(textureU);
                    for (int line = colStartI; line < colEndI; line++)
                    {
                        float v = (line - colStartF) * iColH;
                        Color color = tex.MapColumn(texCol, v);
                        target.buffer[column + target.width * line] = color;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int NearestWall(WallsInfo walls, Segment ray, out float depth, out float u)
        {
            int closest = -1;
            depth = float.PositiveInfinity;
            u = 0f;

            for (int i = 0; i < walls.count; i++)
            {
                Vector rs = walls.segments[i].GetRS(ray);

                // In front of the camera, inside the wall bounds, and nearer than the current
                // best. Phrased as a positive test on purpose: every comparison against a NaN
                // is false, so a degenerate wall fails to be accepted. A reject guard would let
                // it fall through instead and poison depth for the rest of the column.
                if (rs.y >= 0f && rs.x >= 0f && rs.x < 1f && rs.y < depth)
                {
                    depth = rs.y;
                    u = rs.x;
                    closest = i;
                }
            }

            return closest;
        }

        private const int BEHIND = 1;
        private const int LEFT = 2;
        private const int RIGHT = 4;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Outcode(Vector point, float slope)
        {
            float lateral = slope * point.y;

            int code = 0;
            if (point.y <= 0f) code |= BEHIND;
            if (point.x < -lateral) code |= LEFT;
            if (point.x > lateral) code |= RIGHT;
            return code;
        }
    }
}
