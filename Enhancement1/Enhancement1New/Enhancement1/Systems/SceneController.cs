using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enhancement1.Core;
using Enhancement1.Data;
using MoonTools.ECS;
using MoonWorks.Input;

namespace Enhancement1.Systems;

/// <summary>
/// This system controls the scene spawning logic.
/// Used for hot reload as well.
/// </summary>
public sealed class SceneController : Core.System
{
    readonly EntityManipulator EntityManipulator;
    readonly Filter SpawnedFilter;

    public SceneController(Engine engine) : base(engine)
    {
        EntityManipulator = new EntityManipulator(Engine);
        // We need to cache spawned entities in a filter,
        // so that we can clear all those objects when respawning the scene during hot reload
        // or in a more complex scenario this would be used to remove the entities before changing to the next scene
        SpawnedFilter = FilterBuilder.Include<Spawned>().Build();
    }
    public override void Initialize()
    {
        EntityManipulator.SpawnScene();
    }

    public override void Update(TimeSpan delta)
    {
        // Respawn the scene
        if (Engine.Inputs.Keyboard.IsPressed(KeyCode.R))
        {
            SpawnedFilter.DestroyAllEntities();
            EntityManipulator.SpawnScene();
        }
    }

}
