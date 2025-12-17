using GLTech;
using wolf3d;

Map map = new();

var createInfo = new EngineCreateInfo();
createInfo.FullScreen = true;
createInfo.WindowWidth = 1920;
createInfo.WindowHeight = 1080;

var engine = new Engine(createInfo);

engine.CaptureMouse = true;
engine.Run(map);