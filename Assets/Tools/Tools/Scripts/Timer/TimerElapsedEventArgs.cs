using System;

namespace Tools.Timer
{
    public class TimerElapsedEventArgs : EventArgs
    {
        // Simple counter, number times timed event (callback function) executed
        public int Count { get; private set; }

        /// <summary>/// Real timer delay in [µs] since last event/// </summary>
        public long Delay { get; }

        /// <summary>/// Real timer total time in [µs]/// </summary>
        public long TotalTime { get; }

        // How late the timer was compared to when it should have been called
        public long TimerLateBy { get; private set; }

        // Time it took to execute previous call to callback function (Elapsed)
        public long PreviousElapsedEventExecutionTime { get; private set; }

        internal TimerElapsedEventArgs(int count, long delay, long totalTime, long timerLateBy, long previousElapsedEventExecutionTime)
        {
            Count = count;
            Delay = delay;
            TotalTime = totalTime;
            TimerLateBy = timerLateBy;
            PreviousElapsedEventExecutionTime = previousElapsedEventExecutionTime;

        }
    }
}
