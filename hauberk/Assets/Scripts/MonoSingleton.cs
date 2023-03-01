using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
	private static T _sInstance = null;

	public static T Instance
	{
		get
		{
			if (_sInstance == null)
			{
				_sInstance = new GameObject(typeof(T).Name).AddComponent<T>();
			}
			return _sInstance;
		}
	}

	protected virtual void Awake()
	{
		if (_sInstance && _sInstance.GetInstanceID() != GetInstanceID())
		{
			DestroyImmediate(gameObject);
		}
		else
		{
			_sInstance = (T)this;
			DontDestroyOnLoad(gameObject);
		}
	}

//    protected virtual void OnApplicationQuit()
//    {
//        _sInstance = null;
//    }
}
