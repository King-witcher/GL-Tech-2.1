// Otimizável

using System;

namespace GLTech2.Elements
{
	/// <summary>
	/// Provides a tool to build elements based on textures..
	/// <remarks>
	/// Each pixel of the image can be converted into a block of its original color, a block with an especific Texture or nothing.
	/// </remarks>
	/// </summary>
	public partial class BlockMap : Element
	{
		private protected override Vector PositionData { get; set; } = Vector.Zero;
		private protected override Vector DirectionData { get; set; } = Vector.Forward;

		private int walls = 0;

		/// <summary>
		/// How many walls does this BlockMap contain.
		/// </summary>
		public int Walls => walls;

        /// <summary>
        /// Gets a new instance of BlockMap based only on a PixelBuffer.
        /// <remarks>
        /// Totally black pixels won't be
        /// </remarks>
        /// </summary>
        /// <param name="map">A pixelbuffer that represents each block of the map.</param>
        [Obsolete]
		public BlockMap(PixelBuffer map)
		{
			TextureMapper textures = new TextureMapper();


			// Gets a new texture if exists; otherwise creates it.
			Texture getTexture(RGB rgb)
			{
				if (textures.GetTexture(rgb, out Texture texture))
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
		/// Gets a new instance of BlockMap the maps each color to a given texture.
		/// </summary>
		/// <param name="map">The map</param>
		/// <param name="textureBindings">A hashmap that binds a set of Colors to Textures</param>
		public BlockMap(PixelBuffer map, TextureMapper textureBindings)
		{
			bool blockFree(int column, int line)
			{
				if (column < 0 || column >= map.height || line < 0 || line >= map.width)
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
							new Plane(vert1, vert2, texture).Parent = this;
							new Collider(vert1, vert2).Parent = this;
							walls++;
						}
						if (blockFree(column, line + 1))
						{
							new Plane(vert2, vert3, texture).Parent = this;
							new Collider(vert2, vert3).Parent = this;
							walls++;
						}
						if (blockFree(column + 1, line))
						{
							new Plane(vert3, vert4, texture).Parent = this;
							new Collider(vert3, vert4).Parent = this;
							walls++;
						}
						if (blockFree(column, line - 1))
						{
							new Plane(vert4, vert1, texture).Parent = this;
							new Collider(vert4, vert1).Parent = this;
							walls++;
						}
					}
				}
			}
		}
	}
}
