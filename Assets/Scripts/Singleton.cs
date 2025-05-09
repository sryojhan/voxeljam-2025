using Unity.VisualScripting;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    protected virtual bool DestroyOnLoad => true;

    public static T instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<T>();

                if (_instance == null)
                {
                    throw new UnityException("Could not locate singleton of type " + typeof(T).ToString());
                }

                //Check if the singleton needs to persists between scenes
                if(!(_instance as Singleton<T>).DestroyOnLoad)
                {
                    _instance.transform.parent = null;
                    DontDestroyOnLoad(_instance.gameObject);
                }
            }

            return _instance;
        }
    }

    /// <summary>
    /// This method is a save way to check if a singleton is initialized without initialising it
    /// </summary>
    /// <returns>Returns true if the singleton has been initialised</returns>
    public static bool IsInitialised()
    {
        return _instance == null;
    }

    public static T EnsureInitialised()
    {
        return instance;
    }

    public static bool ImTheOne(T me)
    {
        return instance == me;
    }

    public static bool DestroyIfInitialised(T me)
    {
        if(instance != me)
        {
            Destroy(me.gameObject);

            return true;
        }
        return false;

    }

}
