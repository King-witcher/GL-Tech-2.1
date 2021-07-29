namespace GLTech2.Behaviours
{
    /// <summary>
    /// Debugs all info (elements, planes, colliders, etc.) about the corresponding scene on start.
    /// </summary>
    public class DebugSceneInfo : Behaviour
    {
        void Start()
        {
            Debug.Log($"Scene info:");
            Debug.Log($"\tElements: {Scene.ElementCount}");
            Debug.Log($"\tPlanes: {Scene.PlaneCount}");
            Debug.Log($"\tColliders: {Scene.ColliderCount}");
            Debug.Log();
        }
    }
}
