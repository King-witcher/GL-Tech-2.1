using System.Collections.Generic;

namespace GLTech2.PrefabElements
{
    partial class GridMap
    {
        /// <summary>
        /// Represents a list of associations between colors and textures used to remap pixels from a grid to textures.
        /// </summary>
        public class TextureBindings
        {
            Dictionary<uint, Texture> bindings;

            /// <summary>
            /// Gets a new instance of TextureBindings.
            /// </summary>
            public TextureBindings() =>
                bindings = new Dictionary<uint, Texture>(32);

            /// <summary>
            /// Indexes each Color to its Texture.
            /// </summary>
            /// <param name="color">Color</param>
            /// <returns>The Texture, if found; otherwise, the default value of Texture.</returns>
            public Texture this[RGB color]
            {
                get
                {
                    bindings.TryGetValue(color, out Texture texture);
                    return texture;
                }
                set => bindings[color] = value;
            }

            /// <summary>
            /// Bind a new Color to a Texture.
            /// </summary>
            /// <param name="color">Color</param>
            /// <param name="texture">Texture</param>
            public void Bind(RGB color, Texture texture) =>
                bindings[color] = texture;

            /// <summary>
            /// Tries to get the Texture bound to a given Color and returns whether the texture was found or not.
            /// </summary>
            /// <param name="color">The possibly bound Color</param>
            /// <param name="texture">The Texture, if found; otherwise, the default value of Texture.</param>
            /// <returns>true if a Texture was found; otherise, false.</returns>
            public bool GetTexture(RGB color, out Texture texture) =>
                bindings.TryGetValue(color, out texture);
        }
    }
}
