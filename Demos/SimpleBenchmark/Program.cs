using GLTech;
using GLTech.Demos.SimpleBenchmark;

Map map = new();

var createInfo = new EngineCreateInfo();
createInfo.FullScreen = true;
createInfo.WindowWidth = 1920;
createInfo.WindowHeight = 1080;
createInfo.MaxFPS = 5000;

var engine = new GLTech.Engine(createInfo);

engine.CaptureMouse = true;
engine.Run(map);
