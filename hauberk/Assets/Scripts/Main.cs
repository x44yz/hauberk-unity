using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTerminal;

public class Main : MonoBehaviour
{
    public RetroCanvas retroCanvas;
    public int width;
    public int height;

    [Header("RUNTIME")]
    // public float terminalScale;
    public RetroTerminal retroTerminal;

    // Start is called before the first frame update
    void Start()
    {
        var content = GameContent.createContent();

        // _addFont("8x8", 8);
        // _addFont("8x10", 8, 10);
        // _addFont("9x12", 9, 12);
        // _addFont("10x12", 10, 12);
        // _addFont("16x16", 16);
        // _addFont("16x20", 16, 20);

        // // Load the user's font preference, if any.
        // var fontName = html.window.localStorage["font"];
        // _font = _fonts[1];
        // for (var thisFont in _fonts) {
        //     if (thisFont.name == fontName) {
        //     _font = thisFont;
        //     break;
        //     }
        // }

        // var div = html.querySelector("#game")!;
        // div.append(_font.canvas);

        // // Scale the terminal to fit the screen.
        // html.window.onResize.listen((_) {
        //     _resizeTerminal();
        // });

        // _ui = UserInterface<Input>(_font.terminal);

        // // Set up the keyPress.
        // _ui.keyPress.bind(Input.ok, KeyCode.enter);
        // _ui.keyPress.bind(Input.cancel, KeyCode.escape);
        // _ui.keyPress.bind(Input.cancel, KeyCode.backtick);
        // _ui.keyPress.bind(Input.forfeit, KeyCode.f, shift: true);
        // _ui.keyPress.bind(Input.quit, KeyCode.q);

        // _ui.keyPress.bind(Input.open, KeyCode.c, shift: true);
        // _ui.keyPress.bind(Input.close, KeyCode.c);
        // _ui.keyPress.bind(Input.drop, KeyCode.d);
        // _ui.keyPress.bind(Input.use, KeyCode.u);
        // _ui.keyPress.bind(Input.pickUp, KeyCode.g);
        // _ui.keyPress.bind(Input.swap, KeyCode.x);
        // _ui.keyPress.bind(Input.equip, KeyCode.e);
        // _ui.keyPress.bind(Input.toss, KeyCode.t);
        // _ui.keyPress.bind(Input.selectSkill, KeyCode.s);
        // _ui.keyPress.bind(Input.heroInfo, KeyCode.a);
        // _ui.keyPress.bind(Input.editSkills, KeyCode.s, shift: true);

        // // Laptop directions.
        // _ui.keyPress.bind(Input.nw, KeyCode.i);
        // _ui.keyPress.bind(Input.n, KeyCode.o);
        // _ui.keyPress.bind(Input.ne, KeyCode.p);
        // _ui.keyPress.bind(Input.w, KeyCode.k);
        // _ui.keyPress.bind(Input.e, KeyCode.semicolon);
        // _ui.keyPress.bind(Input.sw, KeyCode.comma);
        // _ui.keyPress.bind(Input.s, KeyCode.period);
        // _ui.keyPress.bind(Input.se, KeyCode.slash);
        // _ui.keyPress.bind(Input.runNW, KeyCode.i, shift: true);
        // _ui.keyPress.bind(Input.runN, KeyCode.o, shift: true);
        // _ui.keyPress.bind(Input.runNE, KeyCode.p, shift: true);
        // _ui.keyPress.bind(Input.runW, KeyCode.k, shift: true);
        // _ui.keyPress.bind(Input.runE, KeyCode.semicolon, shift: true);
        // _ui.keyPress.bind(Input.runSW, KeyCode.comma, shift: true);
        // _ui.keyPress.bind(Input.runS, KeyCode.period, shift: true);
        // _ui.keyPress.bind(Input.runSE, KeyCode.slash, shift: true);
        // _ui.keyPress.bind(Input.fireNW, KeyCode.i, alt: true);
        // _ui.keyPress.bind(Input.fireN, KeyCode.o, alt: true);
        // _ui.keyPress.bind(Input.fireNE, KeyCode.p, alt: true);
        // _ui.keyPress.bind(Input.fireW, KeyCode.k, alt: true);
        // _ui.keyPress.bind(Input.fireE, KeyCode.semicolon, alt: true);
        // _ui.keyPress.bind(Input.fireSW, KeyCode.comma, alt: true);
        // _ui.keyPress.bind(Input.fireS, KeyCode.period, alt: true);
        // _ui.keyPress.bind(Input.fireSE, KeyCode.slash, alt: true);

        // _ui.keyPress.bind(Input.ok, KeyCode.l);
        // _ui.keyPress.bind(Input.rest, KeyCode.l, shift: true);
        // _ui.keyPress.bind(Input.fire, KeyCode.l, alt: true);

        // // Arrow keys.
        // _ui.keyPress.bind(Input.n, KeyCode.up);
        // _ui.keyPress.bind(Input.w, KeyCode.left);
        // _ui.keyPress.bind(Input.e, KeyCode.right);
        // _ui.keyPress.bind(Input.s, KeyCode.down);
        // _ui.keyPress.bind(Input.runN, KeyCode.up, shift: true);
        // _ui.keyPress.bind(Input.runW, KeyCode.left, shift: true);
        // _ui.keyPress.bind(Input.runE, KeyCode.right, shift: true);
        // _ui.keyPress.bind(Input.runS, KeyCode.down, shift: true);
        // _ui.keyPress.bind(Input.fireN, KeyCode.up, alt: true);
        // _ui.keyPress.bind(Input.fireW, KeyCode.left, alt: true);
        // _ui.keyPress.bind(Input.fireE, KeyCode.right, alt: true);
        // _ui.keyPress.bind(Input.fireS, KeyCode.down, alt: true);

        // // Numeric keypad.
        // _ui.keyPress.bind(Input.nw, KeyCode.numpad7);
        // _ui.keyPress.bind(Input.n, KeyCode.numpad8);
        // _ui.keyPress.bind(Input.ne, KeyCode.numpad9);
        // _ui.keyPress.bind(Input.w, KeyCode.numpad4);
        // _ui.keyPress.bind(Input.e, KeyCode.numpad6);
        // _ui.keyPress.bind(Input.sw, KeyCode.numpad1);
        // _ui.keyPress.bind(Input.s, KeyCode.numpad2);
        // _ui.keyPress.bind(Input.se, KeyCode.numpad3);
        // _ui.keyPress.bind(Input.runNW, KeyCode.numpad7, shift: true);
        // _ui.keyPress.bind(Input.runN, KeyCode.numpad8, shift: true);
        // _ui.keyPress.bind(Input.runNE, KeyCode.numpad9, shift: true);
        // _ui.keyPress.bind(Input.runW, KeyCode.numpad4, shift: true);
        // _ui.keyPress.bind(Input.runE, KeyCode.numpad6, shift: true);
        // _ui.keyPress.bind(Input.runSW, KeyCode.numpad1, shift: true);
        // _ui.keyPress.bind(Input.runS, KeyCode.numpad2, shift: true);
        // _ui.keyPress.bind(Input.runSE, KeyCode.numpad3, shift: true);

        // _ui.keyPress.bind(Input.ok, KeyCode.numpad5);
        // _ui.keyPress.bind(Input.ok, KeyCode.numpadEnter);
        // _ui.keyPress.bind(Input.rest, KeyCode.numpad5, shift: true);
        // _ui.keyPress.bind(Input.rest, KeyCode.numpadEnter, shift: true);
        // _ui.keyPress.bind(Input.fire, KeyCode.numpad5, alt: true);

        // _ui.keyPress.bind(Input.wizard, KeyCode.w, shift: true, alt: true);

        // _ui.push(MainMenuScreen(content));

        // _ui.handlingInput = true;
        // _ui.running = true;

        // if (Debug.enabled) {
        //     html.document.body!.onKeyDown.listen((_) {
        //     _refreshDebugBoxes();
        //     });
        // }

        Camera.main.orthographicSize = UnityEngine.Screen.height / retroCanvas.pixelToUnits / 2;

        // terminalScale = UnityEngine.Screen.height * 1f / height / 13.0f;
        retroTerminal = RetroTerminal.ShortDos(width, height, UnityEngine.Screen.width, UnityEngine.Screen.height, retroCanvas);
        retroTerminal.running = true;

        retroTerminal.Push(new MainMenuScreen(content));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
