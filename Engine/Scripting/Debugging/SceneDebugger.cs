
namespace GLTech.Scripting.Debugging
{
    public class SceneDebugger : Script
    {

        [ScriptUpdate]
        public void Update()
        {
            Debug.Log("scene stats:");
            Debug.Log($"  entities: {Scene.EntityCount}");
            Debug.Log($"  walls: {Scene.WallCount}");
            Debug.Log($"  colliders: {Scene.ColliderCount}");
        }
    }
}
