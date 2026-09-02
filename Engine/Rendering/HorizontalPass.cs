using Engine.Window;
using GLTech;
using GLTech.Structs;
using System.Runtime.CompilerServices;
using BitOperations = System.Numerics.BitOperations;

namespace Engine.Rendering
{
    internal unsafe interface IWallOcclusion
    {
        static abstract bool Visible(int* wallBound, int col, int line);
    }

    // Floors are visible below the bottom of the wall column.
    internal unsafe struct BelowWalls : IWallOcclusion
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Visible(int* wallBound, int col, int line) => wallBound[col] <= line;
    }

    // Ceilings are visible above the top of the wall column.
    internal unsafe struct AboveWalls : IWallOcclusion
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Visible(int* wallBound, int col, int line) => wallBound[col] > line;
    }

    // Span-based horizontal-plane renderer (floors and ceilings).
    //
    // Every scanline of a horizontal plane lies at a single view-space depth, so it
    // sweeps a straight world-space segment. Prepare() runs once per frame: it culls
    // the triangles against the view wedge, converts each edge into scanline clip
    // constants and buckets the triangle into the range of scanlines whose depth it
    // can intersect. DrawLine() then clips the scanline segment against each bucketed
    // triangle (three half-plane tests yielding one column interval) and rasterizes
    // the span with fixed-point UV stepping.
    internal unsafe class HorizontalPass
    {
        struct FlatTri
        {
            // Edge i keeps column t visible iff Ai + hit*Bi + (t - halfw)*step*Ci >= 0,
            // where hit is the scanline depth and step the world size of one pixel.
            public float A0, B0, C0;
            public float A1, B1, C1;
            public float A2, B2, C2;

            public uint* texels;
            public int wmask, hmask, hshift;
            public float su, ou, sv, ov;   // world position -> texel space
        }

        readonly bool isFloor;

        FlatTri[] flats = new FlatTri[64];
        int[] lineLo = new int[64];
        int[] lineHi = new int[64];
        int flatCount;

        // CSR buckets: triangle indices per scanline, rebuilt every frame.
        int[] bucketStart = new int[1];
        int[] bucketCursor = new int[1];
        int[] entries = new int[64];

        int lineBase, linesCount;

        Vector camPos, camDir, camRight;
        float planeDist, step0, halfHeight;
        int width, height, halfw;

        public HorizontalPass(bool isFloor)
        {
            this.isFloor = isFloor;
        }

        public void Prepare(TriangleList list, Vector camPos, Vector camDir, Vector camRight,
            Vector frustumLeft, Vector frustumRight, float planeDist, float step0, int width, int height)
        {
            this.camPos = camPos;
            this.camDir = camDir;
            this.camRight = camRight;
            this.planeDist = planeDist;
            this.step0 = step0;
            this.width = width;
            this.height = height;
            halfw = width >> 1;
            halfHeight = height * 0.5f;

            int half = height >> 1;
            lineBase = isFloor ? half : 0;
            linesCount = isFloor ? height - half : half;

            if (bucketStart.Length < linesCount + 1)
            {
                bucketStart = new int[linesCount + 1];
                bucketCursor = new int[linesCount + 1];
            }
            Array.Clear(bucketStart, 0, linesCount + 1);

            float det = Vector.CrossProduct(frustumLeft, frustumRight);
            int lastLine = lineBase + linesCount - 1;

            flatCount = 0;
            for (TriangleList.Node* node = list.first; node != null; node = node->next)
            {
                TriangleStruct* tri = node->data;

                Texture texture = tri->texture;
                if (texture.source.buffer == null)
                    continue;

                Vector ra = tri->a - camPos;
                Vector rb = tri->b - camPos;
                Vector rc = tri->c - camPos;

                // Conservative wedge cull: reject only when every vertex lies beyond
                // the same frustum edge.
                if (det * Vector.CrossProduct(frustumLeft, ra) < 0f &&
                    det * Vector.CrossProduct(frustumLeft, rb) < 0f &&
                    det * Vector.CrossProduct(frustumLeft, rc) < 0f)
                    continue;
                if (det * Vector.CrossProduct(ra, frustumRight) < 0f &&
                    det * Vector.CrossProduct(rb, frustumRight) < 0f &&
                    det * Vector.CrossProduct(rc, frustumRight) < 0f)
                    continue;

                float pa = Vector.DotProduct(ra, camDir);
                float pb = Vector.DotProduct(rb, camDir);
                float pc = Vector.DotProduct(rc, camDir);
                float dmin = MathF.Min(pa, MathF.Min(pb, pc));
                float dmax = MathF.Max(pa, MathF.Max(pb, pc));
                if (dmax <= 0f)
                    continue;

                // Depth interval -> conservative scanline interval. A line the triangle
                // doesn't actually reach only costs an empty clip in DrawLine.
                float offMin = planeDist / (dmax * step0);
                float offMax = dmin > 0f ? planeDist / (dmin * step0) : float.MaxValue;

                int lo, hi;
                if (isFloor)
                {
                    float loF = halfHeight + offMin;
                    float hiF = halfHeight + offMax;
                    lo = loF >= height ? height : (int)loF;
                    hi = hiF >= lastLine ? lastLine : (int)MathF.Ceiling(hiF);
                }
                else
                {
                    float loF = halfHeight - offMax;
                    float hiF = halfHeight - offMin;
                    lo = loF <= 0f ? 0 : (int)loF;
                    hi = hiF <= -1f ? -1 : (hiF >= lastLine ? lastLine : (int)MathF.Ceiling(hiF));
                }
                if (lo < lineBase)
                    lo = lineBase;
                if (lo > hi)
                    continue;

                if (flatCount == flats.Length)
                {
                    Array.Resize(ref flats, flatCount * 2);
                    Array.Resize(ref lineLo, flatCount * 2);
                    Array.Resize(ref lineHi, flatCount * 2);
                }

                ref FlatTri flat = ref flats[flatCount];
                flat.A0 = Vector.DotProduct(tri->n0, camPos) - tri->d0;
                flat.B0 = Vector.DotProduct(tri->n0, camDir);
                flat.C0 = Vector.DotProduct(tri->n0, camRight);
                flat.A1 = Vector.DotProduct(tri->n1, camPos) - tri->d1;
                flat.B1 = Vector.DotProduct(tri->n1, camDir);
                flat.C1 = Vector.DotProduct(tri->n1, camRight);
                flat.A2 = Vector.DotProduct(tri->n2, camPos) - tri->d2;
                flat.B2 = Vector.DotProduct(tri->n2, camDir);
                flat.C2 = Vector.DotProduct(tri->n2, camRight);

                flat.texels = texture.source.buffer;
                flat.wmask = texture.source.width - 1;
                flat.hmask = texture.source.height - 1;
                flat.hshift = BitOperations.TrailingZeroCount(texture.source.height);
                flat.su = texture.source.widthf * texture.hrepeat;
                flat.ou = texture.source.widthf * texture.hoffset;
                flat.sv = texture.source.heightf * texture.vrepeat;
                flat.ov = texture.source.heightf * texture.voffset;

                lineLo[flatCount] = lo;
                lineHi[flatCount] = hi;

                for (int l = lo; l <= hi; l++)
                    bucketStart[l - lineBase + 1]++;

                flatCount++;
            }

            for (int i = 1; i <= linesCount; i++)
                bucketStart[i] += bucketStart[i - 1];

            int total = bucketStart[linesCount];
            if (entries.Length < total)
                entries = new int[Math.Max(total, entries.Length * 2)];

            Array.Copy(bucketStart, bucketCursor, linesCount + 1);
            for (int i = 0; i < flatCount; i++)
                for (int l = lineLo[i]; l <= lineHi[i]; l++)
                    entries[bucketCursor[l - lineBase]++] = i;
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public void DrawLine<TOccl>(int line, RenderTarget target, int* wallBound)
            where TOccl : struct, IWallOcclusion
        {
            uint* row = (uint*)target.buffer + (nint)target.width * line;
            int width = this.width;

            float offset = isFloor ? line - halfHeight : halfHeight - line;
            float hit = planeDist / (offset * step0);

            int bucket = line - lineBase;
            int e0 = bucketStart[bucket];
            int e1 = bucketStart[bucket + 1];

            if (e0 == e1 || !float.IsFinite(hit))
            {
                for (int col = 0; col < width; col++)
                    if (TOccl.Visible(wallBound, col, line))
                        row[col] = 0;
                return;
            }

            float step = step0 * hit;
            float halfSpan = step * halfw;
            float px0 = camPos.x + camDir.x * hit - camRight.x * halfSpan;
            float py0 = camPos.y + camDir.y * hit - camRight.y * halfSpan;
            float dpx = camRight.x * step;
            float dpy = camRight.y * step;

            // Background first; spans overwrite it. Overlapping triangles are
            // undefined behavior, so span order doesn't matter.
            for (int col = 0; col < width; col++)
                if (TOccl.Visible(wallBound, col, line))
                    row[col] = 0;

            var flats = this.flats;
            var entries = this.entries;

            for (int e = e0; e < e1; e++)
            {
                ref FlatTri flat = ref flats[entries[e]];

                // Clip the scanline's column range against the three edges.
                float lo = 0f;
                float hi = width - 1;

                float g = flat.A0 + hit * flat.B0 - halfSpan * flat.C0;
                float h = step * flat.C0;
                if (h > 0f) { g = -g / h; if (g > lo) lo = g; }
                else if (h < 0f) { g = -g / h; if (g < hi) hi = g; }
                else if (g < 0f) continue;

                g = flat.A1 + hit * flat.B1 - halfSpan * flat.C1;
                h = step * flat.C1;
                if (h > 0f) { g = -g / h; if (g > lo) lo = g; }
                else if (h < 0f) { g = -g / h; if (g < hi) hi = g; }
                else if (g < 0f) continue;

                g = flat.A2 + hit * flat.B2 - halfSpan * flat.C2;
                h = step * flat.C2;
                if (h > 0f) { g = -g / h; if (g > lo) lo = g; }
                else if (h < 0f) { g = -g / h; if (g < hi) hi = g; }
                else if (g < 0f) continue;

                if (lo > hi)
                    continue;

                int c0 = (int)MathF.Ceiling(lo);
                int c1 = (int)hi;
                if (c0 > c1)
                    continue;

                // Fixed-point (48.16) UV stepping. The doubles in the setup keep texel
                // precision over long spans and large world coordinates; the arithmetic
                // shift in the loop floors negative coordinates, so tiling wraps
                // correctly on both sides of the origin.
                double wx = px0 + (double)c0 * dpx;
                double wy = py0 + (double)c0 * dpy;
                long u = (long)((flat.su * wx + flat.ou) * 65536.0);
                long v = (long)((flat.sv * wy + flat.ov) * 65536.0);
                long du = (long)(flat.su * (double)dpx * 65536.0);
                long dv = (long)(flat.sv * (double)dpy * 65536.0);

                uint* texels = flat.texels;
                int wmask = flat.wmask;
                int hmask = flat.hmask;
                int hshift = flat.hshift;

                for (int col = c0; col <= c1; col++)
                {
                    if (TOccl.Visible(wallBound, col, line))
                    {
                        int x = (int)(u >> 16) & wmask;
                        int y = (int)(v >> 16) & hmask;
                        row[col] = texels[(x << hshift) + y];
                    }
                    u += du;
                    v += dv;
                }
            }
        }
    }
}
