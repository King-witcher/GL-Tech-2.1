namespace GLTech2.Behaviours
{
    /// <summary>
    /// Debugs once how many walls are in the scene and then stops.
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
