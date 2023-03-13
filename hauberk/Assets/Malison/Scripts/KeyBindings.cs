using System;
using System.Collections.Generic;

namespace Malison
{
  class KeyBindings<T> {
    /// The high-level inputs and the low level keyboard bindings that are mapped
    /// to them.
    public Dictionary<_KeyBinding, T> _bindings = new Dictionary<_KeyBinding, T>();

    void bind(T input, int keyCode, bool shift = false, bool alt = false) {
      _bindings[new _KeyBinding(keyCode, shift: shift, alt: alt)] = input;
    }

    T? find(int keyCode, bool shift = false, bool alt = false) {
      return _bindings[new _KeyBinding(keyCode, shift: shift, alt: alt)];
    }
  }

  /// Defines a specific key input (character code and modifier keys) that can be
  /// bound to a higher-level input in the application domain.
  class _KeyBinding {
    /// The character code this is bound to.
    public int charCode;

    /// Whether this key binding requires the shift modifier key to be pressed.
    public bool shift;

    // TODO: Mac-specific. What should this be?
    /// Whether this key binding requires the alt modifier key to be pressed.
    public bool alt;

    public _KeyBinding(int charCode, bool shift, bool alt)
    {
      this.charCode = charCode; 
      this.shift = shift;
      this.alt = alt;
    }

    public static bool operator ==(_KeyBinding a, _KeyBinding b) {
        return a.charCode == b.charCode &&
            a.shift == b.shift &&
            a.alt == b.alt;
    }

    public static bool operator !=(_KeyBinding a, _KeyBinding b) {
        return (a == b) == false;
    }

    int hashCode => charCode.GetHashCode() ^ shift.GetHashCode() ^ alt.GetHashCode();

    string toString() {
      var result = $"key({charCode}";
      if (shift) result += " shift";
      if (alt) result += " alt";
      return result + ")";
    }
  }

  /// Raw key codes. These come straight from the DOM events.
  class KeyCode {
    public const int delete = 8;
    public const int tab = 9;
    public const int enter = 13;
    public const int shift = 16;
    public const int control = 17;
    public const int option = 18;
    public const int escape = 27;
    public const int space = 32;

    public const int left = 37;
    public const int up = 38;
    public const int right = 39;
    public const int down = 40;

    public const int zero = 48;
    public const int one = 49;
    public const int two = 50;
    public const int three = 51;
    public const int four = 52;
    public const int five = 53;
    public const int six = 54;
    public const int seven = 55;
    public const int eight = 56;
    public const int nine = 57;

    public const int a = 65;
    public const int b = 66;
    public const int c = 67;
    public const int d = 68;
    public const int e = 69;
    public const int f = 70;
    public const int g = 71;
    public const int h = 72;
    public const int i = 73;
    public const int j = 74;
    public const int k = 75;
    public const int l = 76;
    public const int m = 77;
    public const int n = 78;
    public const int o = 79;
    public const int p = 80;
    public const int q = 81;
    public const int r = 82;
    public const int s = 83;
    public const int t = 84;
    public const int u = 85;
    public const int v = 86;
    public const int w = 87;
    public const int x = 88;
    public const int y = 89;
    public const int z = 90;

    public const int numpad0 = 96;
    public const int numpad1 = 97;
    public const int numpad2 = 98;
    public const int numpad3 = 99;
    public const int numpad4 = 100;
    public const int numpad5 = 101;
    public const int numpad6 = 102;
    public const int numpad7 = 103;
    public const int numpad8 = 104;
    public const int numpad9 = 105;
    public const int numpadClear = 12;
    public const int numpadMultiply = 106;
    public const int numpadAdd = 107;
    public const int numpadSubtract = 109;
    public const int numpadDecimal = 110;
    public const int numpadDivide = 111;
    public const int numpadEquals = 1000;
    public const int numpadEnter = 1001;

    public const int semicolon = 186;
    public const int equals = 187;
    public const int comma = 188;
    public const int hyphen = 189;
    public const int period = 190;
    public const int slash = 191;
    public const int backtick = 192;
    public const int leftBracket = 219;
    public const int backslash = 220;
    public const int rightBracket = 221;
    public const int apostrophe = 222;
  }
}