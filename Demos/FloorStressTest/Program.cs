using GLTech;
using GLTech.Demos.FloorStressTest;

Map map = new();

var createInfo = new GameEngineCreateInfo();
createInfo.FullScreen = true;
createInfo.WindowWidth = 1920;
createInfo.WindowHeight = 1080;
createInfo.MaxFPS = 5000;

var engine = new GLTech.GameEngine(createInfo);

engine.CaptureMouse = true;
engine.Run(map);
