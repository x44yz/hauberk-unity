namespace Malison
{
  // 保留
  /// Unicode code points for various special characters that also exist on
  /// [code page 437][font].
  ///
  /// [font]: http://en.wikipedia.org/wiki/Code_page_437
  class CharCode {
    // 1 - 15.
    public const int whiteSmilingFace = 0x263a;
    public const int blackSmilingFace = 0x263b;
    public const int blackHeartSuit = 0x2665;
    public const int blackDiamondSuit = 0x2666;
    public const int blackClubSuit = 0x2663;
    public const int blackSpadeSuit = 0x2660;
    public const int bullet = 0x2022;
    public const int inverseBullet = 0x25d8;
    public const int whiteCircle = 0x25cb;
    public const int inverseWhiteCircle = 0x25d9;
    public const int maleSign = 0x2642;
    public const int femaleSign = 0x2640;
    public const int eighthNote = 0x266a;
    public const int beamedEighthNotes = 0x266b;
    public const int whiteSunWithRays = 0x263c;

    // 16 - 31.
    public const int blackRightPointingPointer = 0x25ba;
    public const int blackLeftPointingPointer = 0x25c4;
    public const int upDownArrow = 0x2195;
    public const int doubleExclamationMark = 0x203c;
    public const int pilcrow = 0x00b6;
    public const int sectionSign = 0x00a7;
    public const int blackRectangle = 0x25ac;
    public const int upDownArrowWithBase = 0x21a8;
    public const int upwardsArrow = 0x2191;
    public const int downwardsArrow = 0x2193;
    public const int rightwardsArrow = 0x2192;
    public const int leftwardsArrow = 0x2190;
    public const int rightAngle = 0x221f;
    public const int leftRightArrow = 0x2194;
    public const int blackUpPointingTriangle = 0x25b2;
    public const int blackDownPointingTriangle = 0x25bc;

    // 32 - 47.
    public const int space = 0x0020;
    public const int exclamationPoint = 0x0021;
    public const int doubleQuote = 0x0022;
    public const int numberSign = 0x0023;
    public const int dollarSign = 0x0024;
    public const int percent = 0x0025;
    public const int ampersand = 0x0026;
    public const int apostrophe = 0x0027;
    public const int leftParenthesis = 0x0028;
    public const int rightParenthesis = 0x0029;
    public const int asterisk = 0x002a;
    public const int plus = 0x002b;
    public const int comma = 0x002c;
    public const int minus = 0x002d;
    public const int period = 0x002e;
    public const int slash = 0x002f;

    // 48 - 63.
    public const int zero = 0x0030;
    public const int one = 0x0031;
    public const int two = 0x0032;
    public const int three = 0x0033;
    public const int four = 0x0034;
    public const int five = 0x0035;
    public const int six = 0x0036;
    public const int seven = 0x0037;
    public const int eight = 0x0038;
    public const int nine = 0x0039;
    public const int colon = 0x003a;
    public const int semicolon = 0x003b;
    public const int lessThan = 0x003c;
    public const int equals = 0x003d;
    public const int greaterThan = 0x003e;
    public const int questionMark = 0x003f;

    // 64 - 95.
    public const int at = 0x0040;
    public const int aUpper = 0x0041;
    public const int bUpper = 0x0042;
    public const int cUpper = 0x0043;
    public const int dUpper = 0x0044;
    public const int eUpper = 0x0045;
    public const int fUpper = 0x0046;
    public const int gUpper = 0x0047;
    public const int hUpper = 0x0048;
    public const int iUpper = 0x0049;
    public const int jUpper = 0x004a;
    public const int kUpper = 0x004b;
    public const int lUpper = 0x004c;
    public const int mUpper = 0x004d;
    public const int nUpper = 0x004e;
    public const int oUpper = 0x004f;
    public const int pUpper = 0x0050;
    public const int qUpper = 0x0051;
    public const int rUpper = 0x0052;
    public const int sUpper = 0x0053;
    public const int tUpper = 0x0054;
    public const int uUpper = 0x0055;
    public const int vUpper = 0x0056;
    public const int wUpper = 0x0057;
    public const int xUpper = 0x0058;
    public const int yUpper = 0x0059;
    public const int zUpper = 0x005a;
    public const int leftBracket = 0x005b;
    public const int backSlash = 0x005c;
    public const int rightBracket = 0x005d;
    public const int caret = 0x005e;
    public const int underscore = 0x005f;

    // 96 - 127.
    public const int accent = 0x0060;
    public const int aLower = 0x0061;
    public const int bLower = 0x0062;
    public const int cLower = 0x0063;
    public const int dLower = 0x0064;
    public const int eLower = 0x0065;
    public const int fLower = 0x0066;
    public const int gLower = 0x0067;
    public const int hLower = 0x0068;
    public const int iLower = 0x0069;
    public const int jLower = 0x006a;
    public const int kLower = 0x006b;
    public const int lLower = 0x006c;
    public const int mLower = 0x006d;
    public const int nLower = 0x006e;
    public const int oLower = 0x006f;
    public const int pLower = 0x0070;
    public const int qLower = 0x0071;
    public const int rLower = 0x0072;
    public const int sLower = 0x0073;
    public const int tLower = 0x0074;
    public const int uLower = 0x0075;
    public const int vLower = 0x0076;
    public const int wLower = 0x0077;
    public const int xLower = 0x0078;
    public const int yLower = 0x0079;
    public const int zLower = 0x007a;
    public const int leftBrace = 0x007b;
    public const int pipe = 0x007c;
    public const int rightBrace = 0x007d;
    public const int tilde = 0x007e;
    public const int house = 0x2302;

    // 128 - 143.
    public const int latinCapitalLetterCWithCedilla = 0x00c7;
    public const int latinSmallLetterUWithDiaeresis = 0x00fc;
    public const int latinSmallLetterEWithAcute = 0x00e9;
    public const int latinSmallLetterAWithCircumflex = 0x00e2;
    public const int latinSmallLetterAWithDiaeresis = 0x00e4;
    public const int latinSmallLetterAWithGrave = 0x00e0;
    public const int latinSmallLetterAWithRingAbove = 0x00e5;
    public const int latinSmallLetterCWithCedilla = 0x00e7;
    public const int latinSmallLetterEWithCircumflex = 0x00ea;
    public const int latinSmallLetterEWithDiaeresis = 0x00eb;
    public const int latinSmallLetterEWithGrave = 0x00e8;
    public const int latinSmallLetterIWithDiaeresis = 0x00ef;
    public const int latinSmallLetterIWithCircumflex = 0x00ee;
    public const int latinSmallLetterIWithGrave = 0x00ec;
    public const int latinCapitalLetterAWithDiaeresis = 0x00c4;
    public const int latinCapitalLetterAWithRingAbove = 0x00c5;

    // 144 - 159.
    public const int latinCapitalLetterEWithAcute = 0x00c9;
    public const int latinSmallLetterAe = 0x00e6;
    public const int latinCapitalLetterAe = 0x00c6;
    public const int latinSmallLetterOWithCircumflex = 0x00f4;
    public const int latinSmallLetterOWithDiaeresis = 0x00f6;
    public const int latinSmallLetterOWithGrave = 0x00f2;
    public const int latinSmallLetterUWithCircumflex = 0x00fb;
    public const int latinSmallLetterUWithGrave = 0x00f9;
    public const int latinSmallLetterYWithDiaeresis = 0x00ff;
    public const int latinCapitalLetterOWithDiaeresis = 0x00d6;
    public const int latinCapitalLetterUWithDiaeresis = 0x00dc;
    public const int centSign = 0x00a2;
    public const int poundSign = 0x00a3;
    public const int yenSign = 0x00a5;
    public const int pesetaSign = 0x20a7;
    public const int latinSmallLetterFWithHook = 0x0192;

    // 160 - 175.
    public const int latinSmallLetterAWithAcute = 0x00e1;
    public const int latinSmallLetterIWithAcute = 0x00ed;
    public const int latinSmallLetterOWithAcute = 0x00f3;
    public const int latinSmallLetterUWithAcute = 0x00fa;
    public const int latinSmallLetterNWithTilde = 0x00f1;
    public const int latinCapitalLetterNWithTilde = 0x00d1;
    public const int feminineOrdinalIndicator = 0x00aa;
    public const int masculineOrdinalIndicator = 0x00ba;
    public const int invertedQuestionMark = 0x00bf;
    public const int reversedNotSign = 0x2310;
    public const int notSign = 0x00ac;
    public const int vulgarFractionOneHalf = 0x00bd;
    public const int vulgarFractionOneQuarter = 0x00bc;
    public const int invertedExclamationMark = 0x00a1;
    public const int leftPointingDoubleAngleQuotationMark = 0x00ab;
    public const int rightPointingDoubleAngleQuotationMark = 0x00bb;

    // 176 - 191.
    public const int lightShade = 0x2591;
    public const int mediumShade = 0x2592;
    public const int darkShade = 0x2593;
    public const int boxDrawingsLightVertical = 0x2502;
    public const int boxDrawingsLightVerticalAndLeft = 0x2524;
    public const int boxDrawingsVerticalSingleAndLeftDouble = 0x2561;
    public const int boxDrawingsVerticalDoubleAndLeftSingle = 0x2562;
    public const int boxDrawingsDownDoubleAndLeftSingle = 0x2556;
    public const int boxDrawingsDownSingleAndLeftDouble = 0x2555;
    public const int boxDrawingsDoubleVerticalAndLeft = 0x2563;
    public const int boxDrawingsDoubleVertical = 0x2551;
    public const int boxDrawingsDoubleDownAndLeft = 0x2557;
    public const int boxDrawingsDoubleUpAndLeft = 0x255d;
    public const int boxDrawingsUpDoubleAndLeftSingle = 0x255c;
    public const int boxDrawingsUpSingleAndLeftDouble = 0x255b;
    public const int boxDrawingsLightDownAndLeft = 0x2510;

    // 192 - 207.
    public const int boxDrawingsLightUpAndRight = 0x2514;
    public const int boxDrawingsLightUpAndHorizontal = 0x2534;
    public const int boxDrawingsLightDownAndHorizontal = 0x252c;
    public const int boxDrawingsLightVerticalAndRight = 0x251c;
    public const int boxDrawingsLightHorizontal = 0x2500;
    public const int boxDrawingsLightVerticalAndHorizontal = 0x253c;
    public const int boxDrawingsVerticalSingleAndRightDouble = 0x255e;
    public const int boxDrawingsVerticalDoubleAndRightSingle = 0x255f;
    public const int boxDrawingsDoubleUpAndRight = 0x255a;
    public const int boxDrawingsDoubleDownAndRight = 0x2554;
    public const int boxDrawingsDoubleUpAndHorizontal = 0x2569;
    public const int boxDrawingsDoubleDownAndHorizontal = 0x2566;
    public const int boxDrawingsDoubleVerticalAndRight = 0x2560;
    public const int boxDrawingsDoubleHorizontal = 0x2550;
    public const int boxDrawingsDoubleVerticalAndHorizontal = 0x256c;
    public const int boxDrawingsUpSingleAndHorizontalDouble = 0x2567;

    // 208 - 223.
    public const int boxDrawingsUpDoubleAndHorizontalSingle = 0x2568;
    public const int boxDrawingsDownSingleAndHorizontalDouble = 0x2564;
    public const int boxDrawingsDownDoubleAndHorizontalSingle = 0x2565;
    public const int boxDrawingsUpDoubleAndRightSingle = 0x2559;
    public const int boxDrawingsUpSingleAndRightDouble = 0x2558;
    public const int boxDrawingsDownSingleAndRightDouble = 0x2552;
    public const int boxDrawingsDownDoubleAndRightSingle = 0x2553;
    public const int boxDrawingsVerticalDoubleAndHorizontalSingle = 0x256b;
    public const int boxDrawingsVerticalSingleAndHorizontalDouble = 0x256a;
    public const int boxDrawingsLightUpAndLeft = 0x2518;
    public const int boxDrawingsLightDownAndRight = 0x250c;
    public const int fullBlock = 0x2588;
    public const int lowerHalfBlock = 0x2584;
    public const int leftHalfBlock = 0x258c;
    public const int rightHalfBlock = 0x2590;
    public const int upperHalfBlock = 0x2580;

    // 224 - 239.
    public const int greekSmallLetterAlpha = 0x03b1;
    public const int latinSmallLetterSharpS = 0x00df;
    public const int greekCapitalLetterGamma = 0x0393;
    public const int greekSmallLetterPi = 0x03c0;
    public const int greekCapitalLetterSigma = 0x03a3;
    public const int greekSmallLetterSigma = 0x03c3;
    public const int microSign = 0x00b5;
    public const int greekSmallLetterTau = 0x03c4;
    public const int greekCapitalLetterPhi = 0x03a6;
    public const int greekCapitalLetterTheta = 0x0398;
    public const int greekCapitalLetterOmega = 0x03a9;
    public const int greekSmallLetterDelta = 0x03b4;
    public const int infinity = 0x221e;
    public const int greekSmallLetterPhi = 0x03c6;
    public const int greekSmallLetterEpsilon = 0x03b5;
    public const int intersection = 0x2229;

    // 240 - 254.
    public const int identicalTo = 0x2261;
    public const int plusMinusSign = 0x00b1;
    public const int greaterThanOrEqualTo = 0x2265;
    public const int lessThanOrEqualTo = 0x2264;
    public const int topHalfIntegral = 0x2320;
    public const int bottomHalfIntegral = 0x2321;
    public const int divisionSign = 0x00f7;
    public const int almostEqualTo = 0x2248;
    public const int degreeSign = 0x00b0;
    public const int bulletOperator = 0x2219;
    public const int middleDot = 0x00b7;
    public const int squareRoot = 0x221a;
    public const int superscriptLatinSmallLetterN = 0x207f;
    public const int superscriptTwo = 0x00b2;
    public const int blackSquare = 0x25a0;
  }
}

