using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DartUtils
{
    public static void assert(bool v)
    {
        Debug.Assert(v);
    }

    public static void print(string s)
    {
        Debug.Log(s);
    }
}
