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
/// This system calculates an entity's global transform (model matrix).
/// </summary>
public sealed class TransformSystem : Core.System
{
    Filter LocalTransformFilter;
    public TransformSystem(Engine engine) : base(engine)
    {
        // Cache entities in a filter. This will only have entities that have spawned/moved this frame.
        // An object won't be included if it failed at least one component check.
        LocalTransformFilter = FilterBuilder.Include<Position>()
                                            .Include<Rotation>()
                                            .Include<Scale>()
                                            .Include<LocalTransform>()
                                            .Include<TransformDirty>()
                                            .Build();
    }

    public override void Initialize()
    {
    }

    public override void Update(TimeSpan delta)
    {
        // calculate the local transform for every matching entity
        foreach (var entity in LocalTransformFilter.Entities)
        {
            ref readonly Position position = ref Get<Position>(entity);
            ref readonly Rotation rotation = ref Get<Rotation>(entity);
            ref readonly Scale scale = ref Get<Scale>(entity);
            ref LocalTransform localTransform = ref Get<LocalTransform>(entity);

            // The local transform is a transform-rotation-scale matrix (TRS)
            localTransform = new LocalTransform(
                  Matrix4x4.CreateScale(scale.ToVector3())
                * Matrix4x4.CreateRotationX(float.DegreesToRadians(rotation.X))
                * Matrix4x4.CreateRotationY(float.DegreesToRadians(rotation.Y))
                * Matrix4x4.CreateRotationZ(float.DegreesToRadians(rotation.Z))
                * Matrix4x4.CreateTranslation(position.ToVector3())
            );
        }

        // Starting from the root entity parse the hierarchy using Breadth First Search algorithm
        Entity root = Engine.Root;
        Queue<Entity> queue = new();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            var currentNode = queue.Dequeue();
            ref readonly GlobalTransform parentGlobalTransform = ref Get<GlobalTransform>(currentNode);
            var children = InRelations<Child>(currentNode);

            foreach (var child in children)
            {
                // Only perform the global matrix calculation if the transform has been changed (dirty).
                // This operation is expensive.
                if (Has<TransformDirty>(child))
                {
                    ref var childLocalTransform = ref Get<LocalTransform>(child);
                    Set(child, new GlobalTransform(childLocalTransform.Matrix * parentGlobalTransform.Matrix));
                    Remove<TransformDirty>(child);
                }

                queue.Enqueue(child);
            }

        }
    }
}

