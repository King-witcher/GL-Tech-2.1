namespace GLTech2.Behaviours
{
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
