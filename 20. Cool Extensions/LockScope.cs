using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace System.Threading.Tasks
{
    /// <summary>
    /// Locking scope which can free the lock using Dispose and indicate whether lock is acquired 
    /// </summary>
    public sealed class LockScope : IDisposable
    {
        private SemaphoreSlim _gate;

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="LockScope"/> class.
        /// </summary>
        /// <param name="gate">The gate.</param>
        /// <param name="acquired">if set to <c>true</c> [acquired].</param>
        internal LockScope(SemaphoreSlim gate, bool acquired)
        {
            _gate = gate;
            Acquired = acquired;
        }

        #endregion // Ctor

        #region Acquired

        /// <summary>
        /// Gets a value indicating whether the lock is acquired.
        /// </summary>
        public bool Acquired { get; }

        #endregion // Acquired

        #region Dispose Pattern

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Dispose can be invoked once</exception>
        public void Dispose()
        {
            if (_gate == null)
                throw new ObjectDisposedException("Dispose can be invoked once");
            if(Acquired)
                _gate?.Release();
            _gate = null;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="LockScope"/> class.
        /// </summary>
        ~LockScope()
        {
            Dispose();
        }

        #endregion // Dispose Pattern
    }
}
