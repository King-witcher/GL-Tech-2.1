using Engine.Window;
using GLTech;
using GLTech.Demos.RotatingPillars;

Map map = new();

var window = new LegacyWindow("Rotating Pillars", 1920, 1080, true, false);
window.RelativeMouseMode = true;

var engineInfo = new GameEngineCreateInfo();
engineInfo.MaxFPS = 5000;

var engine = new GameEngine(engineInfo);

var renderer = new Renderer(90f, true);

engine.Run(window, renderer, map);
