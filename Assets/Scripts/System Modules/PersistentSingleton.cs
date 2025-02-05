using UnityEngine;

public class PersistentSingleton<T> : MonoBehaviour where T : Component
{
    // 它的作用是确保某个 MonoBehaviour 类型的对象在整个游戏生命周期中只有一个实例
    public static T Instance {get; private set;}

    protected virtual void Awake()
    {
        if(Instance == null)
        {
            Instance = this as T;
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }


}
