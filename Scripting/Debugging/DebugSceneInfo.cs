
namespace Engine.Scripting.Debugging
{
    public class DebugSceneInfo : Script
    {
        void Start()
        {
            Debug.Log($"Scene info:");
            Debug.Log($"\tElements: {Scene.EntityCount}");
            Debug.Log($"\tPlanes: {Scene.PlaneCount}");
            Debug.Log($"\tColliders: {Scene.ColliderCount}");
            Debug.Log();
        }
    }
}
