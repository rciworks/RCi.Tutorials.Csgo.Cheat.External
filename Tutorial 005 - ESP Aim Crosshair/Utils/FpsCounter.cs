using System;
using System.Diagnostics;

namespace RCi.Tutorials.Csgo.Cheat.External.Utils
{
    /// <summary>
    /// Class for measuring fps.
    /// </summary>
    public class FpsCounter
    {
        #region // storage

        /// <summary>
        /// One second timespan.
        /// </summary>
        private static readonly TimeSpan TimeSpanFpsUpdate = new TimeSpan(0, 0, 0, 1);

        /// <summary>
        /// Stopwatch for measuring time.
        /// </summary>
        private Stopwatch FpsTimer { get; } = Stopwatch.StartNew();

        /// <summary>
        /// Frame count since last timer restart.
        /// </summary>
        private int FpsFrameCount { get; set; }

        /// <summary>
        /// Average fps (frames per second).
        /// </summary>
        public double Fps { get; private set; }

        #endregion

        #region // routines

        /// <summary>
        /// Trigger frame update.
        /// </summary>
        public void Update()
        {
            var fpsTimerElapsed = FpsTimer.Elapsed;
            if (fpsTimerElapsed > TimeSpanFpsUpdate)
            {
                Fps = FpsFrameCount / fpsTimerElapsed.TotalSeconds;
                FpsTimer.Restart();
                FpsFrameCount = 0;
            }
            FpsFrameCount++;
        }

        #endregion
    }
}
