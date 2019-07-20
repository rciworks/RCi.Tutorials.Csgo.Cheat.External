using System;
using System.Threading;

namespace RCi.Tutorials.Csgo.Cheat.External.Utils
{
    /// <summary>
    /// Component which will have a separate thread for looping action.
    /// </summary>
    public abstract class ThreadedComponent :
        IDisposable
    {
        #region // storage

        /// <summary>
        /// Custom thread name.
        /// </summary>
        protected virtual string ThreadName => nameof(ThreadedComponent);

        /// <summary>
        /// Timeout for thread to finish.
        /// </summary>
        protected virtual TimeSpan ThreadTimeout { get; set; } = new TimeSpan(0, 0, 0, 3);

        /// <summary>
        /// Thread frame sleep.
        /// </summary>
        protected virtual TimeSpan ThreadFrameSleep { get; set; } = new TimeSpan(0, 0, 0, 0, 1);

        /// <summary>
        /// Thread for this component.
        /// </summary>
        private Thread Thread { get; set; }

        #endregion

        #region // ctor

        /// <summary />
        protected ThreadedComponent()
        {
            Thread = new Thread(ThreadStart)
            {
                Name = ThreadName, // Virtual member call in constructor
            };
        }

        /// <inheritdoc />
        public virtual void Dispose()
        {
            Thread.Interrupt();
            if (!Thread.Join(ThreadTimeout))
            {
                Thread.Abort();
            }
            Thread = default;
        }

        #endregion

        #region // routines

        /// <summary>
        /// Launch thread for execute frames.
        /// </summary>
        public void Start()
        {
            Thread.Start();
        }

        /// <summary>
        /// Thread method.
        /// </summary>
        private void ThreadStart()
        {
            try
            {
                while (true)
                {
                    FrameAction();
                    Thread.Sleep(ThreadFrameSleep);
                }
            }
            catch (ThreadInterruptedException)
            {
            }
        }

        /// <summary>
        /// Frame to loop inside a thread.
        /// </summary>
        protected abstract void FrameAction();

        #endregion
    }
}
