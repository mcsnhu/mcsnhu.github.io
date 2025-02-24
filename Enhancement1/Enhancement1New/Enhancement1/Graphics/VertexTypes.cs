using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MoonWorks.Graphics;

namespace Enhancement1.Graphics;

// We define the layour explicitly to avoid padding issues
[StructLayout(LayoutKind.Explicit, Size = 48)]
struct PositionTextureNormalVertex : IVertexType
{
    [FieldOffset(0)]
    public Vector3 Position;

    [FieldOffset(16)]
    public Vector2 TexCoord;

    [FieldOffset(32)]
    public Vector3 Normal;

    public static VertexElementFormat[] Formats { get; } =
    [
        VertexElementFormat.Float3,
        VertexElementFormat.Float2,
        VertexElementFormat.Float3,
    ];

    public static uint[] Offsets { get; } =
    [
        0,
        16,
        32
    ];
}