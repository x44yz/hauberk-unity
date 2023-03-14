using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Malison;

class MainScreen : Screen<string> {
  public List<Ball> balls = new List<Ball>();

  public MainScreen() {
    var colors = new List<Malison.Color>{
      Malison.Color.red,
      Malison.Color.orange,
      Malison.Color.gold,
      Malison.Color.yellow,
      Malison.Color.green,
      Malison.Color.aqua,
      Malison.Color.blue,
      Malison.Color.purple
    };

    foreach (var _char in "0123456789") {
      foreach (var color in colors) {
        balls.Add(new Ball(
            color,
            _char,
            UnityEngine.Random.Range(0f, 1.0f) * Ball.pitWidth,
            UnityEngine.Random.Range(0f, 1.0f) * (Ball.pitHeight / 2.0f),
            UnityEngine.Random.Range(0f, 1.0f) + 0.2f,
            0.0f));
      }
    }
  }

  public override bool handleInput(string input) {
    // switch (input) {
    //   case "next terminal":
    //     terminalIndex = (terminalIndex + 1) % terminals.length;
    //     updateTerminal();
    //     ui.refresh();
    //     break;

    //   case "prev terminal":
    //     terminalIndex = (terminalIndex - 1) % terminals.length;
    //     updateTerminal();
    //     ui.refresh();
    //     break;

    //   case "animate":
    //     ui.running = !ui.running;
    //     break;

    //   case "profile":
    //     profile();
    //     break;

    //   default:
    //     return false;
    // }

    return true;
  }

  void profile() {
    ui.running = true;
    for (var i = 0; i < 1000; i++) {
      update();
      ui.refresh();
    }
  }

  public override void update() {
    foreach (var ball in balls) {
      ball.update();
    }

    dirty();
  }

  public override void render(Terminal terminal) {
    terminal.clear();

    void colorBar(int y, string name, Malison.Color light, Malison.Color medium, Malison.Color dark) {
      terminal.writeAt(2, y, name, Malison.Color.gray);
      terminal.writeAt(10, y, "light", light);
      terminal.writeAt(16, y, "medium", medium);
      terminal.writeAt(23, y, "dark", dark);

      terminal.writeAt(28, y, " light ", Malison.Color.black, light);
      terminal.writeAt(35, y, " medium ", Malison.Color.black, medium);
      terminal.writeAt(43, y, " dark ", Malison.Color.black, dark);
    }

    terminal.writeAt(0, 0, "Predefined colors:");
    terminal.writeAt(59, 0, "switch terminal [tab]", Malison.Color.darkGray);
    terminal.writeAt(75, 0, "[tab]", Malison.Color.lightGray);
    colorBar(1, "gray", Malison.Color.lightGray, Malison.Color.gray, Malison.Color.darkGray);
    colorBar(2, "red", Malison.Color.lightRed, Malison.Color.red, Malison.Color.darkRed);
    colorBar(3, "orange", Malison.Color.lightOrange, Malison.Color.orange, Malison.Color.darkOrange);
    colorBar(4, "gold", Malison.Color.lightGold, Malison.Color.gold, Malison.Color.darkGold);
    colorBar(5, "yellow", Malison.Color.lightYellow, Malison.Color.yellow, Malison.Color.darkYellow);
    colorBar(6, "green", Malison.Color.lightGreen, Malison.Color.green, Malison.Color.darkGreen);
    colorBar(7, "aqua", Malison.Color.lightAqua, Malison.Color.aqua, Malison.Color.darkAqua);
    colorBar(8, "blue", Malison.Color.lightBlue, Malison.Color.blue, Malison.Color.darkBlue);
    colorBar(9, "purple", Malison.Color.lightPurple, Malison.Color.purple, Malison.Color.darkPurple);
    colorBar(10, "brown", Malison.Color.lightBrown, Malison.Color.brown, Malison.Color.darkBrown);

    terminal.writeAt(0, 12, "Code page 437:");
    var lines = new string[]{
      " ☺☻♥♦♣♠•◘○◙♂♀♪♫☼",
      "►◄↕‼¶§▬↨↑↓→←∟↔▲▼",
      " !\"#\\$%&'()*+,-./",
      "0123456789:;<=>?",
      "@ABCDEFGHIJKLMNO",
      "PQRSTUVWXYZ[\\]^_",
      "`abcdefghijklmno",
      "pqrstuvwxyz{|}~⌂",
      "ÇüéâäàåçêëèïîìÄÅ",
      "ÉæÆôöòûùÿÖÜ¢£¥₧ƒ",
      "áíóúñÑªº¿⌐¬½¼¡«»",
      "░▒▓│┤╡╢╖╕╣║╗╝╜╛┐",
      "└┴┬├─┼╞╟╚╔╩╦╠═╬╧",
      "╨╤╥╙╘╒╓╫╪┘┌█▄▌▐▀",
      "αßΓπΣσµτΦΘΩδ∞φε∩",
      "≡±≥≤⌠⌡÷≈°∙·√ⁿ²■"
    };

    var y = 13;
    foreach (var line in lines) {
      terminal.writeAt(3, y++, line, Malison.Color.lightGray);
    }

    terminal.writeAt(22, 12, "Simple game loop:");
    terminal.writeAt(66, 12, "toggle [space]", Malison.Color.darkGray);
    terminal.writeAt(73, 12, "[space]", Malison.Color.lightGray);

    foreach (var ball in balls) {
      ball.render(terminal);
    }
  }
}

