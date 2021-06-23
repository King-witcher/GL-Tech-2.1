using System.Collections.Generic;

namespace GLTech2.PrefabElements
{
    /// <summary>
    /// Provides a tool to build maps based on a image.
    /// <remarks>
    /// Each pixel of the image can be converted into a block of its original color, a block with an especific Texture or nothing.
    /// </remarks>
    /// </summary>
	class GridMap : Element
	{
        private protected override Vector AbsolutePosition { get; set; } = Vector.Origin;
        private protected override Vector AbsoluteNormal { get; set; } = Vector.Forward;

        private int walls = 0;

        /// <summary>
        /// How many walls does this GridMap contain.
        /// </summary>
        public int Walls => walls;

        /// <summary>
        /// The scale of this GridMap. Cannot be zero.
        /// </summary>
        public float Scale
		{
            get => Normal.Module;
            set
			{
                if (value != 0)
				{
                    Normal = Normal * value / Normal.Module;
                }
			}
		}

        /// <summary>
        /// Gets a new instance of GridMap based only on a PixelBuffer.
        /// <remarks>
        /// Totally black pixels won't be
        /// </remarks>
        /// </summary>
        /// <param name="map">A pixelbuffer that represents each block of the map.</param>
        public GridMap(PixelBuffer map)
        {
            Dictionary<RGB, Texture> textures = new Dictionary<RGB, Texture>();

            // Gets a new texture if exists, otherwise creates it.
            Texture getTexture(RGB rgb)
            {
                if (textures.TryGetValue(rgb, out Texture texture))
                    return texture;
                else
                {
                    PixelBuffer buffer = new PixelBuffer(1, 1);     // Must be added to disposables
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
                    if (map[column, line] == (RGB)(0, 0, 0))
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

        /// <summary>
        /// Gets a new instance of GridMap the remaps each color to a given texture.
        /// </summary>
        /// <param name="map">The map</param>
        /// <param name="textures">A dictionary the links each pixel color to a texture</param>
        public GridMap(PixelBuffer map, IDictionary<RGB, Texture> textures)
        {
            for (int column = 0; column < map.Width; column++)
            {
                for (int line = 0; line < map.Height; line++)
                {
                    if (textures.TryGetValue(map[column, line], out Texture texture))
                    {
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
        }
    }
}
