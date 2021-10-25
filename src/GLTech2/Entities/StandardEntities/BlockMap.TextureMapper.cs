using System.Collections.Generic;

using GLTech2.Drawing; 

namespace GLTech2.Entities.StandardEntites
{
    partial class BlockMap
    {
        public class TextureMapper
        {
            // Fiz isso pra que o usuário seja capaz de programar utilizando somente a minha biblioteca, mas ainda posso otimizar o funcionamento disso futuramente, implementando na mão.
            Dictionary<uint, Texture> mapper;

            public TextureMapper() =>
                mapper = new Dictionary<uint, Texture>(32);

            public Texture this[RGB color]
            {
                get
                {
                    mapper.TryGetValue(color, out Texture texture);
                    return texture;
                }
                set => mapper[color] = value;
            }

            public void Bind(RGB color, Texture texture) =>
                mapper[color] = texture;

            public bool GetTexture(RGB color, out Texture texture) =>
                mapper.TryGetValue(color, out texture);
        }
    }
}
