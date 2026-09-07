namespace GLTech.Scripting.Debugging
{
    public class PerformanceDebugger : Script
    {
        int frameCount;
        float frameTime;
        float renderTime;
        float scriptTime;
        float presentTime;
        float wallCullTime;
        float wallRenderTime;

        float accumulator; // Just for showing buckets

        public bool ShowFrameTime { get; set; } = true;
        public bool ShowRenderTime { get; set; } = true;
        public bool ShowScriptTime { get; set; } = true;
        public bool ShowPresentTime { get; set; } = true;
        public float Bucket { get; set; } = 0.2f;

        private List<string> logs = new(4);

        [ScriptUpdate]
        public void Update()
        {
            frameCount++;
            accumulator += Time.TimeStep;
            frameTime += Time.TimeStep;
            renderTime += Debug.RenderTime;
            scriptTime += Debug.ScriptTime;
            presentTime += Debug.PresentTime;
            wallCullTime += Debug.WallCullTime;
            wallRenderTime += Debug.WallRenderTime;

            if (accumulator >= Bucket)
            {
                accumulator -= Bucket;

                logs.Clear();

                var avgFrameTime = frameTime / frameCount;
                var fps = 1f / avgFrameTime;
                logs.Add($"{fps:F0} FPS | {avgFrameTime * 1000f:F2}ms");
                logs.Add($"  render: {100f * renderTime / frameTime:F1}%");
                logs.Add($"    walls:");
                logs.Add($"      visible: {Debug.VisibleWalls}");
                logs.Add($"      cull: {100f * wallCullTime / renderTime:F1}%");
                logs.Add($"      render: {100f * wallRenderTime / renderTime:F1}%");
                logs.Add($"  present: {100f * presentTime / frameTime:F1}%");
                logs.Add($"  script (delayed): {100f * scriptTime / frameTime:F1}%");

                frameCount = 0;
                frameTime = 0f;
                renderTime = 0f;
                scriptTime = 0f;
                presentTime = 0f;
                wallCullTime = 0f;
                wallRenderTime = 0f;
            }

            foreach (var log in logs)
            {
                Debug.Log(log);
            }
        }
    }
}
