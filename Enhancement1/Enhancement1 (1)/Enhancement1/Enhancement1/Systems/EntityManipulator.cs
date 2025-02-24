using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Enhancement1.Core;
using Enhancement1.Data;
using MoonTools.ECS;

namespace Enhancement1.Systems;

/// <summary>
/// A manipulator is an object that creates new entities
/// or performs operations on the existing ones.
/// Analogous term in OOP would be a factory, however a manipulator is not constrained to an object of a single type.
/// </summary>
public sealed class EntityManipulator : Manipulator
{
    readonly Engine Engine;

    public EntityManipulator(Engine engine) : base(engine.World)
    {
        Engine = engine;
    }

    /// <summary>
    /// Spawns the most basic type of entity with all the necessary compoentns.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="scale"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public Entity Create3DEntity(in Position position, in Rotation rotation, in Scale scale, string tag = "")
    {
        Entity entity = CreateEntity();
        Tag(entity, tag);
        Set(entity, position);
        Set(entity, rotation);
        Set(entity, scale);
        Set(entity, new LocalTransform(Matrix4x4.Identity));
        Set(entity, new GlobalTransform(Matrix4x4.Identity));
        Set(entity, new TransformDirty());
        Set(entity, new Spawned());
        Relate<Child>(entity, Engine.Root, new());

        return entity;
    }

    public Entity SpawnCube(in Position position, TextureID textureID)
    {
        var Resources = Engine.Resources;

        Entity entity = Create3DEntity(position, new Rotation(0, 0, 0), new Scale(1, 1, 1), "Cube");
        Set(entity, new Mesh(Resources.CubeModel));
        Set(entity, new Material(Resources.DefaultMeshPipeline, textureID));

        return entity;
    }

    public Entity SpawnFlowerPot(in Position position)
    {
        var Resources = Engine.Resources;

        Entity entity = Create3DEntity(position, new Rotation(0, 0, 0), new Scale(1, 1, 1), "FlowerPot");
        Set(entity, new Mesh(Resources.FlowerPotModel));
        Set(entity, new Material(Resources.DefaultMeshPipeline, Resources.FlowerPotDiffuseTexture));

        return entity;
    }

    public Entity SpawnRug(in Position position)
    {
        var Resources = Engine.Resources;

        Entity entity = Create3DEntity(position, new Rotation(0, 0, 0), new Scale(1, 1, 1), "FluffyRug");
        Set(entity, new Mesh(Resources.FluffyRugModel));
        Set(entity, new Material(Resources.DefaultMeshPipeline, Resources.FluffyRugDiffuseTexture));

        return entity;
    }

    public Entity SpawnSofa(in Position position, in Rotation rotation)
    {
        var Resources = Engine.Resources;

        Entity entity = Create3DEntity(position, rotation, new Scale(1, 1, 1), "Sofa");
        Set(entity, new Mesh(Resources.SofaModel));
        Set(entity, new Material(Resources.DefaultMeshPipeline, Resources.SofaDiffuseTexture));

        return entity;
    }

    public Entity SpawnTable(in Position position, in Rotation rotation)
    {
        var Resources = Engine.Resources;

        Entity entity = Create3DEntity(position, rotation, new Scale(1.5f, 1.5f, 1.5f), "Table");
        Set(entity, new Mesh(Resources.TableModel));
        Set(entity, new Material(Resources.DefaultMeshPipeline, Resources.TableDiffuseTexture));

        return entity;
    }

    public Entity SpawnTableRound(in Position position, in Rotation rotation)
    {
        var Resources = Engine.Resources;

        Entity entity = Create3DEntity(position, rotation, new Scale(2f, 2f, 1.5f), "TableRound");
        Set(entity, new Mesh(Resources.TableRoundModel));
        Set(entity, new Material(Resources.DefaultMeshPipeline, Resources.TableRoundDiffuseTexture));

        return entity;
    }

    public Entity SpawnCarpet(in Position position, in Rotation rotation)
    {
        var Resources = Engine.Resources;

        Entity entity = Create3DEntity(position, rotation, new Scale(2f, 2f, 1f), "Carpet");
        Set(entity, new Mesh(Resources.CarpetModel));
        Set(entity, new Material(Resources.DefaultMeshPipeline, Resources.CarpetDiffuseTexture));

        return entity;
    }

    public Entity SpawnDarkWoodenFloorTile(in Position position, in Rotation rotation)
    {
        var Resources = Engine.Resources;

        Entity entity = Create3DEntity(position, rotation, new Scale(1f, 1f, 1f), "DarkWoodenFloorTile");
        Set(entity, new Mesh(Resources.PlaneModel));
        Set(entity, new Material(Resources.DefaultMeshPipeline, Resources.DarkWoodenFloorDiffuseTexture));

        return entity;
    }

    public Entity SpawnPlasterWall(in Position position, in Rotation rotation)
    {
        var Resources = Engine.Resources;

        Entity entity = Create3DEntity(position, rotation, new Scale(1f, 1f, 1f), "PlasterWall");
        Set(entity, new Mesh(Resources.WallModel));
        Set(entity, new Material(Resources.DefaultMeshPipeline, Resources.PlasterWallDiffuseTexture));

        return entity;
    }
    public Entity SpawnPainting(in Position position, in Rotation rotation)
    {
        var Resources = Engine.Resources;

        Entity entity = Create3DEntity(position, rotation, new Scale(0.5f, 0.5f, 0.5f), "Painting");
        Set(entity, new Mesh(Resources.PaintingModel));
        Set(entity, new Material(Resources.DefaultMeshPipeline, Resources.PaintingDiffuseTexture));

        return entity;
    }

    public Entity SpawnCup(in Position position, in Rotation rotation)
    {
        var Resources = Engine.Resources;

        Entity entity = Create3DEntity(position, rotation, new Scale(1.35f, 1.35f, 1f), "Cup");
        Set(entity, new Mesh(Resources.CupModel));
        Set(entity, new Material(Resources.DefaultMeshPipeline, Resources.CupDiffuseTexture));

        return entity;
    }

    /// <summary>
    /// Spawns the scene. Can be used in conjunction with hot reload for fast iteration times.
    /// </summary>
    public void SpawnScene()
    {
        // spawn the walls and floor
        for (int i = -3; i <= 3; i++)
        {
            for (int j = -3; j <= 3; j++)
            {
                SpawnDarkWoodenFloorTile(new(i, j, 0), new());

                if (i == -3)
                {
                    SpawnPlasterWall(new(i - 0.5f, j, 0), new(0, 0, -90));
                }

                if (j == -3)
                {
                    SpawnPlasterWall(new(i, j - 0.5f, 0), new(0, 0, 0));
                }
            }
        }

        SpawnFlowerPot(new Position(-2.5f, -2.25f, 0.0f));
        SpawnCarpet(new Position(0, 0, 0.01f), new());
        SpawnSofa(new(2, -1, 0), new(0, 0, -45));
        SpawnSofa(new(-2, -1, 0), new(0, 0, -135));
        SpawnTableRound(new(), new());
        SpawnPainting(new(-3.49f, 0, 1.5f), new(0, 90, 180));
        SpawnCup(new(0.39f, -0.25f, 0.535f), new(0, 0, 30));
        SpawnCup(new(-0.39f, -0.25f, 0.535f), new(0, 0, -30));
    }
}
