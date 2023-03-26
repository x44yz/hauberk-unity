using System;
using UnityTerminal;

abstract class Panel
{
  Rect _bounds;

  public virtual bool isVisible => _bounds != null;

  /// The bounding box for the panel.
  ///
  /// This can only be called if the panel is visible.
  public virtual Rect bounds => _bounds!;

  public void hide()
  {
    _bounds = null;
  }

  public void show(Rect bounds)
  {
    _bounds = bounds;
  }

  public void render(Terminal terminal)
  {
    if (!isVisible) return;
    renderPanel(terminal.Rect(bounds.x, bounds.y, bounds.width, bounds.height));
  }

  public abstract void renderPanel(Terminal terminal);
}
