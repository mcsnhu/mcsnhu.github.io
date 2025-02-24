using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Enhancement1.Data;
using static MoonWorks.Graphics.VertexStructs;

namespace Enhancement1.Graphics;

//
// Default pipeline uniforms
//

public struct VertexUniforms
{
    public Matrix4x4 ViewProjectionMatrix;
    public Matrix4x4 ModelMatrix;
    public Matrix4x4 NormalMatrix;
}

public struct FragmentUniforms
{
    public DirectionalLight MainLight;
    public Vector3 CameraPosition;
}
