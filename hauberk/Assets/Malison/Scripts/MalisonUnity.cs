using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MalisonUnity : MonoBehaviour
{
  private static MalisonUnity m_Inst;

  public static MalisonUnity Inst
  {
	  get
		{
			if (m_Inst == null)
			{
        var obj = new GameObject("MalisonUnity");
        m_Inst = obj.AddComponent<MalisonUnity>();
			}
			return m_Inst;
		}
  }

  public SpriteRenderer canvasBg;
  public Transform glyphsRoot;

  [Header("RUNTIME")]
  public Sprite[] sprites;

  void Awake() 
  {
   	if (m_Inst == null)
		{
			m_Inst = this;
		}
    // else
    // {
    //   UnityEngine.Debug.LogError("hand exit same ");
    //   Destroy(gameObject);
    //   return;
    // }
    DontDestroyOnLoad(this);

    sprites = Resources.LoadAll<Sprite>("dos-short");
  }

  public System.Action onKeyUpdate = null;
  public System.Action<float> onUpdate = null;
  private void Update() {
    if (onKeyUpdate != null)
      onKeyUpdate();
    if (onUpdate != null)
      onUpdate(Time.deltaTime);
  }
}