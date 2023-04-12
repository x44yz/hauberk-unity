using System;
using System.Collections;
using System.Collections.Generic;
using UnityTerminal;
using UnityEngine;

public class Main : MonoBehaviour
{
  public static Main Inst;

  public RetroCanvas retroCanvas;
  public int width;
  public int height;
  public bool debugRand;
  public int seed;

  [Header("RUNTIME")]
  // public float terminalScale;
  public RetroTerminal retroTerminal;

  private void Awake() 
  {
    Inst = this;

    if (debugRand)
    {
      Rng._rng = new Rng(seed);
    }
  }

  void Start()
  {
    var content = GameContent.createContent();

    Camera.main.orthographicSize = UnityEngine.Screen.height / retroCanvas.pixelToUnits / 2;

    retroTerminal = RetroTerminal.ShortDos(width, height, UnityEngine.Screen.width, UnityEngine.Screen.height, retroCanvas);
    retroTerminal.running = true;

    retroTerminal.Push(new MainMenuScreen(content));
  }

  void Update()
  {
    float dt = Time.deltaTime;

    if (retroTerminal != null)
      retroTerminal.Tick(dt);
  }
}
