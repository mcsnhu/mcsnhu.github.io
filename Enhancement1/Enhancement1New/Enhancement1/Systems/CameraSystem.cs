using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Enhancement1.Core;
using Enhancement1.Data;
using MoonTools.ECS;
using MoonWorks.Input;

namespace Enhancement1.Systems;

/// <summary>
/// This system controls the camera movement.
/// </summary>
public sealed class CameraSystem : Core.System
{

    int previousX = 0;
    int previousY = 0;

    public CameraSystem(Engine engine) : base(engine)
    {
    }

    public override void Initialize()
    {
        Vector3 cameraPosition = Vector3.Zero;
        Vector3 cameraRotation = new(-45, 0, -90);
        float zoom = 3f;

        float aspect = Engine.Renderer.Aspect;

        Matrix4x4 viewMatrix =
              Matrix4x4.CreateFromYawPitchRoll(cameraRotation.Y, cameraRotation.X, cameraRotation.Z)
            * Matrix4x4.CreateTranslation(cameraPosition);

        // We're using a simple orthographic projection
        Matrix4x4 projectionMatrix = Matrix4x4.CreateOrthographicOffCenter(-zoom * aspect, zoom * aspect, -zoom, zoom, -300, 300);

        // Create the camera entity and set the initial data
        Entity camera = World.CreateEntity();
        World.Set(camera, new Camera(cameraPosition, cameraRotation, zoom, viewMatrix, projectionMatrix));
    }

    public override void Update(TimeSpan delta)
    {
        var Mouse = Engine.Inputs.Mouse;

        // Get a reference to the camera component.
        ref var camera = ref World.GetSingleton<Camera>();

        // Control the zoom with the mouse wheel
        if (Mouse.Wheel != 0)
        {
            camera = camera with
            {
                Zoom = camera.Zoom - 0.1f * Mouse.Wheel
            };
        }

        float aspect = Engine.Renderer.Aspect;

        // Move the camera using the left mouse button
        if (Mouse.LeftButton.IsDown)
        {
            float deltaX = previousX - Mouse.X;
            float deltaY = previousY - Mouse.Y;

            camera = camera with
            {
                Position = camera.Position + new Vector3(-deltaX / 100f / aspect, deltaY / 100f / aspect, 0)
            };
        }

        // Rotate the camera using the right mouse button
        if (Mouse.RightButton.IsDown)
        {
            float deltaX = previousX - Mouse.X;
            float deltaY = previousY - Mouse.Y;

            camera = camera with
            {
                Rotation = camera.Rotation + new Vector3(deltaY / 1000f, 0, deltaX / 1000f)
            };
        }

        previousX = Mouse.X;
        previousY = Mouse.Y;

        // Update the camera component with the new data
        camera = camera with
        {
            ViewMatrix = Matrix4x4.CreateFromYawPitchRoll(camera.Rotation.Y, camera.Rotation.X, camera.Rotation.Z)
                       * Matrix4x4.CreateTranslation(camera.Position),
            ProjectionMatrix = Matrix4x4.CreateOrthographicOffCenter(-camera.Zoom * aspect, camera.Zoom * aspect, -camera.Zoom, camera.Zoom, -300, 300)
        };
    }
}
