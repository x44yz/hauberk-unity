using System;
using System.Collections.Generic;

namespace Malison
{
  public class KeyBindings<T> {
    /// The high-level inputs and the low level keyboard bindings that are mapped
    /// to them.
    public Dictionary<_KeyBinding, T> _bindings = new Dictionary<_KeyBinding, T>();

    public void bind(T input, int keyCode, bool shift = false, bool alt = false) {
      _bindings[new _KeyBinding(keyCode, shift: shift, alt: alt)] = input;
    }

    public T find(int keyCode, bool shift = false, bool alt = false) {
      return _bindings[new _KeyBinding(keyCode, shift: shift, alt: alt)];
    }
  }

  /// Defines a specific key input (character code and modifier keys) that can be
  /// bound to a higher-level input in the application domain.
  public class _KeyBinding {
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

    public static UnityEngine.KeyCode toUnityKeyCode(int key)
    {
      if (key == KeyCode.delete) return UnityEngine.KeyCode.Delete;
      else if (key == tab) return UnityEngine.KeyCode.Tab;
      else if (key == enter) return UnityEngine.KeyCode.Return;
      else if (key == shift) return UnityEngine.KeyCode.LeftShift;
      else if (key == control) return UnityEngine.KeyCode.LeftControl;
      else if (key == option) return UnityEngine.KeyCode.LeftWindows;
      else if (key == escape) return UnityEngine.KeyCode.Escape;
      else if (key == space) return UnityEngine.KeyCode.Space;
      else if (key == left) return UnityEngine.KeyCode.LeftArrow;
      else if (key == up) return UnityEngine.KeyCode.UpArrow;
      else if (key == right) return UnityEngine.KeyCode.RightArrow;
      else if (key == down) return UnityEngine.KeyCode.DownArrow;
      else if (key == zero) return UnityEngine.KeyCode.Alpha0;
      else if (key == one) return UnityEngine.KeyCode.Alpha1;
      else if (key == two) return UnityEngine.KeyCode.Alpha2;
      else if (key == three) return UnityEngine.KeyCode.Alpha3;
      else if (key == four) return UnityEngine.KeyCode.Alpha4;
      else if (key == five) return UnityEngine.KeyCode.Alpha5;
      else if (key == six) return UnityEngine.KeyCode.Alpha6;
      else if (key == seven) return UnityEngine.KeyCode.Alpha7;
      else if (key == eight) return UnityEngine.KeyCode.Alpha8;
      else if (key == nine) return UnityEngine.KeyCode.Alpha9;
      else if (key == a) return UnityEngine.KeyCode.A;
      else if (key == b) return UnityEngine.KeyCode.B;
      else if (key == c) return UnityEngine.KeyCode.C;
      else if (key == d) return UnityEngine.KeyCode.D;
      else if (key == e) return UnityEngine.KeyCode.E;
      else if (key == f) return UnityEngine.KeyCode.F;
      else if (key == g) return UnityEngine.KeyCode.G;
      else if (key == h) return UnityEngine.KeyCode.H;
      else if (key == i) return UnityEngine.KeyCode.I;
      else if (key == j) return UnityEngine.KeyCode.J;
      else if (key == k) return UnityEngine.KeyCode.K;
      else if (key == l) return UnityEngine.KeyCode.L;
      else if (key == m) return UnityEngine.KeyCode.M;
      else if (key == n) return UnityEngine.KeyCode.N;
      else if (key == o) return UnityEngine.KeyCode.O;
      else if (key == p) return UnityEngine.KeyCode.P;
      else if (key == q) return UnityEngine.KeyCode.Q;
      else if (key == r) return UnityEngine.KeyCode.R;
      else if (key == s) return UnityEngine.KeyCode.S;
      else if (key == t) return UnityEngine.KeyCode.T;
      else if (key == u) return UnityEngine.KeyCode.U;
      else if (key == v) return UnityEngine.KeyCode.V;
      else if (key == w) return UnityEngine.KeyCode.W;
      else if (key == x) return UnityEngine.KeyCode.X;
      else if (key == y) return UnityEngine.KeyCode.Y;
      else if (key == z) return UnityEngine.KeyCode.Z;
      else if (key == numpad0) return UnityEngine.KeyCode.Keypad0;
      else if (key == numpad1) return UnityEngine.KeyCode.Keypad1;
      else if (key == numpad2) return UnityEngine.KeyCode.Keypad2;
      else if (key == numpad3) return UnityEngine.KeyCode.Keypad3;
      else if (key == numpad4) return UnityEngine.KeyCode.Keypad4;
      else if (key == numpad5) return UnityEngine.KeyCode.Keypad5;
      else if (key == numpad6) return UnityEngine.KeyCode.Keypad6;
      else if (key == numpad7) return UnityEngine.KeyCode.Keypad7;
      else if (key == numpad8) return UnityEngine.KeyCode.Keypad8;
      else if (key == numpad9) return UnityEngine.KeyCode.Keypad9;
      // else if (key == numpadClear) return UnityEngine.KeyCode
      // else if (key == numpadMultiply) return UnityEngine.KeyCode
      // else if (key == numpadAdd) return UnityEngine.KeyCode
      // else if (key == numpadSubtract) return UnityEngine.KeyCode
      // else if (key == numpadDecimal) return UnityEngine.KeyCode
      // else if (key == numpadDivide) return UnityEngine.KeyCode
      // else if (key == numpadEquals) return UnityEngine.KeyCode
      // else if (key == numpadEnter) return UnityEngine.KeyCode
      // else if (key == semicolon) return UnityEngine.KeyCode
      // else if (key == equals) return UnityEngine.KeyCode
      // else if (key == comma) return UnityEngine.KeyCode
      // else if (key == hyphen) return UnityEngine.KeyCode
      // else if (key == period) return UnityEngine.KeyCode
      // else if (key == slash) return UnityEngine.KeyCode
      // else if (key == backtick) return UnityEngine.KeyCode
      // else if (key == leftBracket) return UnityEngine.KeyCode
      // else if (key == backslash) return UnityEngine.KeyCode
      // else if (key == rightBracket) return UnityEngine.KeyCode
      // else if (key == apostrophe) return UnityEngine.KeyCode
      else
        UnityEngine.Debug.Log("not implement key > " + key);
      return UnityEngine.KeyCode.None;
    }
  }
}