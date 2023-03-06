using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DartUtils
{
    public static void assert(bool v, string msg = null)
    {
        if (msg != null)
            Debug.Assert(v, msg);
        else
            Debug.Assert(v);
    }

    public static void print(string s)
    {
        Debug.Log(s);
    }
}
