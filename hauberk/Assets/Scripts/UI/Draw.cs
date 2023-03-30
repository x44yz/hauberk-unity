using System;
using System.Collections.Generic;
using System.Linq;
using UnityTerminal;

public static class Draw
{
  public static void box(Terminal terminal, int x, int y, int width, int height,
      Color? color = null)
  {
    _box(terminal, x, y, width, height, color, "┌", "─", "┐", "│", "└", "─",
        "┘");
  }

  public static void frame(Terminal terminal, int x, int y, int width, int height,
      Color? color = null)
  {
    _box(terminal, x, y, width, height, color, "╒", "═", "╕", "│", "└", "─",
        "┘");
  }

  public static void doubleBox(Terminal terminal, int x, int y, int width, int height,
      Color? color = null)
  {
    _box(terminal, x, y, width, height, color, "╔", "═", "╗", "║", "╚", "═",
        "╝");
  }

  public static void helpKeys(Terminal terminal, Dictionary<string, string> helpKeys,
      string query = null)
  {
    // Draw the help.
    var helpTextLength = 0;
    foreach (var kv in helpKeys)
    {
      var key = kv.Key;
      var text = kv.Value;
      if (helpTextLength > 0) helpTextLength += 2;
      helpTextLength += key.Length + text.Length + 3;
    };

    var x = (terminal.width - helpTextLength) / 2;

    // Show the query string, if there is one.
    if (query != null)
    {
      box(terminal, x - 2, terminal.height - 4, helpTextLength + 4, 5,
          UIHue.text);
      terminal.WriteAt((terminal.width - query.Length) / 2,
          terminal.height - 3, query, UIHue.primary);
    }
    else
    {
      box(terminal, x - 2, terminal.height - 2, helpTextLength + 4, 3,
          UIHue.text);
    }

    var first = true;
    foreach (var kv in helpKeys)
    {
      var key = kv.Key;
      var text = kv.Value;
      if (!first)
      {
        terminal.WriteAt(x, terminal.height - 1, ", ", UIHue.secondary);
        x += 2;
      }

      terminal.WriteAt(x, terminal.height - 1, "[", UIHue.secondary);
      x++;
      terminal.WriteAt(x, terminal.height - 1, key, UIHue.selection);
      x += key.Length;
      terminal.WriteAt(x, terminal.height - 1, "] ", UIHue.secondary);
      x += 2;

      terminal.WriteAt(x, terminal.height - 1, text, UIHue.helpText);
      x += text.Length;

      first = false;
    };
  }

  static void _box(
      Terminal terminal,
      int x,
      int y,
      int width,
      int height,
      Color? color,
      String topLeft,
      String top,
      String topRight,
      String vertical,
      String bottomLeft,
      String bottom,
      String bottomRight)
  {
    color ??= Color.darkCoolGray;

    // left/right bar
    for (int i = 1; i < height - 1; ++i)
    {
      terminal.WriteAt(x, y + i, vertical, color);
      for (int j = 1; j < width - 1; ++j)
        terminal.WriteAt(x + j, y + i, CharCode.space, color);
      terminal.WriteAt(x + width - 1, y + i, vertical, color);
    }

    // top/bottom row
    for (int i = 0; i < width; ++i)
    {
      if (i == 0)
      {
        terminal.WriteAt(x + i, y, topLeft, color);
        terminal.WriteAt(x + i, y + height - 1, bottomLeft, color);
      }
      else if (i == width - 1)
      {
        terminal.WriteAt(x + i, y, topRight, color);
        terminal.WriteAt(x + i, y + height - 1, bottomRight, color);
      }
      else
      {
        terminal.WriteAt(x + i, y, top, color);
        terminal.WriteAt(x + i, y + height - 1, bottom, color);
      }
    }
  }

  /// Draws a progress bar to reflect [value]'s range between `0` and [max].
  /// Has a couple of special tweaks: the bar will only be empty if [value] is
  /// exactly `0`, otherwise it will at least show a sliver. Likewise, the bar
  /// will only be full if [value] is exactly [max], otherwise at least one
  /// half unit will be missing.
  public static void meter(
      Terminal terminal, int x, int y, int width, int value, int max,
      Color? fore = null, Color? back = null)
  {
    Debugger.assert(max != 0);

    fore ??= Hues.red;
    back ??= Hues.maroon;

    var barWidth = (int)Math.Round(width * 2f * value / max);

    // Edge cases, don't show an empty or full bar unless actually at the min
    // or max.
    if (barWidth == 0 && value > 0) barWidth = 1;
    if (barWidth == width * 2 && value < max) barWidth = width * 2 - 1;

    for (var i = 0; i < width; i++)
    {
      var char_ = CharCode.space;
      if (i < barWidth / 2)
      {
        char_ = CharCode.fullBlock;
      }
      else if (i < (barWidth + 1) / 2)
      {
        char_ = CharCode.leftHalfBlock;
      }
      terminal.WriteAt(x + i, y, char_, fore, back);
    }
  }

  // /// Draws a progress bar to reflect [value]'s range between `0` and [max].
  // /// Has a couple of special tweaks: the bar will only be empty if [value] is
  // /// exactly `0`, otherwise it will at least show a sliver. Likewise, the bar
  // /// will only be full if [value] is exactly [max], otherwise at least one
  // /// half unit will be missing.
  public static void thinMeter(
      Terminal terminal, int x, int y, int width, int value, int max,
      Color? fore = null, Color? back = null)
  {
    Debugger.assert(max != 0);

    fore ??= Hues.red;
    back ??= Hues.maroon;

    var barWidth = (int)Math.Round(width * value * 1f / max);

    // Edge cases, don't show an empty or full bar unless actually at the min
    // or max.
    if (barWidth == 0 && value > 0) barWidth = 1;
    if (barWidth == width && value < max) barWidth = width - 1;

    for (var i = 0; i < width; i++)
    {
      var color = i < barWidth ? fore : back;
      terminal.WriteAt(x + i, y, CharCode.lowerHalfBlock, color);
    }
  }
}
