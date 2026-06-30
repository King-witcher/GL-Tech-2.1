using GLTech;
using StbImageSharp;

namespace Engine
{
    public static class ResourceManager
    {
        public static string BasePath { get; set; } = "base";

        public static string LoadText(string path)
        {
            return "";
        }

        public async static Task<byte[]> LoadBuffer(string path)
        {
            var bytes = await File.ReadAllBytesAsync($"{BasePath}/{path}");
            if (bytes is null)
            {
                throw new Exception($"Failed to load resource {path}");
            }
            return bytes;
        }

        public unsafe static TextureBuffer LoadTextureBuffer(string path)
        {
            using var stream = File.OpenRead($"{BasePath}/textures/{path}");
            var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
            fixed (byte* p = image.Data)
            {
                var texBuf = new TextureBuffer(image.Width, image.Height, (uint*)p);
                return texBuf;
            }
        }
    }
}
