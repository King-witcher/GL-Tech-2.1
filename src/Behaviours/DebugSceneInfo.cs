namespace GLTech2.PrefabBehaviours
{
    /// <summary>
    /// Debugs once how many walls are in the scene and then stops.
    /// </summary>
    public class DebugSceneInfo : Behaviour
    {
        void Start()
        {
            Debug.Log($"Walls: {Scene.WallCount}");
            Debug.Log();
        }
    }
}
