using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class HauberkEditor
{
    [MenuItem("Tool/ClearStorage")]  
    public static void ClearStorage()
    {
        PlayerPrefs.SetString("heroes", null);
        PlayerPrefs.Save();
        Debug.Log("Success ClearStorage.");
    }
}
