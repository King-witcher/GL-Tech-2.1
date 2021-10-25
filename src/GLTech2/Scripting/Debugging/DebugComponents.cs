namespace GLTech2.Behaviours
{
    public class DebugComponents : Behaviour
    {
        public float Interval { get; set; } = 1f;

        public bool DebugPosition { get; set; } = true;

        public bool DebugRotation { get; set; } = true;

        public bool DebugNormal { get; set; } = false;

        float timeSpent;

        void OnFrame()
        {
            timeSpent += Frame.DeltaTime;

            if (timeSpent >= Interval)
            {
                if (DebugPosition)
                    Debug.Log($"Relative position: {Entity.RelativePosition}");

                if (DebugRotation)
                    Debug.Log($"Relative rotation: {Entity.RelativeRotation}");

                if (DebugNormal)
                    Debug.Log($"Relative normal: {Entity.RelativeDirection}");

                Debug.Log();

                timeSpent = 0f;
            }
        }
    }
}
