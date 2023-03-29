using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameDebugger : MonoBehaviour
{
    public Main main;

    // public Game game;

    void Update()
    {
        if (main == null)
            main = GameObject.FindObjectOfType<Main>();
        
        // if (main == null || main.retroTerminal == null)
        //     return;
        // if (main.retroTerminal.screens.Count > 0)
        // {
        //     var topScene = main.retroTerminal.screens[main.retroTerminal.screens.Count - 1];
        //     if (topScene is GameScreen && game == null)
        //     {
        //         // Debug.Log("cur top scene is game screen");
        //         game = (topScene as GameScreen).game;
        //     }
        //     // else
        //     // {
        //     //     game = null;
        //     // }
        // }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GameDebugger))]
public class GameDebuggerEditor : Editor
{
    public override void OnInspectorGUI() 
    {
        // base.OnInspectorGUI();

        var gd = target as GameDebugger;
        // if (gd.game == null)
        //     return;

        // EditorGUILayout.LabelField("Actions:", gd.game._actions.Count.ToString());
        // EditorGUILayout.LabelField("Reactions:", gd.game._reactions.Count.ToString());
        // EditorGUILayout.LabelField("Events:", gd.game._events.Count.ToString());

        if (GUILayout.Button("Make Dirty"))
        {
            if (gd.main != null && gd.main.retroTerminal != null)
            {
                Debug.Log("Make terminal dirty.");
                gd.main.retroTerminal.Dirty();
            }
        }

        if (GUILayout.Button("Make Lighting Refresh"))
        {
            if (gd.main != null && gd.main.retroTerminal != null)
            {
                Debug.Log("Make lighting dirty.");
                gd.main.retroTerminal.Dirty();

                if (gd.main.retroTerminal.screens.Count > 0)
                {
                    var topScene = gd.main.retroTerminal.screens[gd.main.retroTerminal.screens.Count - 1];
                    if (topScene is GameScreen)
                    {
                        // Debug.Log("cur top scene is game screen");
                        var game = (topScene as GameScreen).game;
                        if (game.stage != null)
                        {
                            game.stage.heroVisibilityChanged();
                            game.stage._lighting.refresh();
                        }
                    }
                }
            }
        }
    }
} 
#endif
