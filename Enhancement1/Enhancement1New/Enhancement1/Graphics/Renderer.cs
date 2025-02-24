using System.Numerics;
using Enhancement1.Core;
using Enhancement1.Data;
using MoonTools.ECS;
using MoonWorks;
using MoonWorks.Graphics;

namespace Enhancement1.Graphics;

public sealed class Renderer : MoonTools.ECS.Renderer
{
    readonly Engine Engine;
    readonly GraphicsDevice GraphicsDevice;
    Window Window;

    Texture RenderTexture;
    Texture DepthTexture;

    MoonTools.ECS.Filter RenderFilter;

    public float Aspect { get; private set; }

    public Renderer(Engine engine, World world) : base(world)
    {
        Engine = engine;
        GraphicsDevice = engine.GraphicsDevice;
        Window = Engine.MainWindow;

        CreateRenderTextures(Window.Width, Window.Height);
        // It is important to handle swapchain size changes
        Engine.MainWindow.RegisterSizeChangeCallback(OnResizeSwapchain);

        // Only process entities that have the necessary components
        RenderFilter = FilterBuilder.Include<GlobalTransform>().Include<Mesh>().Include<Material>().Build();
    }


    public void OnResizeSwapchain(uint width, uint height)
    {
        Logger.LogInfo($"Resizing window to [{width}:{height}].");
        CreateRenderTextures(width, height);
        // we need the aspect for a proper projection matrix
        Aspect = width / (float)height;
    }

    public void CreateRenderTextures(uint width, uint height)
    {
        Logger.LogInfo("Creating render textures...");

        // Dispose old textures
        if (RenderTexture != null && !RenderTexture.IsDisposed)
        {
            RenderTexture.Dispose();
            RenderTexture = null;
        }

        if (DepthTexture != null && !DepthTexture.IsDisposed)
        {
            DepthTexture.Dispose();
            DepthTexture = null;
        }

        // Create a new set of render and depth textures
        RenderTexture = Texture.Create2D(GraphicsDevice, width, height, Window.SwapchainFormat, TextureUsageFlags.ColorTarget | TextureUsageFlags.Sampler);
        DepthTexture = Texture.Create2D(GraphicsDevice, width, height, GraphicsDevice.SupportedDepthFormat, TextureUsageFlags.DepthStencilTarget);
    }

    public void Draw(double alpha)
    {
        // All the drawing occurs in this function

        // the command buffer is responsible for communicating the instructions to the GPU
        var commandBuffer = GraphicsDevice.AcquireCommandBuffer();

        // get the swapchain texture (the window texture)
        var swapchainTexture = commandBuffer.AcquireSwapchainTexture(Window);

        if (swapchainTexture != null)
        {
            // we begin the drawing by initiating a render pass
            RenderPass renderPass = commandBuffer.BeginRenderPass(
                new DepthStencilTargetInfo(DepthTexture, 1.0f, true),
                new ColorTargetInfo(RenderTexture, Color.Transparent, true)
            );

            // get constant references to the camera and the main light
            ref readonly var camera = ref GetSingleton<Camera>();
            ref readonly var mainLight = ref GetSingleton<DirectionalLight>();

            // process every entity
            foreach (var entity in RenderFilter.Entities)
            {
                // get constant references to the relevant components
                ref readonly var GlobalTransform = ref Get<GlobalTransform>(entity);
                ref readonly var Mesh = ref Get<Mesh>(entity);
                ref readonly var Material = ref Get<Material>(entity);

                // retrieve assets from the database
                Model model = Engine.AssetDatabase.GetModel(Mesh.ModelID);
                GraphicsPipeline pipeline = Engine.AssetDatabase.GetGraphicsPipeline(Material.PipelineID);
                Texture texture = Engine.AssetDatabase.GetTexture(Material.DiffuseTextureID);
                Sampler sampler = Engine.AssetDatabase.GetSampler(Engine.Resources.PointWrapSampler);

                // normal matrix is required to transform the vertex normals into the world space,
                // since the calculations in the fragment shader are in world space
                Matrix4x4.Invert(GlobalTransform.Matrix, out var inverse);
                Matrix4x4 NormalMatrix = Matrix4x4.Transpose(inverse);

                // fill in and push the uniform data
                commandBuffer.PushVertexUniformData(
                    new VertexUniforms()
                    {
                        ViewProjectionMatrix = camera.ViewMatrix * camera.ProjectionMatrix,
                        ModelMatrix = GlobalTransform.Matrix,
                        NormalMatrix = NormalMatrix,
                    }
                );

                commandBuffer.PushFragmentUniformData(
                    new FragmentUniforms()
                    {
                        MainLight = mainLight,
                        CameraPosition = camera.Position,
                    }
                );

                // bind the necessary graphics resources
                renderPass.BindVertexBuffers(model.VertexBuffer);
                renderPass.BindIndexBuffer(model.IndexBuffer, IndexElementSize.ThirtyTwo);
                renderPass.BindGraphicsPipeline(pipeline);
                renderPass.BindFragmentSamplers(new TextureSamplerBinding(texture, sampler));

                // perform the draw call
                renderPass.DrawIndexedPrimitives(model.IndexCount, 1, 0, 0, 0);
            }

            // end the render pass
            commandBuffer.EndRenderPass(renderPass);

            // blit (copy) the contents of the render texture to the swapchain texture
            commandBuffer.Blit(RenderTexture, swapchainTexture, MoonWorks.Graphics.Filter.Nearest);
        }

        // up until now all the code was mere instructions, this is where we send them to the GPU and it wil get scheduled for execution
        GraphicsDevice.Submit(commandBuffer);
    }

    // Cleanup
    public void Destroy()
    {
        RenderTexture.Dispose();
        DepthTexture.Dispose();
    }
}