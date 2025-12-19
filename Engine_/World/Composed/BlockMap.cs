namespace GLTech.World.Composed
{
    public partial class BlockMap : Entity
    {

        private int walls = 0;

        public int Walls => walls;

        public enum TextureFilling
        {
            Side,
            Block,
            Space
        }

        private enum Side
        {
            top,
            right,
            bottom,
            left
        }

        public BlockMap(Image map, Func<Color, Texture> mapTexture, TextureFilling textureFilling,
            bool optimize = true, bool colliders = true)
        {
            if (optimize) optmizedMode();
            else suboptimalMode();

            // Cada bloco, em tese, geraria quatro paredes e quatro colisores ao seu redor.
            // Esse método foi otimizado considerando-se que, se duas paredes forem desenhadas sobre o mesmo lugar, isso significa que não existe superfície visível por ali e, portanto, nenhuma das duas deverá ser realmente renderizada.
            bool isEmpty(int column, int line)
            {
                if (column < 0 || column >= map.Height || line < 0 || line >= map.Width)
                    return true;
                if (mapTexture(map[column, line]).IsNull)
                    return true;
                return false;
            }


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

            void suboptimalMode()
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

            void optmizedMode()
            {
                #region Make Planes
                void makePlaneOnly(int column, int line, Texture block_texture, Side side)
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
                }

                for (int column = 0; column < map.Width; column++)
                {
                    for (int line = 0; line < map.Height; line++)
                    {
                        Texture block_texture = mapTexture(map[column, line]);
                        if (block_texture.IsNull) continue;

                        if (isEmpty(column - 1, line))
                            makePlaneOnly(column, line, block_texture, Side.left);

                        if (isEmpty(column, line + 1))
                            makePlaneOnly(column, line, block_texture, Side.bottom);

                        if (isEmpty(column + 1, line))
                            makePlaneOnly(column, line, block_texture, Side.right);

                        if (isEmpty(column, line - 1))
                            makePlaneOnly(column, line, block_texture, Side.top);
                    }
                }
                #endregion

                #region Make Colliders
                // Suboptimal
                ushort[,,] joining_table = new ushort[map.Width, map.Height, 4];

                for (int column = 0; column < map.Width; column++)
                {
                    for (int line = 0; line < map.Height; line++)
                    {
                        if (isEmpty(column, line))
                            continue;

                        // Left
                        if (isEmpty(column - 1, line))
                        {
                            if (line == 0 || joining_table[column, line - 1, 0] == 0)
                            {
                                joining_table[column, line, 0] = 1;
                            }
                            else
                            {
                                joining_table[column, line, 0] = (ushort)(joining_table[column, line - 1, 0] + 1);
                                joining_table[column, line - 1, 0] = 0;
                            }
                        }

                        // Bottom
                        if (isEmpty(column, line + 1))
                        {
                            if (column == 0 || joining_table[column - 1, line, 1] == 0)
                            {
                                joining_table[column, line, 1] = 1;
                            }
                            else
                            {
                                joining_table[column, line, 1] = (ushort)(joining_table[column - 1, line, 1] + 1);
                                joining_table[column - 1, line, 1] = 0;
                            }
                        }

                        // Right
                        if (isEmpty(column + 1, line))
                        {
                            if (line == 0 || joining_table[column, line - 1, 2] == 0)
                            {
                                joining_table[column, line, 2] = 1;
                            }
                            else
                            {
                                joining_table[column, line, 2] = (ushort)(joining_table[column, line - 1, 2] + 1);
                                joining_table[column, line - 1, 2] = 0;
                            }
                        }

                        // Top
                        if (isEmpty(column, line - 1))
                        {
                            if (column == 0 || joining_table[column - 1, line, 3] == 0)
                            {
                                joining_table[column, line, 3] = 1;
                            }
                            else
                            {
                                joining_table[column, line, 3] = (ushort)(joining_table[column - 1, line, 3] + 1);
                                joining_table[column - 1, line, 3] = 0;
                            }
                        }
                    }
                }

                for (int column = 0; column < map.Width; column++)
                {
                    for (int line = 0; line < map.Height; line++)
                    {
                        // Left
                        int left_stack = joining_table[column, line, 0];
                        int bottom_stack = joining_table[column, line, 1];
                        int right_stack = joining_table[column, line, 2];
                        int top_stack = joining_table[column, line, 3];

                        if (left_stack != 0)
                        {
                            Vector start = (line - left_stack + 1, column);
                            Vector end = (line + 1, column);

                            Collider collider = new(start, end);
                            collider.Parent = this;
                        }

                        if (bottom_stack != 0)
                        {
                            Vector start = (line + 1, column + 1 - bottom_stack);
                            Vector end = (line + 1, column + 1);

                            Collider collider = new(start, end);
                            collider.Parent = this;
                        }

                        if (right_stack != 0)
                        {
                            Vector start = (line + 1, column + 1);
                            Vector end = (line - right_stack + 1, column + 1);

                            Collider collider = new(start, end);
                            collider.Parent = this;
                        }

                        if (top_stack != 0)
                        {
                            Vector start = (line, column + 1);
                            Vector end = (line, column - top_stack + 1);

                            Collider collider = new(start, end);
                            collider.Parent = this;
                        }
                    }
                }
                #endregion
            }
        }
    }
}
