using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Enhancement1.Core;
using Enhancement1.Data;
using MoonTools.ECS;
using MoonWorks.Input;

namespace Enhancement1.Systems;

/// <summary>
/// This system controls the lighting.
/// </summary>
internal class LightingSystem : Core.System
{
    public LightingSystem(Engine engine) : base(engine)
    {
    }

    public override void Initialize()
    {
        // Create the directional light and set the initial data
        Entity directionalLight = World.CreateEntity();
        Tag(directionalLight, "DirectionalLight");
        Set(directionalLight, new DirectionalLight()
        {
            Direction = new(0.5f, 1, 1),
            Color = new Vector3(1.0f, 1.0f, 1.0f),
            Intensity = 1f,
            AmbientColor = new Vector3(1.0f, 0.1f, 0.1f),
            AmbientThreshold = 0.1f,
        });
    }
    public override void Update(TimeSpan delta)
    {
        // Get a reference to the DirectionalLight component
        ref var directionalLight = ref GetSingleton<DirectionalLight>();

        // to avoid inconsistent behavior we store inputs as axes
        // X - rotation around Z in world space
        // Y - rotation around Y in world space
        // Z - light intensity
        Vector3 input = Vector3.Zero;

        // rotate around Z
        if (Engine.Inputs.Keyboard.IsDown(KeyCode.A))
        {
            input.X -= 1;
        }

        // rotate around Z
        if (Engine.Inputs.Keyboard.IsDown(KeyCode.D))
        {
            input.X += 1;
        }

        // rotate around Y
        if (Engine.Inputs.Keyboard.IsDown(KeyCode.W))
        {
            input.Y -= 1;
        }

        // rotate around Y
        if (Engine.Inputs.Keyboard.IsDown(KeyCode.S))
        {
            input.Y += 1;
        }

        // decrease light intensity
        if (Engine.Inputs.Keyboard.IsDown(KeyCode.Q))
        {
            input.Z -= 1;
        }

        // increase light intensity
        if (Engine.Inputs.Keyboard.IsDown(KeyCode.E))
        {
            input.Z += 1;
        }

        // create a transformation matrix to rotate the light with
        Matrix4x4 transform = Matrix4x4.CreateRotationZ(float.DegreesToRadians((float)delta.TotalSeconds * 20f) * input.X)
                            * Matrix4x4.CreateRotationY(float.DegreesToRadians((float)delta.TotalSeconds * 20f) * input.Y);

        // apply changes
        directionalLight = directionalLight with
        {
            Direction = Vector3.Transform(directionalLight.Direction, transform),
            Intensity = float.Clamp(directionalLight.Intensity + 0.5f * (float)delta.TotalSeconds * input.Z, 0, 1)
        };

        // in case the scene is being respawned reset the light
        if (Engine.Inputs.Keyboard.IsPressed(KeyCode.R))
        {
            directionalLight = directionalLight with
            {
                Direction = new(0.5f, 1, 1),
                Color = new Vector3(1.0f, 1.0f, 1.0f),
                AmbientThreshold = 0.1f,
                AmbientColor = new Vector3(0.9f, 0.9f, 1.0f),
                Intensity = 1.0f
            };
        }
    }
}
