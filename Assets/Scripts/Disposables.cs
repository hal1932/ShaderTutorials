using System;
using UnityEngine;
using UnityEngine.Rendering;

public class Disposable : IDisposable
{
    public Action<bool> OnDispose;

    ~Disposable()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }
        _disposed = true;
        OnDispose?.Invoke(disposing);
    }

    private bool _disposed;
}

public struct TempRenderTarget : IDisposable
{
    public CommandBuffer Command;
    public int Identifier;

    public TempRenderTarget(CommandBuffer cmd, RenderTextureDescriptor desc, FilterMode filter, int nameId)
    {
        Command = cmd;
        Identifier = nameId;
        cmd.GetTemporaryRT(Identifier, desc, filter);
    }

    public TempRenderTarget(CommandBuffer cmd, RenderTextureDescriptor desc, int nameId)
        : this(cmd, desc, FilterMode.Point, nameId)
    { }

    public TempRenderTarget(CommandBuffer cmd, RenderTextureDescriptor desc, FilterMode filter, string name = null)
        : this(cmd, desc, filter, Shader.PropertyToID(name ?? Guid.NewGuid().ToString()))
    { }

    public TempRenderTarget(CommandBuffer cmd, RenderTextureDescriptor desc, string name = null)
        : this(cmd, desc, FilterMode.Point, Shader.PropertyToID(name ?? Guid.NewGuid().ToString()))
    { }

    public void Dispose()
    {
        Command.ReleaseTemporaryRT(Identifier);
    }

    public static implicit operator int(TempRenderTarget rt) => rt.Identifier;
    public static implicit operator RenderTargetIdentifier(TempRenderTarget rt) => rt.Identifier;
}

public struct CommandBufferFromPoolScope : IDisposable
{
    public CommandBuffer Command;

    public CommandBufferFromPoolScope(CommandBuffer cmd)
    {
        Command = cmd;
    }

    public void Dispose()
    {
        CommandBufferPool.Release(Command);
    }
}
