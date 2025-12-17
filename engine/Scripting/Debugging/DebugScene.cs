
namespace Engine.Scripting.Debugging
{
    public class DebugScene : Script
    {
        private Logger logger = new Logger(typeof(DebugScene).Name);
        void Start()
        {
            logger.Log($"Elements: {Scene.EntityCount}");
            logger.Log($"Planes: {Scene.PlaneCount}");
            logger.Log($"Colliders: {Scene.ColliderCount}");
        }
    }
}
