using System;
using UnityEngine.Rendering;

public struct Profiler<TEvent>
    where TEvent : Enum
{
    public Profiler(CommandBuffer command)
    {
        _command = command;
    }

    public ProfilingScope Scope(TEvent ev)
    {
        return new ProfilingScope(_command, ProfilingSampler.Get(ev));
    }

    private CommandBuffer _command;
}
