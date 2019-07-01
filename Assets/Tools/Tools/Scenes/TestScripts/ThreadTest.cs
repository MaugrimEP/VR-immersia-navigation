using System;
using System.Threading;
using UnityEngine;

public class ThreadTest : MonoBehaviour
{

    public int ExecuteEveryXFrame = 20;

    public bool TestExceptionInThread = true;

    public bool UseShortThread = true;
    public bool UseLongThread = true;
    public bool UseVeryLongThread = true;

    public bool UseAnyUpdateForVeryLongThread = true;
    public bool UseUpdateForVeryLongThread = false;
    public bool UseLateUpdateForVeryLongThread = false;
    public bool UseFixedUpdateForVeryLongThread = false;

    public bool RegisterRegisterThread = true;
    private bool threadRegistered = false;

    private static float startingAngle = 0f;


    private int threadId = 0;

    void OnEnable()
    {
        if (RegisterRegisterThread)
            RegisterThread();
        WorkingThread();
        CrashingThread();
    }

    void Awake()
    {
        WorkingThread();
        CrashingThread();
    }

    void Start()
    {
        WorkingThread();
        CrashingThread();
    }

    // may not work
    void OnDisable()
    {
        WorkingThread();
        CrashingThread();
    }

    // Update is called once per frame
    void Update()
    {
        if (RegisterRegisterThread && !threadRegistered)
            RegisterThread();
        if (!RegisterRegisterThread && threadRegistered)
            UnRegisterThread();
        if (Time.frameCount % ExecuteEveryXFrame == 0)
        {
            WorkingThread();
            CrashingThread();
        }
    }

    private void CrashingThread()
    {
        if (!TestExceptionInThread)
            return;
        SeparateThread.Instance.ExecuteInThread(() =>
        {
            // wait for 1.1sec
            Thread.Sleep(1100);
            DateTime time = DateTime.Now;

            throw new Exception("Crashing Thread. that's Normal !");

        },
             () =>
             {
                 Debug.LogError("Thread executed but should not !!");
                 // no error
                 GameObject.Find("ThreadTest");
             }
             );
    }

    private void RegisterThread()
    {
        threadRegistered = true;
        SeparateThread.Instance.ExecuteInThread(() => RegisterThread(RegisteredThreadAction),
             () =>
             {
                 Debug.Log("Registered Thread executed");
                 // no error
                 GameObject.Find("ThreadTest");
             }
             );
    }

    private void UnRegisterThread()
    {
        threadRegistered = false;
        SeparateThread.Instance.ExecuteInThread(() => UnRegisterThread(RegisteredThreadAction),
             () =>
             {
                 Debug.Log("Unregistered Thread executed");
                 // no error
                 GameObject.Find("ThreadTest");
             }
             );
    }

    private void WorkingThread()
    {
        if (UseLongThread)
        {
            SeparateThread.Instance.ExecuteInThread(() => LongThread(threadId++),
                 () =>
                 {
                     Debug.Log("Long Thread executed");
                     // no error
                     GameObject.Find("ThreadTest");
                 }
                 );
        }

        if (UseShortThread)
        {
            SeparateThread.Instance.ExecuteInThread(ShortThread,
              () =>
              {
                  Debug.Log("Short Thread executed");
                  // no error
                  GameObject.Find("ThreadTest");
              }
              );
        }

        if (UseVeryLongThread)
        {
            SeparateThread.Instance.ExecuteInThread(() => VeryLongThread(threadId++),
                () =>
                {
                    Debug.Log("Very Long Thread executed");
                    // no error
                    GameObject.Find("ThreadTest");
                }
                );
        }
    }

    private void RegisterThread(Action registeredThreadAction)
    {
        // error should be displayed
        // GameObject.Find("ThreadTest");
        UnityThreadExecute.RegisterActionForExecutionSteps(registeredThreadAction, UnityThreadExecute.UnityExecutionStep.OnRenderObject);
    }

    private void UnRegisterThread(Action registeredThreadAction)
    {
        // error should be displayed
        // GameObject.Find("ThreadTest");
        UnityThreadExecute.UnRegisterActionForExecutionSteps(registeredThreadAction, UnityThreadExecute.UnityExecutionStep.OnRenderObject);
    }

    private void LongThread(int threadId)
    {
        // wait for 1sec
        Thread.Sleep(1000);
        DateTime time = DateTime.Now;

        Debug.Assert(!SeparateThread.Instance.InUnityThread());

        // error should be displayed
        // GameObject.Find("ThreadTest");
        // use unity thread for that part
        var handle = new EventWaitHandle(false, EventResetMode.AutoReset);
        UnityThreadExecute.InvokeNextUpdate(() =>
        {
            Debug.Assert(SeparateThread.Instance.InUnityThread());
            Debug.Log("Update " + threadId + " : Time is " + time.Hour + ":" + time.Minute);
            // no error
            GameObject.Find("ThreadTest");
            handle.Set();
        });
        handle.WaitOne();

        // wait for 1sec
        Thread.Sleep(1000);
    }

    private void VeryLongThread(int threadId)
    {
        // wait for 10sec
        Thread.Sleep(10000);
        DateTime time = DateTime.Now;

        UnityThreadExecute.UnityExecutionStep stepFlag = UnityThreadExecute.UnityExecutionStep.None;
        if (UseUpdateForVeryLongThread)
            stepFlag |= UnityThreadExecute.UnityExecutionStep.Update;
        if (UseLateUpdateForVeryLongThread)
            stepFlag |= UnityThreadExecute.UnityExecutionStep.LateUpdate;
        if (UseFixedUpdateForVeryLongThread)
            stepFlag |= UnityThreadExecute.UnityExecutionStep.FixedUpdate;
        if (UseAnyUpdateForVeryLongThread)
            stepFlag |= UnityThreadExecute.UnityExecutionStep.Any;

        // error should be displayed
        // GameObject.Find("ThreadTest");
        // use unity thread for that part
        var handle = new EventWaitHandle(false, EventResetMode.AutoReset);
        UnityThreadExecute.InvokeActionForNextExecutionSteps(() =>
        {
            Debug.Log("Any " + threadId + ": Time is " + time.Hour + ":" + time.Minute);
            // no error
            GameObject.Find("ThreadTest");

            handle.Set();
        }, stepFlag);
        handle.WaitOne();

        // wait for 10sec
        Thread.Sleep(10000);
    }

    private void ShortThread()
    {
        DateTime time = DateTime.Now;

        Debug.Log("Time is " + time.Hour + ":" + time.Minute);
    }


    private void RegisteredThreadAction()
    {
        DateTime time = DateTime.Now;
        Debug.Log("Update registered : Time is " + time.Hour + ":" + time.Minute);
        // no error
        GameObject.Find("ThreadTest");
        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        //GL.MultMatrix(transform.localToWorldMatrix);

        // Draw lines
        GL.Begin(GL.LINES);
        for (int i = 0; i < 5; ++i)
        {
            float a = i / (float)5;
            float angle = a * Mathf.PI * 2 + startingAngle++;
            // Vertex colors change from red to green
            GL.Color(new Color(a, 1 - a, 0, 0.8F));
            // One vertex at transform position
            GL.Vertex3(0, 0, 0);
            // Another vertex at edge of circle
            GL.Vertex3(Mathf.Cos(angle) * 5, Mathf.Sin(angle) * 5, 0);
        }
        GL.End();
        GL.PopMatrix();
    }
}

