using GLTech;
using System.Runtime.CompilerServices;

namespace Engine.Rendering
{
    internal unsafe static class WallPass
    {

        /// <summary>
        /// Culls a list of world space segments and resolves the survivors into screen space walls.
        /// The intermediate view space has the camera at the origin, +y meaning front and +x right.
        /// </summary>
        /// <remarks>
        /// <paramref name="outWalls"/> and <paramref name="outTextures"/> must have, at least,
        /// <paramref name="inCount"/> of capacity. <paramref name="outTextures"/> is allowed to
        /// alias <paramref name="inTextures"/>: compaction never writes ahead of what it has read.
        /// </remarks>
        public static void GatherScreenWalls(
            CameraInfo camera,
            in Projection projection,
            int inCount,
            Segment* inSegments,
            Texture* inTextures,
            out int outCount,
            ScreenWall* outWalls,
            Texture* outTextures
        )
        {
            float slope = projection.frustrumSlope;
            float halfWidth = projection.halfWidth;

            // colHeight1 is 1 / columnSlope, which is exactly what turns a ray slope into a column.
            float invColumnSlope = projection.colHeight1;
            int lastImageColumn = (int)(halfWidth + halfWidth) - 1;

            outCount = 0;
            for (int i = 0; i < inCount; i++)
            {
                var segment = inSegments[i];

                Vector relative = segment.start - camera.position;

                // Cull backfaces
                if (Vector.CrossProduct(relative, segment.direction) >= 0)
                    continue;

                // Project the segment to view space
                var start = relative / camera.direction;
                var dir = segment.direction / camera.direction;

                // Cull by frustrum using Liam-Barsky algorithm
                if (!ClipToFrustrum(start, dir, slope, out float enter, out float exit))
                    continue;

                // The sides bound |x| by slope * y, so a clipped point can only reach y == 0 at the
                // origin, which the backface cull already excluded. Both ratios are therefore
                // finite and inside [-slope, slope], and both columns land inside the image.
                Vector first = start + enter * dir;
                Vector last = start + exit * dir;

                // Front facing walls run left to right on the screen, so this order holds.
                int firstColumn = (int)MathF.Ceiling(halfWidth + invColumnSlope * (first.x / first.y));
                int lastColumn = (int)MathF.Floor(halfWidth + invColumnSlope * (last.x / last.y));

                if (firstColumn < 0) firstColumn = 0;
                if (lastColumn > lastImageColumn) lastColumn = lastImageColumn;

                // A thin wall can fall entirely between two column centres.
                if (firstColumn > lastColumn)
                    continue;

                // Same cross product as the backface cull, now in view space. It is the constant
                // numerator of the depth along every ray, so both slopes fall out of its inverse.
                float invCross = 1f / Vector.CrossProduct(start, dir);

                outWalls[outCount] = new ScreenWall(
                    invDepthSlope: dir.y * invCross,
                    invDepthBase: -dir.x * invCross,
                    uSlope: -start.y * invCross,
                    uBase: start.x * invCross,
                    firstColumn: firstColumn,
                    lastColumn: lastColumn);
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
                    float t = (column - halfWidth) * columnSlope;

                    int closestIndex = NearestWall(walls, column, t, out float invDepth);

                    // No wall covers this column
                    if (closestIndex == -1)
                    {
                        colStarts[column] = target.height;
                        colEnds[column] = 0;
                        continue;
                    }

                    ScreenWall* wall = walls.items + closestIndex;

                    // The only division in the column; the three quantities below share it.
                    float depth = 1f / invDepth;
                    float textureU = MathF.FusedMultiplyAdd(wall->uSlope, t, wall->uBase) * depth;

                    // Height the current column should have on the screen...
                    float colHeightF = colHeight1 * invDepth;

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
                    float iColH = columnSlope * depth; // 1 / colHeightF, without a second divide
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

        // Nearest is the largest inverse depth, so the search itself never divides.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int NearestWall(WallsInfo walls, int column, float t, out float invDepth)
        {
            int closest = -1;

            // Zero stands for infinitely far away: any wall covering the column beats it.
            float best = 0f;

            for (int i = 0; i < walls.count; i++)
            {
                ScreenWall* wall = walls.items + i;

                if (column < wall->firstColumn || column > wall->lastColumn)
                    continue;

                float current = MathF.FusedMultiplyAdd(wall->invDepthSlope, t, wall->invDepthBase);

                // Phrased as a positive test on purpose: every comparison against a NaN is false,
                // so a degenerate wall fails to be accepted. A reject guard would let it fall
                // through instead and poison the rest of the column.
                if (current > best)
                {
                    best = current;
                    closest = i;
                }
            }

            invDepth = best;
            return closest;
        }

        /// <summary>
        /// Liang-Barsky against the three frustrum half planes. Each one is affine along the
        /// segment, so clipping is a running [enter, exit] interval on its parameter, and the
        /// interval collapsing is an exact reject: no separate culling test is needed.
        /// </summary>
        private static bool ClipToFrustrum(Vector start, Vector dir, float slope, out float enter, out float exit)
        {
            enter = 0f;
            exit = 1f;

            Vector end = start + dir;

            return Narrow(start.y, end.y, ref enter, ref exit)                                   // Near
                && Narrow(start.x + slope * start.y, end.x + slope * end.y, ref enter, ref exit)  // Left
                && Narrow(slope * start.y - start.x, slope * end.y - end.x, ref enter, ref exit); // Right
        }

        // Narrows [enter, exit] to the stretch where an affine function stays non negative, given
        // its values at both ends of the segment. Returns false once nothing is left.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Narrow(float f0, float f1, ref float enter, ref float exit)
        {
            if (f0 < 0f)
            {
                // Fully outside this plane, and parallel to it when f1 == f0.
                if (f1 <= 0f)
                    return false;

                float hit = f0 / (f0 - f1);
                if (hit > enter) enter = hit;
            }
            else if (f1 < 0f)
            {
                float hit = f0 / (f0 - f1);
                if (hit < exit) exit = hit;
            }

            return enter < exit;
        }
    }
}
