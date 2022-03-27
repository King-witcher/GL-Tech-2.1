
using System.Collections.Generic;

using Engine.Imaging; 

namespace Engine.World.Prefab
{

    partial class BlockMap
    {

        public class TextureMapper
        {
            // Fiz isso pra que o usuário seja capaz de programar utilizando somente a minha biblioteca, mas ainda posso otimizar o funcionamento disso futuramente, implementando na mão.
            Dictionary<uint, Texture> mapper;

            public TextureMapper() =>
                mapper = new Dictionary<uint, Texture>(32);

            public Texture this[Color color]
            {
                get
                {
                    mapper.TryGetValue(color, out Texture texture);
                    return texture;
                }
                set => mapper[color] = value;
            }

            public void Bind(Color color, Texture texture) =>
                mapper[color] = texture;

            public bool GetTexture(Color color, out Texture texture) =>
                mapper.TryGetValue(color, out texture);
        }
    }
}
