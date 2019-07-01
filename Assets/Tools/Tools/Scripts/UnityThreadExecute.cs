using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Place this object on any game object to deffer code execution from other threads to unity's thread
/// <example>
/// <code>
/// var handle = new EventWaitHandle(false, EventResetMode.AutoReset);
/// UnityThreadExecute.InvokeNextUpdate(() =>
/// {
///     *Unity dependent code *
///     handle.Set();
/// });
/// handle.WaitOne();
/// </code>
/// the handle variable ensures that the thread waits for the code to be executed in unity's thread
/// </example>
/// </summary>
public class UnityThreadExecute : UnitySingleton<UnityThreadExecute>
{

    [Flags]
    public enum UnityExecutionStep
    {
        None = 0,
        Update = 1,
        LateUpdate = 2,
        /// <summary>
        /// Update or LateUpdate
        /// </summary>
        GraphicsUpdate = Update | LateUpdate,
        FixedUpdate = 4,
        OnRenderObject = 8,
        OnGUI = 16,
        Any = Update | LateUpdate | FixedUpdate | OnRenderObject | OnGUI
    };

    private static Array checkTypeValues;

    static Array CheckTypeValues
    {
        get
        {
            if (checkTypeValues == null)
                checkTypeValues = Enum.GetValues(typeof(UnityExecutionStep));
            return checkTypeValues;
        }
    }

    protected class ActionEntry
    {
        public Action Action { get; set; }
        public UnityExecutionStep ExecutionSteps { get; set; }
        public bool InvokeOnce { get; set; }
    }

    protected UnityThreadExecute() { }

    private readonly List<ActionEntry> actionsToInvoke = new List<ActionEntry>();

    private static object _lock = new object();

    // This object needs to exist from the start otherwise we will attempt to create it through the first threaded call and it will fail
    static UnityThreadExecute _instance;

    void Awake()
    {
        _instance = Instance;
    }

    void Update()
    {
        InvokeActions(UnityExecutionStep.Update);
    }

    void LateUpdate()
    {
        InvokeActions(UnityExecutionStep.LateUpdate);
    }

    void FixedUpdate()
    {
        InvokeActions(UnityExecutionStep.FixedUpdate);
    }

    private void OnRenderObject()
    {
        InvokeActions(UnityExecutionStep.OnRenderObject);
    }

    private void OnGUI()
    {
        InvokeActions(UnityExecutionStep.OnGUI);
    }

    private void InvokeActions(UnityExecutionStep flag)
    {
        List<ActionEntry> actions = null;
        // recover all actions corresponding to the given step
        lock (_lock)
        {
            // ToList forces the creation of a new collection which is good because we don't want to have inter-thread issues
            actions = actionsToInvoke.Where(actionEntry => (actionEntry.ExecutionSteps & flag) > 0).ToList();
        }
        // execute and remove from the dictionary
        foreach (ActionEntry actionEntry in actions)
        {
            actionEntry.Action();
            lock (_lock)
            {
                if (actionEntry.InvokeOnce)
                    actionsToInvoke.Remove(actionEntry);
            }
        }

    }

    /// <summary>
    /// Deffer code execution from other threads to unity's thread
    /// <example>
    /// <code>
    /// var handle = new EventWaitHandle(false, EventResetMode.AutoReset);
    /// UnityThreadExecute.InvokeNextUpdate(() =>
    /// {
    ///     *Unity dependent code *
    ///     handle.Set();
    /// });
    /// handle.WaitOne();
    /// </code>
    /// the handle variable ensures that the thread waits for the code to be executed in unity's thread
    /// </example>
    /// </summary>
    public static void InvokeNextUpdate(System.Action action)
    {
        InvokeActionForNextExecutionSteps(action, UnityExecutionStep.Update);
    }

    /// <summary>
    /// Invokes the given action at the first execution step that will occur amongst the ones specified. The action is invoked once
    /// </summary>
    /// <param name="action"></param>
    /// <param name="executionSteps"></param>
    public static void InvokeActionForNextExecutionSteps(System.Action action, UnityExecutionStep executionSteps = UnityExecutionStep.Any)
    {
        if (action == null)
            throw new ArgumentNullException("action");
        var h = _instance;
        ActionEntry newAction = new ActionEntry();
        newAction.Action = action;
        newAction.ExecutionSteps = executionSteps;
        newAction.InvokeOnce = true;
        lock (_lock)
        {
            h.actionsToInvoke.Add(newAction);
        }
    }

    /// <summary>
    /// Invokes the given action at EVERY execution step specified. The action is invoked while it stays registered
    /// </summary>
    /// <param name="action"></param>
    /// <param name="executionSteps"></param>
    public static void RegisterActionForExecutionSteps(System.Action action, UnityExecutionStep executionSteps = UnityExecutionStep.Any)
    {
        if (action == null)
            throw new ArgumentNullException("action");
        var h = _instance;
        ActionEntry newAction = new ActionEntry();
        newAction.Action = action;
        newAction.ExecutionSteps = executionSteps;
        newAction.InvokeOnce = false;
        lock (_lock)
        {
            h.actionsToInvoke.Add(newAction);
        }
    }

    public static void UnRegisterActionForExecutionSteps(System.Action action, UnityExecutionStep executionSteps = UnityExecutionStep.Any)
    {
        if (action == null)
            throw new ArgumentNullException("action");
        var h = _instance;
        lock (_lock)
        {
            ActionEntry actionToChange = h.actionsToInvoke.Find(actionEntry => actionEntry.Action.Equals(action) && !actionEntry.InvokeOnce);
            if (actionToChange != null)
            {
                UnityExecutionStep newFlags = actionToChange.ExecutionSteps & ~executionSteps;
                if (newFlags == UnityExecutionStep.None)
                    h.actionsToInvoke.Remove(actionToChange);
                else
                    actionToChange.ExecutionSteps = newFlags;
            }
        }
    }


}
