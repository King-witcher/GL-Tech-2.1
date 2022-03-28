using System;
using System.Threading.Tasks;

using Engine.Imaging;

namespace Engine.World.Prefab
{
    public partial class BlockMap : Entity
    {
        private protected override Vector PositionData { get; set; } = Vector.Zero;
        private protected override Vector DirectionData { get; set; } = Vector.Forward;

        private int walls = 0;

        public int Walls => walls;

        public enum Optimization
        {
            Low,
            Medium,
            High
        }

        public enum TextureFilling
        {
            Side,
            Block,
            Space
        }

        private enum Side : byte
        {
            top = 0b0001,
            right = 0b0010,
            bottom = 0b0100,
            left = 0b1000
        }

        internal BlockMap(Image map, Func<Color, Texture> mapTexture, TextureFilling textureFilling,
            Optimization optimization = Optimization.Medium, bool colliders = true)
        {
            if (optimization == Optimization.Low) make_O1();
            else if (optimization == Optimization.Medium) make_O2();

            Texture decideSideTexture(int line, int column, Texture texture, Side side)
                => textureFilling switch
                {
                    TextureFilling.Side => texture,
                    TextureFilling.Space => texture,
                    TextureFilling.Block => new Texture(
                            source: texture.source,
                            hoffset: texture.hrepeat * side switch
                            {
                                Side.left => 0f,
                                Side.bottom => 0.25f,
                                Side.right => 0.5f,
                                Side.top => 0.75f,
                                _ => throw new ArgumentException("side")
                            },
                            hrepeat: texture.hrepeat * 0.25f,
                            voffset: texture.voffset,
                            vrepeat: texture.vrepeat),
                    _ => throw new ArgumentException("textureFilling")
                };

            void makeSide(int column, int line, Texture block_texture, Side side)
            {
                Vector start, end;

                switch (side)
                {
                    case Side.left:
                        start = (line, column);
                        end = (line + 1, column);
                        break;

                    case Side.bottom:
                        start = (line + 1, column);
                        end = (line + 1, column + 1);
                        break;

                    case Side.right:
                        start = (line + 1, column + 1);
                        end = (line, column + 1);
                        break;

                    case Side.top:
                        start = (line, column + 1);
                        end = (line, column);
                        break;

                    default:
                        throw new ArgumentException("side");
                }

                Texture side_texture = decideSideTexture(line, column, block_texture, side);
                Plane plane = new(start, end, side_texture);
                plane.Parent = this;

                if (colliders)
                {
                    Collider collider = new(plane);
                    collider.Parent = this;
                }

                walls++;
            }

            void make_O1()
            {
                // Parallel.For causou problemas de concorrência.
                for (int column = 0; column < map.Width; column++)
                {
                    for (int line = 0; line < map.Height; line++)
                    {
                        var mapResult = mapTexture(map[column, line]);
                        makeBlock(line, column, mapResult);
                    }
                }

                void makeBlock(int line, int column, Texture block_texture)
                {
                    // Null Texture means that there's no block.
                    if (block_texture.source.Buffer == IntPtr.Zero) return;

                    makeSide(column, line, block_texture, Side.left);
                    makeSide(column, line, block_texture, Side.bottom);
                    makeSide(column, line, block_texture, Side.right);
                    makeSide(column, line, block_texture, Side.top);
                }
            }

            void make_O2()
            {
                // Cada bloco, em tese, geraria quatro paredes e quatro colisores ao seu redor.
                // Esse método foi otimizado considerando-se que, se duas paredes forem desenhadas sobre o mesmo lugar, isso significa que não existe superfície visível por ali e, portanto, nenhuma das duas deverá ser realmente renderizada.
                bool blockFree(int column, int line)
                {
                    if (column < 0 || column >= map.Height || line < 0 || line >= map.Width)
                        return true;
                    if (mapTexture(map[column, line]).IsNull)
                        return true;
                    return false;
                }

                for (int column = 0; column < map.Width; column++)
                {
                    for (int line = 0; line < map.Height; line++)
                    {
                        Texture block_texture = mapTexture(map[column, line]);
                        if (block_texture.IsNull) continue;

                        if (blockFree(column - 1, line))
                            makeSide(column, line, block_texture, Side.left);

                        if (blockFree(column, line + 1))
                            makeSide(column, line, block_texture, Side.bottom);

                        if (blockFree(column + 1, line))
                            makeSide(column, line, block_texture, Side.right);

                        if (blockFree(column, line - 1))
                            makeSide(column, line, block_texture, Side.top);
                    }
                }
            }
        }
    }
}
