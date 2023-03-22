using System;
using System.Collections.Generic;
using System.Linq;
using UnityTerminal;

/// Base class for a centered modal dialog.
abstract class Popup : Screen {
  public override bool isTransparent => true;

  /// The width of the content area of the popup.
  ///
  /// If not overridden, is calculated from the width of the longest line in
  /// [message].
  int? width => null;

  /// The height of the content area of the popup.
  ///
  /// If not overridden, is calculated from the number of lines in [message].
  int? height => null;

  /// Override this to return a list of lines of text that should be shown
  /// centered at the top of the popup.
  List<string> message => null;

  Dictionary<string, string> helpKeys;

  public override void Render() {
    Draw.helpKeys(terminal, helpKeys);

    var messageLines = message;

    var widestLine = 0;
    var lineCount = 0;
    if (messageLines != null) {
      widestLine = messageLines.fold<string>(
          0, (width, line) => Math.Max(width, line.Length));
      lineCount = messageLines.Count;
    }

    // If the width and height aren't specified, make it big enough to contain
    // the message with a margin around it.
    var popupWidth = width ?? widestLine + 2;
    var popupHeight = height ?? lineCount + 2;

    // Horizontally centered and a third of the way from the top.
    var top = (terminal.height - popupHeight) / 3;
    var left = (terminal.width - popupWidth) / 2;
    Draw.doubleBox(
        terminal, left - 1, top - 1, popupWidth + 2, popupHeight + 2, Hues.gold);

    var p = terminal.Rect(left, top, popupWidth, popupHeight);
    terminal.Clear(p);

    // Draw the message if there is one.
    if (messageLines != null) {
      var widest = messageLines.fold<string>(
          0, (width, line) => Math.Max(width, line.Length));
      var x = (terminal.width - widest) / 2;
      var y = 1;

      foreach (var line in messageLines) {
        terminal.WriteAt(p, x, y, line, UIHue.text);
        y++;
      }
    }

    renderPopup(terminal);
  }

  void renderPopup(Terminal terminal) {}
}