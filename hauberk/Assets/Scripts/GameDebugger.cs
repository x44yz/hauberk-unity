using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameDebugger : MonoBehaviour
{
    public Main main {
        get {
            return GameObject.FindObjectOfType<Main>();
        }
    }

    public Game game {
        get {
            if (main == null || main.retroTerminal == null)
                return null;
            int count = main.retroTerminal.screens.Count;
            if (count > 0)
            {
                var topScene = main.retroTerminal.screens[count - 1];
                if (topScene is GameScreen)
                    return (topScene as GameScreen).game;
            }
            return null;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GameDebugger))]
public class GameDebuggerEditor : Editor
{
    public override void OnInspectorGUI() 
    {
        var gd = target as GameDebugger;

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
            if (gd.game != null && gd.game.stage != null)
            {
                Debug.Log("Make lighting dirty.");
                gd.game.stage.heroVisibilityChanged();
                gd.game.stage._lighting.refresh();
                gd.main.retroTerminal.Dirty();
            }
        }

        string btnTex = Debugger.debugHideFog ? "Make fog visible" : "Make fog hide";
        if (GUILayout.Button(btnTex))
        {
            Debugger.debugHideFog = !Debugger.debugHideFog;
            gd.game.stage.heroVisibilityChanged();
            foreach (var pos in gd.game.stage.bounds)
            {
                gd.game.stage.setVisibility(pos, false, 0);
            }
            gd.game.stage._lighting.refresh();
            gd.main.retroTerminal.Dirty();
        }

        if (GUILayout.Button("Add Gold"))
        {
            if (gd.game != null && gd.game.stage != null)
            {
                Debug.Log("Add Gold.");
                gd.game.hero.gold += 100;
            }
        }
    }
} 
#endif
