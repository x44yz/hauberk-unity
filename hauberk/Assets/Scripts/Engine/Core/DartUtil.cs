using System.Collections;
using System.Collections.Generic;

public static class DartUtils
{
    public static void assert(bool v, string msg = null)
    {
        if (msg != null)
            UnityEngine.Debug.Assert(v, msg);
        else
            UnityEngine.Debug.Assert(v);
    }

    public static void print(string s)
    {
        UnityEngine.Debug.Log(s);
    }
}
