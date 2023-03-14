using System;
using System.Collections.Generic;

namespace Malison
{
  /// A simple modal user interface layer.
  ///
  /// It maintains a stack of screens. All screens in the stack update. Screens
  /// may indicate if they are opaque or transparent. Transparent screens allow
  /// the screen under them to render.
  ///
  /// In addition, the interface can define a number of global [KeyBindings]
  /// which screens can use to map raw keypresses to something higher-level.
  public class UserInterface<T> {
    /// Keyboard bindings for key press events.
    public KeyBindings<T> keyPress = new KeyBindings<T>();

    public List<Screen<T>> _screens = new List<Screen<T>>();
    public RenderableTerminal? _terminal;
    bool _dirty = true;

    /// Whether or not the UI is listening for keyboard events.
    ///
    /// Initially off.
    bool _handlingInput = false;
    public bool handlingInput 
    {
      get { return _handlingInput; }
      set {
        if (value == _handlingInput) return;
        _handlingInput = value;

        if (value) {
          // MalisonUnity.Inst.onKeyUpdate = onKeyUpdate;
        } else {
          // MalisonUnity.Inst.onKeyUpdate = null;
        }
      }
    }

    /// Whether or not the game loop is running and the UI is refreshing itself
    /// every frame.
    ///
    /// Initially off.
    ///
    /// If you want to manually refresh the UI yourself when you know it needs
    /// to be updated -- maybe your game is explicitly turn-based -- you can
    /// leave this off.
    bool _running = false;
    public bool running
    {
      get { return _running; }
      set {
        if (value == _running) return;

        _running = value;
        if (_running) {
          // MalisonUnity.Inst.onUpdate = _tick;
        } else {
          // MalisonUnity.Inst.onUpdate = null;
        }
      }
    }

    public UserInterface(RenderableTerminal _terminal = null)
    {
      this._terminal = _terminal;
    }

    public void setTerminal(RenderableTerminal terminal) {
      var resized = terminal != null &&
          (_terminal == null ||
              _terminal!.width != terminal.width ||
              _terminal!.height != terminal.height);

      _terminal = terminal;
      dirty();

      // If the terminal size changed, let the screens known.
      if (resized) {
        foreach (var screen in _screens)
          screen.resize(terminal.size);
      }
    }

    /// Pushes [screen] onto the top of the stack.
    public void push(Screen<T> screen) {
      screen._bind(this);
      _screens.Add(screen);
      _render();
    }

    /// Pops the top screen off the top of the stack.
    ///
    /// The next screen down is activated. If [result] is given, it is passed to
    /// the new active screen's [activate] method.
    void pop(object? result) {
      var screen = _screens[_screens.Count - 1];
      _screens.RemoveAt(_screens.Count - 1);
      screen._unbind();
      _screens[_screens.Count - 1].activate(screen, result);
      _render();
    }

    /// Switches the current top screen to [screen].
    ///
    /// This is equivalent to a [pop] followed by a [push].
    void goTo(Screen<T> screen) {
      var old = _screens[_screens.Count - 1];
      _screens.RemoveAt(_screens.Count - 1);
      old._unbind();

      screen._bind(this);
      _screens.Add(screen);
      _render();
    }

    public void dirty() {
      _dirty = true;
    }

    public void refresh() {
      // Don't use a for-in loop here so that we don't run into concurrent
      // modification exceptions if a screen is added or removed during a call to
      // update().
      for (var i = 0; i < _screens.Count; i++) {
        _screens[i].update();
      }
      if (_dirty) _render();
    }

    void onKeyUpdate()
    {
      _keyDown();
    }

