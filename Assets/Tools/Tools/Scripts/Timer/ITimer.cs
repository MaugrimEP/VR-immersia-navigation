using System;

namespace Tools.Timer
{

    public interface ITimer
    {
        /// <summary>
        /// Interval [in µs]
        /// </summary>
        long Interval
        {
            get;
            set;
        }

        /// <summary>
        /// Event called when the timer elapsed
        /// </summary>
        event EventHandler<TimerElapsedEventArgs> Elapsed;

        /// <summary>
        /// Start the timer
        /// </summary>
        void Start();

        /// <summary>
        /// Stop/pause the timer
        /// </summary>
        void Stop(bool joinThread = true);
    }
}