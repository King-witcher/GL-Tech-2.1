// Otimizável

using System;

using GLTech2.Imaging;

namespace GLTech2.Entities.StandardEntites
{
    public partial class BlockMap : Entity
    {
        private protected override Vector PositionData { get; set; } = Vector.Zero;
        private protected override Vector DirectionData { get; set; } = Vector.Forward;

        private int walls = 0;

        public int Walls => walls;

        [Obsolete]
        public BlockMap(ImageData map)
        {
            TextureMapper textures = new TextureMapper();


            // Gets a new texture if exists; otherwise creates it.
            Texture getTexture(Pixel rgb)
            {
                if (textures.GetTexture(rgb, out Texture texture))
                    return texture;
                else
                {
                    ImageData buffer = new ImageData(1, 1);     // Must be added to disposables
                    buffer[0, 0] = rgb;
                    Texture tex = new Texture(buffer);
                    textures[rgb] = tex;
                    return tex;
                }
            }

            for (int column = 0; column < map.Width; column++)
            {
                for (int line = 0; line < map.Height; line++)
                {
                    // Checks for transparency.
                    if (map[column, line] == (Pixel)(0, 0, 0))
                        continue;

                    Texture texture = getTexture(map[column, line]);

                    Vector vert1 = (line, column);
                    Vector vert2 = (line + 1, column);
                    Vector vert3 = (line + 1, column + 1);
                    Vector vert4 = (line, column + 1);

                    new Wall(vert1, vert2, texture).Parent = this;
                    new Wall(vert2, vert3, texture).Parent = this;
                    new Wall(vert3, vert4, texture).Parent = this;
                    new Wall(vert4, vert1, texture).Parent = this;

                    walls += 4;
                }
            }
        }

        public BlockMap(ImageData map, TextureMapper textureBindings)
        {
            // Cada bloco, em tese, geraria quatro paredes e quatro colisores ao seu redor.
            // Esse método foi otimizado considerando-se que, se duas paredes forem desenhadas sobre o mesmo lugar, isso significa que não existe superfície visível por ali e, portanto, nenhuma das duas deverá ser realmente renderizada.
            bool blockFree(int column, int line)
            {
                if (column < 0 || column >= map.Height || line < 0 || line >= map.Width)
                    return true;
                if (textureBindings.GetTexture(map[column, line], out _))
                    return false;
                return true;
            }

            if (textureBindings == null)
                throw new System.ArgumentNullException("textureBindings");

            for (int column = 0; column < map.Width; column++)
            {
                for (int line = 0; line < map.Height; line++)
                {
                    if (textureBindings.GetTexture(map[column, line], out Texture texture))
                    {
                        Vector vert1 = (line, column);
                        Vector vert2 = (line + 1, column);
                        Vector vert3 = (line + 1, column + 1);
                        Vector vert4 = (line, column + 1);

                        if (blockFree(column - 1, line))
                        {
                            var plane = new Plane(vert1, vert2, texture);
                            var collider = new Collider(plane);
                            plane.Parent = collider.Parent = this;
                            walls++;
                        }
                        if (blockFree(column, line + 1))
                        {
                            var plane = new Plane(vert2, vert3, texture);
                            var collider = new Collider(plane);
                            plane.Parent = collider.Parent = this;
                            walls++;
                        }
                        if (blockFree(column + 1, line))
                        {
                            var plane = new Plane(vert3, vert4, texture);
                            var collider = new Collider(plane);
                            plane.Parent = collider.Parent = this;
                            walls++;
                        }
                        if (blockFree(column, line - 1))
                        {
                            var plane = new Plane(vert4, vert1, texture);
                            var collider = new Collider(plane);
                            plane.Parent = collider.Parent = this;
                            walls++;
                        }
                    }
                }
            }
        }
    }
}
