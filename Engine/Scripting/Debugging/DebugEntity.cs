
namespace GLTech.Scripting.Debugging
{
    public class DebugEntity : Script
    {
        private static Logger logger = new(typeof(DebugEntity).Name);
        public float Interval { get; set; } = 1f;

        public bool DebugPosition { get; set; } = true;

        public bool DebugRotation { get; set; } = true;

        public bool DebugNormal { get; set; } = false;

        float timeSpent;

        void OnFrame()
        {
            timeSpent += Time.TimeStep;

            if (timeSpent >= Interval)
            {
                if (DebugPosition)
                    logger.Log($"Relative position: {Entity.RelativePosition}");

                if (DebugRotation)
                    logger.Log($"Relative rotation: {Entity.RelativeRotation}");

                if (DebugNormal)
                    logger.Log($"Relative normal: {Entity.RelativeDirection}");

                timeSpent = 0f;
            }
        }
    }
}
