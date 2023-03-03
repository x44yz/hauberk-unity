using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Direction : Vec {
    public static Direction none = new Direction(0, 0);
    public static Direction n = new Direction(0, -1);
    public static Direction ne = new Direction(1, -1);
    public static Direction e = new Direction(1, 0);
    public static Direction se = new Direction(1, 1);
    public static Direction s = new Direction(0, 1);
    public static Direction sw = new Direction(-1, 1);
    public static Direction w = new Direction(-1, 0);
    public static Direction nw = new Direction(-1, -1);

    /// The eight cardinal and intercardinal directions.
    public static Direction[] all = new Direction[]{n, ne, e, se, s, sw, w, nw};

    /// The four cardinal directions: north, south, east, and west.
    public static Direction[] cardinal = new Direction[]{n, e, s, w};

    /// The four directions between the cardinal ones: northwest, northeast,
    /// southwest and southeast.
    public static Direction[] intercardinal = new Direction[]{ne, se, sw, nw};

    public Direction(int x, int y) : base(x, y)
    {
    }

    Direction rotateLeft45 {
        get {
            switch (this) {
            case none:
                return none;
            case n:
                return nw;
            case ne:
                return n;
            case e:
                return ne;
            case se:
                return e;
            case s:
                return se;
            case sw:
                return s;
            case w:
                return sw;
            case nw:
                return w;
            }

            throw "unreachable";
        }
    }

    Direction rotateRight45 {
        switch (this) {
        case none:
            return none;
        case n:
            return ne;
        case ne:
            return e;
        case e:
            return se;
        case se:
            return s;
        case s:
            return sw;
        case sw:
            return w;
        case w:
            return nw;
        case nw:
            return n;
        }

        throw "unreachable";
    }

    Direction rotateLeft90 {
        switch (this) {
        case none:
            return none;
        case n:
            return w;
        case ne:
            return nw;
        case e:
            return n;
        case se:
            return ne;
        case s:
            return e;
        case sw:
            return se;
        case w:
            return s;
        case nw:
            return sw;
        }

        throw "unreachable";
    }

    Direction rotateRight90 {
        switch (this) {
        case none:
            return none;
        case n:
            return e;
        case ne:
            return se;
        case e:
            return s;
        case se:
            return sw;
        case s:
            return w;
        case sw:
            return nw;
        case w:
            return n;
        case nw:
            return ne;
        }

        throw "unreachable";
    }

    Direction rotate180 {
        get {
            switch (this) {
            case none:
                return none;
            case n:
                return s;
            case ne:
                return sw;
            case e:
                return w;
            case se:
                return nw;
            case s:
                return n;
            case sw:
                return ne;
            case w:
                return e;
            case nw:
                return se;
            }

            throw "unreachable";
        }
    }

    string toString() {
        switch (this) {
        case none:
            return "none";
        case n:
            return "n";
        case ne:
            return "ne";
        case e:
            return "e";
        case se:
            return "se";
        case s:
            return "s";
        case sw:
            return "sw";
        case w:
            return "w";
        case nw:
            return "nw";
        }

        throw "unreachable";
    }
}
