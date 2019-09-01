using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 场景中物体单例
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingleTon<T> : MonoBehaviour where T : MonoBehaviour
{
    static T _instance = default(T);
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (T)FindObjectOfType(typeof(T));
                if (_instance == null)
                {
                    GameObject go = new GameObject(typeof(T).Name);
                    _instance = go.AddComponent<T>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }
    public virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
        }
        else
            Destroy(this.gameObject);
    }
    public virtual void OnEnable()
    {

    }
    public virtual void Dispose()
    {

    }
    void OnDestroy()
    {
        Instance = null;
    }
}