    void _keyDown() {
      // var keyCode = event.keyCode;

      // // If the keypress happened on the numpad, translate the keyCode.
      // if (event.location == 3) {
      //   switch (keyCode) {
      //     case KeyCode.zero:
      //       keyCode = KeyCode.numpad0;
      //       break;
      //     case KeyCode.one:
      //       keyCode = KeyCode.numpad1;
      //       break;
      //     case KeyCode.two:
      //       keyCode = KeyCode.numpad2;
      //       break;
      //     case KeyCode.three:
      //       keyCode = KeyCode.numpad3;
      //       break;
      //     case KeyCode.four:
      //       keyCode = KeyCode.numpad4;
      //       break;
      //     case KeyCode.five:
      //       keyCode = KeyCode.numpad5;
      //       break;
      //     case KeyCode.six:
      //       keyCode = KeyCode.numpad6;
      //       break;
      //     case KeyCode.seven:
      //       keyCode = KeyCode.numpad7;
      //       break;
      //     case KeyCode.eight:
      //       keyCode = KeyCode.numpad8;
      //       break;
      //     case KeyCode.nine:
      //       keyCode = KeyCode.numpad9;
      //       break;
      //     case KeyCode.equals:
      //       keyCode = KeyCode.numpadEquals;
      //       break;
      //     case KeyCode.enter:
      //       keyCode = KeyCode.numpadEnter;
      //       break;
      //   }
      // }

      // Firefox uses 59 for semicolon.
      // if (keyCode == 59) keyCode = KeyCode.semicolon;

      foreach (var kv in keyPress._bindings)
      {
        if (UnityEngine.Input.GetKeyDown(KeyCode.toUnityKeyCode(kv.Key.charCode)))
        {
          var input =
              keyPress.find(kv.Key.charCode, 
              shift: UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.LeftShift), 
              alt: UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.LeftAlt));

          var screen = _screens[_screens.Count - 1];
          if (input != null) {
            // Bound keys are always consumed, even if the screen doesn't use it.
            // event.preventDefault();
            if (screen.handleInput(input)) return;
          }
        }
      }



      // if (screen.keyDown(keyCode, shift: event.shiftKey, alt: event.altKey)) {
      //   event.preventDefault();
      // }
      // if (screen.keyDown())
      // {

      // }
    }

    void _keyUp() {
      // var keyCode = event.keyCode;

      // Firefox uses 59 for semicolon.
      // if (keyCode == 59) keyCode = KeyCode.semicolon;

      // var screen = _screens.last;
      // if (screen.keyUp(keyCode, shift: event.shiftKey, alt: event.altKey)) {
      //   event.preventDefault();
      // }
    }

    /// Called every animation frame while the UI's game loop is running.
    public void _tick(float dt) {
      if (!_running)
        return;
      
      refresh();

      // if (_running) html.window.requestAnimationFrame(_tick);
    }

    void _render() {
      // If the UI isn't currently bound to a terminal, there's nothing to render.
      var terminal = _terminal;
      if (terminal == null) return;

      terminal.clear();

      // Skip past all of the covered screens.
      int i;
      for (i = _screens.Count - 1; i >= 0; i--) {
        if (!_screens[i].isTransparent) break;
      }

      if (i < 0) i = 0;

      // Render the top opaque screen and any transparent ones above it.
      for (; i < _screens.Count; i++) {
        _screens[i].render(terminal);
      }

      _dirty = false;
      terminal.render();
    }
  }

  public class Screen<T> {
    UserInterface<T>? _ui;

    /// The [UserInterface] this screen is bound to.
    ///
    /// Throws an exception if the screen is not currently bound to an interface.
    public UserInterface<T> ui => _ui!;

    /// Whether this screen is bound to a [UserInterface].
    ///
    /// If this is `false`, then [ui] cannot be accessed.
    bool isBound => _ui != null;

    /// Whether this screen allows any screens under it to be visible.
    ///
    /// Subclasses can override this. Defaults to `false`.
    public bool isTransparent => false;

    /// Binds this screen to [ui].
    public void _bind(UserInterface<T> ui) {
      DartUtils.assert(_ui == null);
      _ui = ui;

      resize(ui._terminal!.size);
    }

    /// Unbinds this screen from the [ui] that owns it.
    public void _unbind() {
      DartUtils.assert(_ui != null);
      _ui = null;
    }

    /// Marks the user interface as needing to be rendered.
    ///
    /// Call this during [update] to indicate that a subsequent call to [render]
    /// is needed.
    public void dirty() {
      // If we aren't bound (yet), just do nothing. The screen will be dirtied
      // when it gets bound.
      if (_ui == null) return;

      _ui!.dirty();
    }

    /// If a keypress has a binding defined for it and is pressed, this will be
    /// called with the bound input when this screen is active.
    ///
    /// If this returns `false` (the default), then the lower-level [keyDown]
    /// method will be called.
    public virtual bool handleInput(T input) => false;

    public virtual bool keyDown(int keyCode, bool shift, bool alt) => false;

    public virtual bool keyUp(int keyCode, bool shift, bool alt) => false;

    /// Called when the screen above this one ([popped]) has been popped and this
    /// screen is now the top-most screen. If a value was passed to [pop()], it
    /// will be passed to this as [result].
    public virtual void activate(Screen<T> popped, Object? result) {}

    public virtual void update() {}

    public virtual void render(Terminal terminal) {}

    /// Called when the [UserInterface] has been bound to a new terminal with a
    /// different size while this [Screen] is present.
    public virtual void resize(Vec size) {}
  }
}