using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoonTools.ECS;

namespace Enhancement1.Core;

/// <summary>
/// A thin abstraction over the MoonTools.ECS.System.
/// This injects the Engine into the class as well as introduces an Initialize method.
/// Because filters can only register entities that were spawned AFTER they were built
/// it is necessary to separate the system instantiation from initialization, meaning
/// that entity creation can only be performed at the initialization state and onwards.
/// </summary>
public abstract class System : MoonTools.ECS.System
{
    protected readonly Engine Engine;

    protected System(Engine engine) : base(engine.World)
    {
        Engine = engine;
    }

    public abstract void Initialize();

}
