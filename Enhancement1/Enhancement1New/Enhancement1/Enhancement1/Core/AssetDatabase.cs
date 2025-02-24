using System.Runtime.InteropServices;
using Enhancement1.Graphics;
using MoonWorks;
using MoonWorks.Graphics;

namespace Enhancement1.Core;

/// <summary>
/// The storage class for multiple asset types.
/// Every asset has to be preregistered and then can be accessed by it's unique ID.
/// </summary>
public sealed class AssetDatabase
{
    readonly Engine Engine;
    readonly GraphicsDevice GraphicsDevice;

    List<TextureMetadata> TextureMetadata = [];
    List<ShaderMetadata> ShaderMetadata = [];
    List<GraphicsPipelineMetadata> GraphicsPipelineMetadata = [];
    List<SamplerMetadata> SamplerMetadata = [];
    List<ModelMetadata> ModelMetadata = [];

    int NextTextureID = 0;
    int NextShaderID = 0;
    int NextGraphicsPipelineID = 0;
    int NextSamplerID = 0;
    int NextModelID = 0;

    List<Texture> Textures = [];
    List<Shader> Shaders = [];
    List<GraphicsPipeline> GraphicsPipelines = [];
    List<Sampler> Samplers = [];
    List<Model> Models = [];

    public AssetDatabase(Engine engine)
    {
        Engine = engine;
        GraphicsDevice = engine.GraphicsDevice;
    }

    public TextureID RegisterTexture(TextureMetadata metadata)
    {
        TextureID textureID = new(NextTextureID);
        TextureMetadata.Add(metadata);
        NextTextureID++;
        return textureID;
    }

    public ShaderID RegisterShader(ShaderMetadata metadata)
    {
        ShaderID shaderID = new(NextShaderID);
        ShaderMetadata.Add(metadata);
        NextShaderID++;
        return shaderID;
    }

    public GraphicsPipelineID RegisterPipeline(GraphicsPipelineMetadata metadata)
    {
        GraphicsPipelineID graphicsPipelineID = new(NextGraphicsPipelineID);
        NextGraphicsPipelineID++;
        GraphicsPipelineMetadata.Add(metadata);
        return graphicsPipelineID;
    }

    public SamplerID RegisterSampler(SamplerMetadata metadata)
    {
        SamplerID samplerID = new(NextSamplerID);
        NextSamplerID++;
        SamplerMetadata.Add(metadata);
        return samplerID;
    }

    public ModelID RegisterModel(ModelMetadata metadata)
    {
        ModelID modelID = new(NextModelID);
        NextModelID++;
        ModelMetadata.Add(metadata);
        return modelID;
    }

    public Texture GetTexture(TextureID textureID) => Textures[textureID.ID];
    public Shader GetShader(ShaderID shaderID) => Shaders[shaderID.ID];
    public GraphicsPipeline GetGraphicsPipeline(GraphicsPipelineID graphicsPipelineID) => GraphicsPipelines[graphicsPipelineID.ID];
    public Sampler GetSampler(SamplerID samplerID) => Samplers[samplerID.ID];
    public Model GetModel(ModelID modelID) => Models[modelID.ID];

