using System;
using UnityTerminal;

abstract class XPanel {
  Rect _bounds;

  public virtual bool isVisible => _bounds != null;

  /// The bounding box for the panel.
  ///
  /// This can only be called if the panel is visible.
  public virtual Rect bounds => _bounds!;

  void hide() {
    _bounds = null;
  }

  void show(Rect bounds) {
    _bounds = bounds;
  }

  void render(Terminal terminal) {
    if (!isVisible) return;
    renderPanel(terminal, terminal.Rect(bounds.x, bounds.y, bounds.width, bounds.height));
  }

  public abstract void renderPanel(Terminal terminal, Panel p);
}
