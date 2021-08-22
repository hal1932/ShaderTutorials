using System;
using UnityEngine.Rendering;

namespace Renderers
{
    public interface IRendererContext
    { }

    public abstract class RendererBase<TContext> : IDisposable
        where TContext : struct, IRendererContext
    {
        public RendererBase()
        {
            _profilingSampler = new ProfilingSampler(GetType().Name);
        }

        public void Render(ref TContext context, CommandBuffer cmd)
        {
            using (new ProfilingScope(cmd, _profilingSampler))
            {
                OnRender(ref context, cmd);
            }
        }

        protected abstract void OnRender(ref TContext context, CommandBuffer cmd);
        protected virtual void OnCleanup() { }

        #region IDisposable
        ~RendererBase()
            => Dispose(false);

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

            OnCleanup();
        }

        private bool _disposed;
        #endregion

        private TContext _context;
        private ProfilingSampler _profilingSampler;
    }
}
