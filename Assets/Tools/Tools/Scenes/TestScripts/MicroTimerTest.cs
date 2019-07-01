using Tools.GraphDisplay;
using Tools.Timer;
using UnityEditor;
using UnityEngine;

public class MicroTimerTest : MonoBehaviour
{

    MicroTimer microTimer;

    [Tooltip("time step in microseconds")]
    public long TimeStep = 1000;

    public int steps = 0;

    public long realStepTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        microTimer = new MicroTimer(TimeStep);
        microTimer.Elapsed += TimerUpdate;
        microTimer.Start();

        // Showing how to use the graph display
        TimerGraphDisplay graphDisplay = FindObjectOfType<TimerGraphDisplay>();
        if (graphDisplay != null)
            graphDisplay.ShowTimer(microTimer);


#if UNITY_EDITOR
        EditorApplication.pauseStateChanged += HandlePause;
#endif
    }

#if UNITY_EDITOR
    void HandlePause(PauseState state)
    {
        if (state == PauseState.Paused && microTimer.Running)
            microTimer.Stop();
        if (state == PauseState.Unpaused && !microTimer.Running)
            microTimer.Start();
    }
#endif

    // Update is called once per frame
    public void TimerUpdate(object sender, TimerElapsedEventArgs arg)
    {
        steps = arg.Count;
        realStepTime = arg.Delay;
    }

}
