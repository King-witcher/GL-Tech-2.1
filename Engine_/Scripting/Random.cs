
namespace GLTech.Scripting
{
    public static class Random
    {
        static System.Random random = new();

        public static double GetDouble(double min = 0.0, double max = 1.0)
        {
            return random.NextDouble() * (max - min) + min;
        }

        public static float GetFloat(float min = 0f, float max = 1f)
        {
            return random.NextSingle() * (max - min) + min;
        }

        public static int GetInt(int min, int max)
        {
            return random.Next(min, max);
        }
    }
}