    public unsafe void Load()
    {
        // This class is used to transfer graphics resources to the GPU
        ResourceUploader resourceUploader = new ResourceUploader(GraphicsDevice);

        Logger.LogInfo("Loading textures...");
        foreach (var metadata in TextureMetadata)
        {
            Logger.LogInfo($"    {metadata.Path}");

            Engine.RootTitleStorage.GetFileSize(metadata.Path, out var textureSize);
            // allocate the necessary amount of bytes on the heap to store the texture
            var textureBytes = NativeMemory.Alloc((nuint)textureSize);
            // get a safe wrapper for the said bytes
            var textureSpan = new Span<byte>(textureBytes, (int)textureSize);
            // using the read-only storage abstraction fill the bytes with the texture data
            Engine.RootTitleStorage.ReadFile(metadata.Path, textureSpan);

            // create texture and add it to the list
            Texture texture = resourceUploader.CreateTexture2DFromCompressed(textureSpan, TextureFormat.R8G8B8A8Unorm, TextureUsageFlags.Sampler);
            Textures.Add(texture);
            resourceUploader.Upload();

            // perform cleanup
            NativeMemory.Free(textureBytes);
        }

        Logger.LogInfo("Compiling shaders...");
        foreach (var metadata in ShaderMetadata)
        {
            Logger.LogInfo($"    {metadata.Path}");

            Shader shader = ShaderCross.Create(
                GraphicsDevice,
                Engine.RootTitleStorage,
                metadata.Path,
                metadata.EntryPoint,
                ShaderCross.ShaderFormat.HLSL,
                metadata.ShaderStage
            );
            Shaders.Add(shader);
        }

        Logger.LogInfo("Creating graphics pipelines...");
        foreach (var metadata in GraphicsPipelineMetadata)
        {
            Logger.LogInfo($"    {metadata.Name}");

            GraphicsPipelineCreateInfo graphicsPipelineCreateInfo = metadata.GraphicsPipelineCreateInfo;
            graphicsPipelineCreateInfo.VertexShader = GetShader(metadata.VertexShaderID);
            graphicsPipelineCreateInfo.FragmentShader = GetShader(metadata.FragmentShaderID);
            GraphicsPipeline graphicsPipeline = GraphicsPipeline.Create(GraphicsDevice, graphicsPipelineCreateInfo);
            GraphicsPipelines.Add(graphicsPipeline);
        }

        Logger.LogInfo("Creating samplers...");
        foreach (var metadata in SamplerMetadata)
        {
            Logger.LogInfo($"    {metadata.Name}");

            Sampler sampler = Sampler.Create(GraphicsDevice, metadata.SamplerCreateInfo);
            Samplers.Add(sampler);
        }

        Logger.LogInfo("Loading models...");
        foreach (var metadata in ModelMetadata)
        {
            Logger.LogInfo($"    {metadata.Path}");

            Model model = new Model(Engine, metadata, resourceUploader);
            Models.Add(model);
        }

        // the resource uploader needs to be disposed as it has an internal transfer buffer
        resourceUploader.Dispose();


        GC.Collect();
    }

    // Dispose of all resources and clear the lists
    public void Destroy()
    {
        foreach (var texture in Textures)
        {
            texture.Dispose();
        }
        Textures.Clear();

        foreach (var shader in Shaders)
        {
            shader.Dispose();
        }
        Shaders.Clear();

        foreach (var pipeline in GraphicsPipelines)
        {
            pipeline.Dispose();
        }
        GraphicsPipelines.Clear();

        foreach (var sampler in Samplers)
        {
            sampler.Dispose();
        }
        Samplers.Clear();

        foreach (var model in Models)
        {
            model.Dispose();
        }
        Models.Clear();

    }
}

// All metadata and IDs are readonly record structs, which is almost equal to a plain C struct.
// The entity component system (ECS) paradigm relies on small unmanaged objects to keep everything simple and efficient.
// Therefore where we need a texture - we supply a simple ID for the texture, it is the responsibility of the storage 
// to provide the managed object which corresponds to the said ID.
public readonly record struct TextureMetadata(string Path);
public readonly record struct ShaderMetadata(string Path, string EntryPoint, ShaderStage ShaderStage);
public readonly record struct GraphicsPipelineMetadata(string Name, ShaderID VertexShaderID, ShaderID FragmentShaderID, GraphicsPipelineCreateInfo GraphicsPipelineCreateInfo);
public readonly record struct SamplerMetadata(string Name, SamplerCreateInfo SamplerCreateInfo);
public readonly record struct ModelMetadata(string Name, string Path);

public readonly record struct TextureID(int ID);
public readonly record struct ShaderID(int ID);
public readonly record struct GraphicsPipelineID(int ID);
public readonly record struct SamplerID(int ID);
public readonly record struct ModelID(int ID);