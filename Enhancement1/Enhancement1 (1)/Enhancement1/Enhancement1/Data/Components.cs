using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Enhancement1.Core;

namespace Enhancement1.Data;

//
// Default components
//

public readonly record struct Camera(Vector3 Position, Vector3 Rotation, float Zoom, Matrix4x4 ViewMatrix, Matrix4x4 ProjectionMatrix);

public readonly record struct Position(float X, float Y, float Z)
{
    public Vector3 ToVector3() => new(X, Y, Z);
}

public readonly record struct Rotation(float X, float Y, float Z)
{
    public Vector3 ToVector3() => new(X, Y, Z);
}

public readonly record struct Scale(float X, float Y, float Z)
{
    public Vector3 ToVector3() => new(X, Y, Z);
}

public readonly record struct GlobalTransform(Matrix4x4 Matrix);
public readonly record struct LocalTransform(Matrix4x4 Matrix);
public readonly record struct TransformDirty();

public readonly record struct Mesh(ModelID ModelID);
public readonly record struct Material(GraphicsPipelineID PipelineID, TextureID DiffuseTextureID);


public readonly record struct Spawned();

public readonly record struct DirectionalLight(Vector3 Direction, float Intensity, Vector3 Color, float AmbientThreshold, Vector3 AmbientColor);
