using System;
using System.Drawing;
using JetBrains.Annotations;

namespace Yaap
{

    /// <summary>
    /// An enum representing the one true set of 3/4 bit ANSI colors
    /// Feel free to read more about ansi color codes:
    /// https://en.wikipedia.org/wiki/ANSI_escape_code#Colors
    /// </summary>
    public enum ANSIColor
    {
        /// <summary>
        /// Roughly 0,0,0
        /// </summary>
        Black = 30,
        /// <summary>
        /// Roughly 170,0,0
        /// </summary>
        Red = 31,
        /// <summary>
        /// Roughly 0,170,0
        /// </summary>
        Green = 32,
        /// <summary>
        /// Roughly 170,85,0
        /// </summary>
        Yellow = 33,
        /// <summary>
        /// Roughly 0,0,170
        /// </summary>
        Blue = 34,
        /// <summary>
        /// Roughly 170,0,170
        /// </summary>
        Magenta = 35,
        /// <summary>
        /// Roughly 0,170,170
        /// </summary>
        Cyan = 36,
        /// <summary>
        /// Roughly 170,170,170
        /// </summary>
        White = 37,
        /// <summary>
        /// Roughly 85,85,85
        /// </summary>
        BrightBlack = 90,
        /// <summary>
        /// Roughly 255,85,85
        /// </summary>
        BrightRed = 91,
        /// <summary>
        /// Roughly 85,255,85
        /// </summary>
        BrightGreen = 92,
        /// <summary>
        /// Roughly 255,255,85
        /// </summary>
        BrightYellow = 93,
        /// <summary>
        /// Roughly 85,85,255
        /// </summary>
        BrightBlue = 94,
        /// <summary>
        /// Roughly 255,85,255
        /// </summary>
        BrightMagenta = 95,
        /// <summary>
        /// Roughly 85,255,255
        /// </summary>
        BrightCyan = 96,
        /// <summary>
        /// Roughly 255,255,255
        /// </summary>
        BrightWhite = 97,
        /// <summary>
        /// No color (Reset/Normal)
        /// </summary>
        Default = 39,
    }

    /// <summary>
    /// A class that represents a color that is either originated from <see cref="ConsoleColor"/> instance or from a
    /// <see cref="Color"/> instance.
    /// </summary>
    public class TerminalColorCode
    {
        static readonly bool _isTrueColorSupported = Environment.GetEnvironmentVariable("COLORTERM") == "truecolor";
        const string Esc = "\u001B[";
        const string FgReset = Esc + "0m";

        /// <summary>
        /// The vt100 escape code that should generate this <see cref="TerminalColorCode"/> on screen
        /// </summary>
        [PublicAPI]
        public string EscapeCode { get; }

        /// <summary>
        /// A <see cref="TerminalColorCode"/> representing no color change
        /// </summary>
        [PublicAPI]
        public static TerminalColorCode None { get; } = new TerminalColorCode(string.Empty);

        /// <summary>
        /// A <see cref="TerminalColorCode"/> representing a reset of the terminal coloring back to the default color
        /// </summary>
        [PublicAPI]
        public static TerminalColorCode Reset { get; } = new TerminalColorCode(FgReset);
        TerminalColorCode(string color) => EscapeCode = color;

        /// <summary>
        /// Create a <see cref="TerminalColorCode"/> instance from a <see cref="ConsoleColor"/>
        /// </summary>
        /// <param name="fg">The <see cref="TerminalColorCode"/> to use as a foreground color</param>
        /// <param name="bg">The <see cref="TerminalColorCode"/> to use as a background color</param>
        /// <returns>The newly created <see cref="TerminalColorCode"/></returns>
        [PublicAPI]
        public static TerminalColorCode FromConsoleColor(ANSIColor fg, ANSIColor bg = ANSIColor.Default)
            => new TerminalColorCode(fg, bg);

        TerminalColorCode(ANSIColor fg, ANSIColor bg = ANSIColor.Default)
            => EscapeCode = GetVT100Representation(fg, bg);

        static string GetVT100Representation(ANSIColor fg, ANSIColor bg = ANSIColor.Default)
        {
            return $"{Esc}{GetFGColor(fg)};{GetBGColor(bg)}m";
            string GetFGColor(ANSIColor color) => ((int) color).ToString();
            string GetBGColor(ANSIColor color) => ((int) color + 10).ToString();
        }

        /// <summary>
        /// Create a <see cref="TerminalColorCode"/> instance from a <see cref="Color"/>
        /// </summary>
        /// <param name="color">The <see cref="Color"/> to base this color upon</param>
        /// <param name="bg">a <see cref="Boolean"/> indicating wethear this color will be used as a background color</param>
        /// <returns>The newly created <see cref="TerminalColorCode"/></returns>
        public static TerminalColorCode FromColor(Color color, bool bg = false) => new TerminalColorCode(color, bg);

        [PublicAPI]
        TerminalColorCode(Color color, bool bg) => EscapeCode = GetVt100Representation(color, bg);

        static string GetVt100Representation(Color color, bool bg)
        {
            if (!_isTrueColorSupported)
                throw new Exception("terminal truecolor support doesn't seem to be supported by this terminal, if you are sure this is wrong, you can set the $TERMCOLOR environment variable to 'trueolor'");
            var (r, b, g, _) = color;
            return bg ?
                $"{Esc}48;2;{r};{g};{b}m" :
                $"{Esc}38;2;{r};{g};{b}m";
        }
    }
}
