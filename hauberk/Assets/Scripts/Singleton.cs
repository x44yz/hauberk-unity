using System.Collections;
using System.Collections.Generic;

public class Singleton<T> where T : class, new()
{
	private static T _sInstance = null;

	public static T Instance
	{
		get
		{
			if (_sInstance == null)
			{
				_sInstance = new T();
			}
			return _sInstance;
		}
	}

	public static void Release()
	{
		_sInstance = null;
	}
}
