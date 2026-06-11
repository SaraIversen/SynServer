using System;
using System.Collections.Generic;
using UnityEngine;

class ThreadManager : MonoBehaviour
{
    // Queue of actions waiting to be executed on Unity's main thread.
    // Other threads add actions to this list.
    private static readonly List<Action> executeOnMainThread = new List<Action>();

    // Temporary copy of the queue.
    // This prevents issues if new actions are added while we're executing.
    private static readonly List<Action> executeCopiedOnMainThread = new List<Action>();

    // Simple flag so UpdateMain() doesn't lock and check the list every frame.
    private static bool actionToExecuteOnMainThread = false;


    private void FixedUpdate()
    {
        // Unity calls FixedUpdate at a fixed time interval
        // Process any queued actions here instead of every frame.
        UpdateMain();
    }


    /// <summary>
    /// Queues an action to be executed on Unity's main thread.
    /// Can safely be called from any thread.
    /// </summary>
    /// <param name="action">The action to be executed on the main thread.</param>
    public static void ExecuteOnMainThread(Action action)
    {
        // Prevent null actions from being added.
        if (action == null)
        {
            Debug.Log("No action to execute on main thread!");
            return;
        }

        // Lock prevents multiple threads from modifying
        // the list at the same time.
        lock (executeOnMainThread)
        {
            // Add the action to the queue.
            executeOnMainThread.Add(action);

            // Let UpdateMain know there is work to do.
            actionToExecuteOnMainThread = true;
        }
    }

    /// <summary>
    /// Executes all queued actions to run on the main thread.
    /// IMPORTANT: Must only be called from Unity's main thread.
    /// </summary>
    public static void UpdateMain()
    {
        // Skip everything if no actions are waiting.
        if (actionToExecuteOnMainThread)
        {
            // Remove any old copied actions.
            executeCopiedOnMainThread.Clear();

            // Lock while copying to avoid race conditions.
            lock (executeOnMainThread)
            {
                // Copy queued actions into a temporary list.
                executeCopiedOnMainThread.AddRange(executeOnMainThread);

                // Clear the original queue so new actions can be added while we're executing.
                executeOnMainThread.Clear();

                // Reset the flag.
                actionToExecuteOnMainThread = false;
            }

            // Execute every queued action on the main thread.
            for (int i = 0; i < executeCopiedOnMainThread.Count; i++)
            {
                executeCopiedOnMainThread[i]();
            }
        }
    }
}