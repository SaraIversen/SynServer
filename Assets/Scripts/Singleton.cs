using UnityEngine;

public static class Singleton
{
    /// <summary>
    /// Initializes a singleton instance for a given MonoBehaviour.
    /// </summary>
    /// <typeparam name="T">The type of the MonoBehaviour</typeparam>
    /// <param name="instance">Reference to the instance variable in the MonoBehaviour</param>
    /// <param name="owner">Reference to the owning GameObject</param>
    /// <returns>The singleton instance of type T</returns>
    public static T Initialize<T>(ref T instance, MonoBehaviour owner/*, bool dontDestroyOnLoad = false*/) where T : MonoBehaviour
    {
        if (instance != null)
        {
            Debug.LogWarning($"Singleton of type {typeof(T).Name} already exists on {instance.gameObject.name}. Destroying duplicate on {owner.gameObject.name}.");
            GameObject.Destroy(owner.gameObject);
            return instance;
        }

        instance = owner as T;

        /* if (dontDestroyOnLoad) GameObject.DontDestroyOnLoad(owner.gameObject);*/

        return instance;
    }
}
