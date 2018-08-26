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
    public class TerminalColor
    {
        static readonly bool _isTrueColorSupported = Environment.GetEnvironmentVariable("COLORTERM") == "truecolor";
        const string CSI = "\u001B[";
        const string FgReset = CSI + "0m";

        /// <summary>
        /// The vt100 escape code that should generate this <see cref="TerminalColor"/> on screen
        /// </summary>
        [PublicAPI]
        public string EscapeCode { get; }

        /// <summary>
        /// A <see cref="TerminalColor"/> representing no color change
        /// </summary>
        [PublicAPI]
        public static TerminalColor None { get; } = new TerminalColor(string.Empty);

        /// <summary>
        /// A <see cref="TerminalColor"/> representing a reset of the terminal coloring back to the default color
        /// </summary>
        [PublicAPI]
        public static TerminalColor Reset { get; } = new TerminalColor(FgReset);
        TerminalColor(string color) => EscapeCode = color;

        /// <summary>
        /// Create a <see cref="TerminalColor"/> instance from a <see cref="ConsoleColor"/>
        /// </summary>
        /// <param name="fg">The <see cref="TerminalColor"/> to use as a foreground color</param>
        /// <param name="bg">The <see cref="TerminalColor"/> to use as a background color</param>
        /// <returns>The newly created <see cref="TerminalColor"/></returns>
        [PublicAPI]
        public static TerminalColor FromConsoleColor(ANSIColor fg, ANSIColor bg = ANSIColor.Default)
            => new TerminalColor(fg, bg);

        TerminalColor(ANSIColor fg, ANSIColor bg = ANSIColor.Default)
            => EscapeCode = GetVT100Representation(fg, bg);

        static string GetVT100Representation(ANSIColor fg, ANSIColor bg = ANSIColor.Default)
        {
            return $"{CSI}{GetFGColor(fg)};{GetBGColor(bg)}m";
            string GetFGColor(ANSIColor color) => ((int) color).ToString();
            string GetBGColor(ANSIColor color) => ((int) color + 10).ToString();
        }

        /// <summary>
        /// Create a <see cref="TerminalColor"/> instance from a <see cref="Color"/>
        /// </summary>
        /// <param name="color">The <see cref="Color"/> to base this color upon</param>
        /// <param name="bg">a <see cref="Boolean"/> indicating wethear this color will be used as a background color</param>
        /// <returns>The newly created <see cref="TerminalColor"/></returns>
        public static TerminalColor FromColor(Color color, bool bg = false) => new TerminalColor(color, bg);

        [PublicAPI]
        TerminalColor(Color color, bool bg) => EscapeCode = GetVt100Representation(color, bg);

        static string GetVt100Representation(Color color, bool bg)
        {
            if (!_isTrueColorSupported)
                throw new Exception("terminal truecolor support doesn't seem to be supported by this terminal, if you are sure this is wrong, you can set the $TERMCOLOR environment variable to 'trueolor'");
            var (r, b, g, _) = color;
            return bg ?
                $"{CSI}48;2;{r};{g};{b}m" :
                $"{CSI}38;2;{r};{g};{b}m";
        }
    }
}
