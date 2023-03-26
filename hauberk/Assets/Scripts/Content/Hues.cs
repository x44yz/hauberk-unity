using System.Collections;
using System.Collections.Generic;
using UnityTerminal;

public class UIHue
{
  // TODO: These aren't very meaningful and are sort of randomly applied. Redo.
  public static Color text = Hues.lightWarmGray;
  public static Color helpText = Hues.lightWarmGray;
  public static Color selection = Hues.gold;
  public static Color disabled = Hues.darkCoolGray;
  public static Color primary = Hues.ash;
  public static Color secondary = Hues.darkCoolGray;
}

public static class Hues
{
  public static Color ash = new Color(0xe2, 0xdf, 0xf0);
  public static Color lightCoolGray = new Color(0x74, 0x92, 0xb5);
  public static Color coolGray = new Color(0x3f, 0x4b, 0x73);
  public static Color darkCoolGray = new Color(0x26, 0x2a, 0x42);
  public static Color darkerCoolGray = new Color(0x14, 0x13, 0x1f);

  public static Color lightWarmGray = new Color(0x84, 0x7e, 0x87);
  public static Color warmGray = new Color(0x48, 0x40, 0x4a);
  public static Color darkWarmGray = new Color(0x2a, 0x24, 0x2b);
  public static Color darkerWarmGray = new Color(0x16, 0x11, 0x17);

  public static Color sandal = new Color(0xbd, 0x90, 0x6c);
  public static Color tan = new Color(0x8e, 0x52, 0x37);
  public static Color brown = new Color(0x4d, 0x1d, 0x15);
  public static Color darkBrown = new Color(0x24, 0x0a, 0x05);

  public static Color gold = new Color(0xde, 0x9c, 0x21);
  public static Color carrot = new Color(0xb3, 0x4a, 0x04);
  public static Color persimmon = new Color(0x6e, 0x20, 0x0d);

  public static Color buttermilk = new Color(0xff, 0xee, 0xa8);
  public static Color yellow = new Color(0xe8, 0xc8, 0x15);
  public static Color olive = new Color(0x63, 0x57, 0x07);
  public static Color darkOlive = new Color(0x33, 0x30, 0x1c);

  public static Color mint = new Color(0x81, 0xd9, 0x75);
  public static Color lima = new Color(0x83, 0x9e, 0x0d);
  public static Color peaGreen = new Color(0x16, 0x75, 0x26);
  public static Color sherwood = new Color(0x00, 0x40, 0x27);

  public static Color lightAqua = new Color(0x81, 0xe7, 0xeb);
  public static Color aqua = new Color(0x0f, 0x82, 0x94);
  public static Color darkAqua = new Color(0x06, 0x31, 0x4f);

  public static Color lightBlue = new Color(0x40, 0xa3, 0xe5);
  public static Color blue = new Color(0x15, 0x57, 0xc2);
  public static Color darkBlue = new Color(0x1a, 0x2e, 0x96);

  public static Color lavender = new Color(0xc9, 0xa6, 0xff);
  public static Color lilac = new Color(0xad, 0x58, 0xdb);
  public static Color purple = new Color(0x56, 0x1e, 0x8a);
  public static Color violet = new Color(0x38, 0x10, 0x7d);

  public static Color pink = new Color(0xff, 0x7a, 0x69);
  public static Color red = new Color(0xcc, 0x23, 0x39);
  public static Color maroon = new Color(0x54, 0x00, 0x27);

  public static Dictionary<Element, Color> _elementColors = new Dictionary<Element, Color>(){
        {Element.none, lightCoolGray},
        {Elements.air, lightAqua},
        {Elements.earth, tan},
        {Elements.fire, red},
        {Elements.water, darkBlue},
        {Elements.acid, lima},
        {Elements.cold, lightBlue},
        {Elements.lightning, lilac},
        {Elements.poison, peaGreen},
        {Elements.dark, darkCoolGray},
        {Elements.light, buttermilk},
        {Elements.spirit, purple}
    };

  public static Color elementColor(Element element)
  {
    return _elementColors[element]!;
  }
}
