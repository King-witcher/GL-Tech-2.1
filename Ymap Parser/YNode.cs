using System.Collections.Generic;

namespace Engine.Ymap_Parser
{
    internal class YNode
    {
        public YNode(string key)
        {
            this.key = key;
        }

        public readonly string key;
        public string value;
        public List<YNode> children;
    }

    internal static class Func
    {
        public static YNode ReadString(string data)
        {
            YNode root = new YNode("root");

            return null;
        }
    }
}
