using Tools.Timer;
using UnityEditor;
using UnityEngine;

public class HighResolutionTimerTest : MonoBehaviour
{

    HighResolutionTimer highResolutionTimer;

    [Tooltip("time step in miliseconds")]
    public float TimeStep = 2.5f;

    public int steps = 0;

    public double realStepTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        highResolutionTimer = new HighResolutionTimer(TimeStep);
        highResolutionTimer.Elapsed += TimerUpdate;
        highResolutionTimer.Start();
#if UNITY_EDITOR
        EditorApplication.pauseStateChanged += HandlePause;
#endif
    }


#if UNITY_EDITOR
    void HandlePause(PauseState state)
    {
        if (state == PauseState.Paused && highResolutionTimer.Running)
            highResolutionTimer.Stop();
        if (state == PauseState.Unpaused && !highResolutionTimer.Running)
            highResolutionTimer.Start();
    }
#endif

    // Update is called once per frame
    public void TimerUpdate(object sender, TimerElapsedEventArgs arg)
    {
        steps = arg.Count;
        realStepTime = arg.Delay / 1000f;
    }

}
