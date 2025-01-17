namespace Toast.Engine;

/// <summary>
/// Joystick hat states.
/// </summary>
public enum GLFWHat
{
    Centered = 0,
    Up = 1,
    Right = 2,
    Down = 4,
    Left = 8,
    RightUp = Right | Up,
    RightDown = Right | Down,
    LeftUp = Left | Up,
    LeftDown = Left | Down
}

/// <summary>
/// Keyboard key tokens.<br/>
/// <br/>
/// These key codes are inspired by the _USB RID Usage Tables v1.12_ (p. 53-60),<br/>
/// but re-arranged to map 7-bit ASCII for printable keys (function keys are<br/>
/// put in the 256+ range).<br/>
/// <br/>
/// The naming of the key codes follow these rules:
/// <list type="bullet">
///     <item>The US keyboard is used</item>
///     <item>Names of printable alphanumeric characters are used (e.g. "A", "R",
///     "3", etc.)</item>
///     <item>For non-alphanumeric characters, Unicode:ish names are used (e.g.
///     "Comma", "LeftSquareBracket", etc.). Note that some names do not
///     correspond to the Unicode standard (usually for brevity)</item>
///     <item>Keys that lack a clear US mapping are named "WorldX"</item>
///     <item>For non-printable keys, custom names are used (e.g. "F4", 
///     "Backspace", etc.)</item>
/// </list>
/// </summary>
public enum GLFWKey
{
    Unknown = -1,

    /* Printable keys */
    Space = 32,
    Apostrophe = 39, /* ' */
    Comma = 44, /* , */
    Minus = 45, /* - */
    Period = 46, /* . */
    Slash = 47, /* / */
    Zero = 48,
    One = 49,
    Two = 50,
    Three = 51,
    Four = 52,
    Five = 53,
    Six = 54,
    Seven = 55,
    Eight = 56,
    Nine = 57,
    SemiColon = 59,
    Equal = 61,
    A = 65,
    B = 66,
    C = 67,
    D = 68,
    E = 69,
    F = 70,
    G = 71,
    H = 72,
    I = 73,
    J = 74,
    K = 75,
    L = 76,
    M = 77,
    N = 78,
    O = 79,
    P = 80,
    Q = 81,
    R = 82,
    S = 83,
    T = 84,
    U = 85,
    V = 86,
    W = 87,
    X = 88,
    Y = 89,
    Z = 90,
    LeftBracket = 91,
    Backslash = 92,
    RightBracket = 93,
    GraveAccent = 96,
    World1 = 161,
    World2 = 162,

    /* Function keys */
    Escape = 256,
    Enter = 257,
    Tab = 258,
    Backspace = 259,
    Insert = 260,
    Delete = 261,
    Right = 262, /* Arrow right */
    Left = 263, /* Arrow left */
    Down = 264, /* Arrow down */
    Up = 265, /* Arrow up */
    PageUp = 266,
    PageDn = 267,
    Home = 268,
    End = 269,
    CapsLock = 280,
    ScrollLock = 281,
    NumLock = 282,
    PrintScreen = 283,
    Pause = 284,
    F1 = 290,
    F2 = 291,
    F3 = 292,
    F4 = 293,
    F5 = 294,
    F6 = 295,
    F7 = 296,
    F8 = 297,
    F9 = 298,
    F10 = 299,
    F11 = 300,
    F12 = 301,
    F13 = 302,
    F14 = 303,
    F15 = 304,
    F16 = 305,
    F17 = 306,
    F18 = 307,
    F19 = 308,
    F20 = 309,
    F21 = 310,
    F22 = 311,
    F23 = 312,
    F24 = 313,
    F25 = 314,
    KPZero = 320,
    KPOne = 321,
    KPTwo = 322,
    KPThree = 323,
    KPFour = 324,
    KPFive = 325,
    KPSix = 326,
    KPSeven = 327,
    KPEight = 328,
    KPNine = 329,
    KPDecimal = 330,
    KPDivide = 331,
    KPMultiply = 332,
    KPSubtract = 333,
    KPAdd = 334,
    KPEnter = 335,
    KPEqual = 336,
    LeftShift = 340,
    LeftControl = 341,
    LeftAlt = 342,
    LeftSuper = 343,
    RightShift = 344,
    RightControl = 345,
    RightAlt = 346,
    RightSuper = 347,
    Menu = 348
}

/// <summary>
/// Modifier key flags.
/// </summary>
public enum GLFWMod
{
    // If this bit is set one or more Shift keys were held down
    Shift = 0x0001,

    // If this bit is set one or more Control keys were held down
    Control = 0x0002,

    // If this bit is set one or more Alt keys were held down
    Alt = 0x0004,

    // If this bit is set one or more Super keys were held down
    Super = 0x0008,

    // If this bit is set the Caps Lock key is enabled
    CapsLock = 0x0010,

    // If this bit is set the Num Lock key is enabled
    NumLock = 0x0020,
}

/// <summary>
/// Mouse buttons.
/// </summary>
public enum GLFWMouse
{
    Button1 = 0,
    Button2 = 1,
    Button3 = 2,
    Button4 = 3,
    Button5 = 4,
    Button6 = 5,
    Button7 = 6,
    Button8 = 7,
    ButtonLast = Button8,
    ButtonLeft = Button1,
    ButtonRight = Button2,
    ButtonMiddle = Button3,
}

/// <summary>
/// Joysticks.
/// </summary>
public enum GLFWJoystick
{
    JS1 = 0,
    JS2 = 1,
    JS3 = 2,
    JS4 = 3,
    JS5 = 4,
    JS6 = 5,
    JS7 = 6,
    JS8 = 7,
    JS9 = 8,
    JS10 = 9,
    JS11 = 10,
    JS12 = 11,
    JS13 = 12,
    JS14 = 13,
    JS15 = 14,
    JS16 = 15,
    Last = JS16
}

/// <summary>
/// Gamepad buttons.
/// </summary>
public enum GLFWGamepad
{

}