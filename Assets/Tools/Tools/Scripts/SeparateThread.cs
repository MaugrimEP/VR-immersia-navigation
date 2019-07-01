using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

/// <summary>
/// Executes actions in a different thread than the Unity thread.
/// We use a singleton to avoid having too many threads launched. Only one will be used here
/// <example>
/// <code>
/// SeparateThread(parameter => { /* Action instructions */ parameter(); }, () => Debug.Log("Done"));
/// </code>
/// </example>
/// </summary>
public class SeparateThread : UnitySingleton<SeparateThread>
{

    private Thread unityThread;

    /// <summary>
    /// Keeps track on each launched thread
    /// </summary>
    class ThreadStatus
    {
        public bool isExecuted = false;
        public Action action = null;
        public Action<Action> oldAction = null;
        public Action callback = null;
        public bool isDone = false;
        public StackTrace stackTrace = null;
        public bool MustStop = false;
    }

    /// <summary>
    /// Keeps track on each launched thread
    /// </summary>
    List<ThreadStatus> handles = new List<ThreadStatus>();

    void Awake()
    {
        // save the unity thread info
        unityThread = Thread.CurrentThread;
        // force the creation of UnityThreadExecute if needed
        var unityThreadExecute = UnityThreadExecute.Instance;
        // Waits for the separate thread to be executed from the Unity thread.
        StartCoroutine(WaitForThreadExecution());
    }

    /// <summary>
    /// Check if the code is in Unity's thread
    /// </summary>
    /// <returns>True if the call to this method was made from Unity's thread</returns>
    public bool InUnityThread()
    {
        return Thread.CurrentThread == unityThread;
    }


    /// <summary>
    /// Executes an action in a separate thread and fires a callback once the action has been executed.
    /// </summary>
    /// <example>
    /// <code>
    /// ExecuteInThread(parameter => { /* Action instructions */ parameter(); }, () => Debug.Log("Done"));
    /// </code>
    /// </example>
    /// <param name="action">The action</param>
    /// <param name="callback">The callback which is fired</param>
    public void ExecuteInThread(Action action, Action callback)
    {
        // threadStatus synchronizes/joins the separate thread and the Unity thread.
        ThreadStatus threadStatus = new ThreadStatus();
        threadStatus.action = action;
        threadStatus.callback = callback;
        // Note : may also be done using new System.Diagnostics.StackTrace(Thread) but this seems to not be implmented in mono yet
        threadStatus.stackTrace = new System.Diagnostics.StackTrace(true);

        // Starts the separate thread.
        ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadProc), threadStatus);
        handles.Add(threadStatus);

    }

    /// <summary>
    /// Executes an action in a separate thread and fires a callback once the action has been executed.
    /// </summary>
    /// <example>
    /// <code>
    /// ExecuteInThread(parameter => { /* Action instructions */ parameter(); }, () => Debug.Log("Done"));
    /// </code>
    /// </example>
    /// <param name="action">The action (the parameter action must be called at the end)</param>
    /// <param name="callback">The callback which is fired</param>
    [Obsolete("This version of the method requires the action paramter to be called at the end. Use the other version instead")]
    public void ExecuteInThread(Action<Action> action, Action callback)
    {
        // threadStatus synchronizes/joins the separate thread and the Unity thread.
        ThreadStatus threadStatus = new ThreadStatus();
        threadStatus.oldAction = action;
        threadStatus.callback = callback;
        // Note : may also be done using new System.Diagnostics.StackTrace(Thread) but this seems to not be implmented in mono yet
        threadStatus.stackTrace = new System.Diagnostics.StackTrace(true);

        // Starts the separate thread.
        ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadProc), threadStatus);
        handles.Add(threadStatus);

    }

    // The thread procedure performs the independent task
    //
    static void ThreadProc(System.Object stateInfo)
    {
        ThreadStatus threadStatus = (ThreadStatus)stateInfo;
        try
        {
            if (threadStatus.action != null)
            {
                threadStatus.action();
                threadStatus.isExecuted = true;
            }
            else
            {
                threadStatus.oldAction(() => { threadStatus.isExecuted = true; });
            }
        }/* TODO Check these exceptions. I Don't think it's a good idea to hide them because they can happen in a thread launched by a thread
        catch (ThreadAbortException e)
        {
            //Previous thread was being aborted.
            return;
        }*/
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Exception in thread : " + e + "\n" + threadStatus.stackTrace.ToString());
        }
    }


    IEnumerator WaitForThreadExecution()
    {
        while (true)
        {
            for (int i = 0; i < handles.Count; i++)
            {
                ThreadStatus handle = handles[i];
                if (handle.isExecuted)
                {
                    // This can start a new thread !
                    handle.callback();

                    handle.isDone = true;
                }
            }
            handles.RemoveAll(handle => handle.isDone);
            yield return null;
        }
    }

}
