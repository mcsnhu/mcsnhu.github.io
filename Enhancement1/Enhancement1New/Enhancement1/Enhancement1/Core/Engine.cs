using System.Numerics;
using Enhancement1.Data;
using Enhancement1.Graphics;
using Enhancement1.Systems;
using MoonTools.ECS;
using MoonWorks;
using MoonWorks.Graphics;
using MoonWorks.Input;
using MoonWorks.Math;
using SDL3;

namespace Enhancement1.Core;

/// <summary>
/// The engine is the main class which controls the flow of the application.
/// </summary>
public sealed class Engine : Game
{
    public World World { get; private set; }
    public Graphics.Renderer Renderer { get; private set; }
    public AssetDatabase AssetDatabase { get; private set; }
    public Resources Resources { get; private set; }

    public readonly Entity Root;

    SceneController SceneController;
    LightingSystem LightingSystem;
    CameraSystem CameraSystem;
    TransformSystem TransformSystem;


    public Engine(AppInfo appInfo,
                  WindowCreateInfo windowCreateInfo,
                  FramePacingSettings framePacingSettings,
                  ShaderFormat availableShaderFormats,
                  bool debugMode = false) : base(appInfo, windowCreateInfo, framePacingSettings, availableShaderFormats, debugMode)
    {
        // This would set the window to be always on top, useful for hot reload and prototyping
        //SDL.SDL_SetWindowAlwaysOnTop(MainWindow.Handle, true);

        // Create the world, which holds all of the entities and filters
        World = new World();

        // Spawn the root of the scene hierarchy
        Root = World.CreateEntity("RootEntity");
        World.Set(Root, new GlobalTransform(Matrix4x4.Identity));
        World.Set(Root, new LocalTransform(Matrix4x4.Identity));

        // Instantiate the necessary core classes
        Renderer = new Graphics.Renderer(this, World);
        AssetDatabase = new AssetDatabase(this);
        Resources = new Resources(this);

        // Load resources
        AssetDatabase.Load();

        // Instantiate systems and initialize them
        SceneController = new SceneController(this);
        LightingSystem = new LightingSystem(this);
        CameraSystem = new CameraSystem(this);
        TransformSystem = new TransformSystem(this);

        SceneController.Initialize();
        LightingSystem.Initialize();
        CameraSystem.Initialize();
        TransformSystem.Initialize();
    }


    /// <summary>
    /// This is the main update loop
    /// </summary>
    /// <param name="delta"></param>
    protected override void Update(TimeSpan delta)
    {
        // Quit upon pressing Escape
        if (Inputs.Keyboard.IsDown(KeyCode.Escape))
        {
            Quit();
        }

        // Update all systems
        SceneController.Update(delta);

        LightingSystem.Update(delta);
        CameraSystem.Update(delta);
        TransformSystem.Update(delta);
    }

    protected override void Draw(double alpha)
    {
        Renderer.Draw(alpha);
    }

    /// <summary>
    /// Cleanup function
    /// </summary>
    protected override void Destroy()
    {
        Renderer.Destroy();
        AssetDatabase.Destroy();
    }
}