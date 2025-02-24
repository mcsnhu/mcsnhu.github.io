using Enhancement1.Core;
using MoonWorks;
using MoonWorks.Graphics;

// Define the basic application info
AppInfo appInfo = new AppInfo("", "Enhancement1");
WindowCreateInfo windowCreateInfo = new WindowCreateInfo("Enhancement1", 1920, 1080, ScreenMode.Windowed);
FramePacingSettings framePacingSettings = FramePacingSettings.CreateCapped(60, 60);
ShaderFormat shaderFormat = ShaderFormat.SPIRV | ShaderFormat.DXIL | ShaderFormat.DXBC;

// Create the application instance and run
Engine engine = new Engine(appInfo, windowCreateInfo, framePacingSettings, shaderFormat, false);
engine.Run();