class Ball {
  public const float pitWidth = 56.0f;
  public const float pitHeight = 17.0f;

  public Malison.Color color;
  public int charCode;

  float x, y, h, v;

  public Ball(Malison.Color color, int charCode, 
        float x, float y, float h, float v)
  {
    this.color = color;
    this.charCode = charCode;
    this.x = x;
    this.y = y;
    this.h = h;
    this.v = v;
  }

  public void update() {
    x += h;
    if (x < 0.0) {
      x = -x;
      h = -h;
    } else if (x > pitWidth) {
      x = pitWidth - x + pitWidth;
      h = -h;
    }

    v += 0.03f;
    y += v;
    if (y > pitHeight) {
      y = pitHeight - y + pitHeight;
      v = -v;
    }
  }

  public void render(Terminal terminal) {
    terminal.drawChar(24 + (int)x, 13 + (int)y, charCode, color);
  }
}

public class TestMalison : MonoBehaviour
{
    public const int width = 80;
    public const int height = 30;

    public UserInterface<string> ui = new UserInterface<string>();

    /// A few different terminals to choose from.
    public Terminal terminals(int idx) {
        if (idx == 0) return RetroTerminal.dos(width, height);
        else if (idx == 1) return RetroTerminal.shortDos(width, height);
        else if (idx == 2) return CanvasTerminal.create(width, height,
        new Malison.Font("Menlo, Consolas", size: 12, w: 8, h: 14, x: 1, y: 11));
        else if (idx == 3) return CanvasTerminal.create(
        width, height, new Malison.Font("Courier", size: 13, w: 10, h: 15, x: 1, y: 11));
        else if (idx == 4) return CanvasTerminal.create(
        width, height, new Malison.Font("Courier", size: 12, w: 8, h: 14, x: 1, y: 10));
        UnityEngine.Debug.LogError("not sumpper > " + idx);
        return null;
    }

    /// Index of the current terminal in [terminals].
    int terminalIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Set up the keybindings.
        ui.keyPress.bind("next terminal", Malison.KeyCode.tab);
        ui.keyPress.bind("prev terminal", Malison.KeyCode.tab, shift: true);
        ui.keyPress.bind("animate", Malison.KeyCode.space);
        ui.keyPress.bind("profile", Malison.KeyCode.p);

        updateTerminal();

        ui.push(new MainScreen());

        ui.handlingInput = true;
        ui.running = true;
    }

    void updateTerminal() {
        // html.document.body!.children.clear();
        ui.setTerminal((RenderableTerminal)terminals(terminalIndex));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
