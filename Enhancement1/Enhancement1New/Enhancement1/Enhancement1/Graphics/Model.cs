using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Enhancement1.Core;
using MoonTools.ECS.Collections;
using MoonWorks;
using MoonWorks.Graphics;
using Buffer = MoonWorks.Graphics.Buffer;

namespace Enhancement1.Graphics;

/// <summary>
/// This is an asset class, which stores a mesh from an .OBJ file
/// .OBJ was chosen for it's simplicity
/// </summary>
public unsafe sealed class Model : IDisposable
{
    public readonly Buffer VertexBuffer;
    public readonly Buffer IndexBuffer;
    public readonly uint IndexCount;
    private bool disposedValue;

    public Model(Engine Engine, ModelMetadata metadata, ResourceUploader resourceUploader)
    {
        // If the obj file exists
        if (Engine.RootTitleStorage.GetFileSize(metadata.Path, out var size))
        {
            // we allocate a chunk of memory necessary to store it on the heap
            var modelBytes = NativeMemory.Alloc((nuint)size);
            // to use it safely in the managed space we wrap it into a span
            var modelSpan = new Span<byte>(modelBytes, (int)size);

            // and then read the file from the root application storage into the span
            Engine.RootTitleStorage.ReadFile(metadata.Path, modelSpan);

            // convert the read bytes to string and split it by lines
            string file = Encoding.UTF8.GetString(modelSpan);
            List<string> objFile = file.Split('\n').ToList();

            // declare lists to store vertex attributes
            List<Vector3> positions = new();
            List<Vector3> normals = new();
            List<Vector2> texCoords = new();
            // as well as an acceleration structure to store faces
            List<(int vertID, int texCoordID, int normalID)> faceBuffer = new();


            for (int i = 0; i < objFile.Count; i++)
            {
                string[] splitComment = objFile[i].Split("#");
                // skip line if it's a comment
                if (splitComment[0] == string.Empty)
                {
                    continue;
                }

                // split the current line into tokens
                List<string> tokens = splitComment[0].Split(" ").ToList();
                switch (tokens[0])
                {
                    // vertex
                    case "v":
                    {
                        positions.Add(new Vector3(
                            float.Parse(tokens[1]),
                            float.Parse(tokens[2]),
                            float.Parse(tokens[3])
                            ));
                        break;
                    }
                    // vertex normal
                    case "vn":
                    {
                        normals.Add(new Vector3(
                            float.Parse(tokens[1]),
                            float.Parse(tokens[2]),
                            float.Parse(tokens[3])
                            ));
                        break;
                    }
                    // texture coordinate
                    case "vt":
                    {
                        texCoords.Add(new Vector2(
                            float.Parse(tokens[1]),
                            1f - float.Parse(tokens[2])
                            ));
                        break;
                    }
                    // face
                    case "f":
                    {

                        for (int j = 1; j < tokens.Count; j++)
                        {
                            string[] faceTokens = tokens[j].Split("/");
                            faceBuffer.Add(new(
                                    int.Parse(faceTokens[0]) - 1,
                                    int.Parse(faceTokens[1]) - 1,
                                    int.Parse(faceTokens[2]) - 1)
                                    );
                        }
                        break;
                    }
                    // skip other tokens
                    case "mtllib":
                    case "usemtl":
                    case "o":
                    case "s":
                    {
                        break;
                    }
                    // in case the file is malformed log a warning
                    default:
                    {
                        Logger.LogWarn($"Unknown token \"{tokens[0]}\" at line {i + 1}, skipping...");
                        break;
                    }
                }
            }

            List<PositionTextureNormalVertex> vertices = [];
            List<uint> indices = [];

            // parse the obj data and convert it into two lists containing vertex data and index data
            for (int i = 0; i < faceBuffer.Count; i++)
            {
                vertices.Add(
                    new PositionTextureNormalVertex()
                    {
                        Position = positions[faceBuffer[i].vertID],
                        TexCoord = texCoords[faceBuffer[i].texCoordID],
                        Normal = normals[faceBuffer[i].normalID]
                    });
                indices.Add((uint)i);
            }

            // upload the data to the buffers using a resource uploader
            VertexBuffer = resourceUploader.CreateBuffer<PositionTextureNormalVertex>(metadata.Name + "Vertex", CollectionsMarshal.AsSpan(vertices), BufferUsageFlags.Vertex);
            IndexBuffer = resourceUploader.CreateBuffer<uint>(metadata.Name + "Index", CollectionsMarshal.AsSpan(indices), BufferUsageFlags.Index);
            IndexCount = (uint)indices.Count;
            resourceUploader.Upload();

            // perform clean up
            positions.Clear();
            positions = null;
            texCoords.Clear();
            texCoords = null;
            normals.Clear();
            normals = null;
            vertices.Clear();
            vertices = null;
            indices.Clear();
            indices = null;
            NativeMemory.Free(modelBytes);
        }
    }

    // Buffers require to be disposed after use
    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                VertexBuffer.Dispose();
                IndexBuffer.Dispose();
            }

            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~Model()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
