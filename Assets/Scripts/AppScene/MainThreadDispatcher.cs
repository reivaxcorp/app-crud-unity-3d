using System.Collections.Generic;
using UnityEngine;

public class MainThreadDispatcher : MonoBehaviour
{
    private static MainThreadDispatcher instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void ExecuteOnMainThread(System.Action action)
    {
        if (instance != null)
        {
            instance.RunAction(action);
        }
    }

    private void RunAction(System.Action action)
    {
        if (action != null)
        {
            lock (queueLock)
            {
                actionQueue.Enqueue(action);
            }
        }
    }

    private readonly object queueLock = new object();
    private Queue<System.Action> actionQueue = new Queue<System.Action>();

    private void Update()
    {
        lock (queueLock)
        {
            while (actionQueue.Count > 0)
            {
                actionQueue.Dequeue().Invoke();
            }
        }
    }
}
