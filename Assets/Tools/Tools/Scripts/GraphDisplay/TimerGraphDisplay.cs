using Tools.Timer;
using UnityEngine;

namespace Tools.GraphDisplay
{
    /// <summary>
    /// Delay : shows how precise the timer is
    /// EventExecutionTime : show how long the execution of the loop is
    /// </summary>
    public class TimerGraphDisplay : MonoBehaviour
    {
        public UnityEngine.UI.Image Image;

        public bool PushMode = false;

        public enum DataType
        {
            Delay = 0,
            EventExecutionTime = 1
        };

        public DataType dataType = DataType.Delay;

        protected ITimer timer;

        protected Graph graph;

        protected long lastValue;

        public void ShowTimer(ITimer timer)
        {
            if (this.timer == null)
            {
                this.timer = timer;
                graph = new Graph(Image, (int)(5 * 1 * 1000 * 1000 / timer.Interval));
                timer.Elapsed += UpdateGraphData;
                graph.Limit = timer.Interval;
                graph.Max = timer.Interval * 5;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (graph != null)
                graph.UpdateGraph();
        }

        private void UpdateGraphData(object sender, TimerElapsedEventArgs e)
        {
            if (graph != null)
            {
                switch (dataType)
                {
                    case DataType.EventExecutionTime:
                        lastValue = e.PreviousElapsedEventExecutionTime;
                        break;
                    case DataType.Delay:
                        lastValue = e.Delay;
                        break;
                    default:
                        lastValue = e.Delay;
                        break;
                }
                if (!PushMode)
                    graph.AddData(lastValue);
                else
                    graph.PushData(lastValue);
            }
        }
    }
}