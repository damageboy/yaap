using System;

namespace Yaap
{
    /// <summary>
    /// A class representing the various colors that can be applied to Yaap
    /// </summary>
    public class YaapColorScheme
    {
        /// <summary>
        /// The <see cref="TerminalColorCode"/> of the <see cref="YaapElement.ProgressBar"/> element
        /// </summary>
        public TerminalColorCode ProgressBarColor { get; set; } = TerminalColorCode.None;

        /// <summary>
        /// The <see cref="TerminalColorCode"/> of the <see cref="YaapElement.ProgressPercent"/> element
        /// </summary>
        public TerminalColorCode ProgressPercentColor { get; set; } = TerminalColorCode.None;

        /// <summary>
        /// The <see cref="TerminalColorCode"/> of the <see cref="YaapElement.ProgressCount"/> element
        /// </summary>
        public TerminalColorCode ProgressCountColor { get; set; } = TerminalColorCode.None;

        /// <summary>
        /// The <see cref="TerminalColorCode"/> of the <see cref="YaapElement.Rate"/> element
        /// </summary>
        public TerminalColorCode RateColor { get; set; } = TerminalColorCode.None;

        /// <summary>
        /// The <see cref="TerminalColorCode"/> of the <see cref="YaapElement.TimeCounts"/> element
        /// </summary>
        public TerminalColorCode TimeColor { get; set; } = TerminalColorCode.None;

        /// <summary>
        /// The "no-color" color scheme for Yaap
        /// </summary>
        public static YaapColorScheme NoColor = new YaapColorScheme()
        {
            ProgressBarColor     = TerminalColorCode.None,
            ProgressPercentColor = TerminalColorCode.None,
        };

        /// <summary>
        /// The Bright Yaap color scheme for Yaap
        /// </summary>
        public static YaapColorScheme Bright = new YaapColorScheme()
        {
            ProgressBarColor     = TerminalColorCode.FromConsoleColor(ANSIColor.BrightGreen),
            ProgressPercentColor = TerminalColorCode.FromConsoleColor(ANSIColor.BrightYellow),
            ProgressCountColor   = TerminalColorCode.FromConsoleColor(ANSIColor.BrightMagenta),
            RateColor            = TerminalColorCode.FromConsoleColor(ANSIColor.BrightCyan),
            TimeColor            = TerminalColorCode.FromConsoleColor(ANSIColor.BrightGreen),
        };

        /// <summary>
        /// The Bright Yaap color scheme for Yaap
        /// </summary>
        public static YaapColorScheme Dark = new YaapColorScheme()
        {
            ProgressBarColor     = TerminalColorCode.FromConsoleColor(ANSIColor.Green),
            ProgressPercentColor = TerminalColorCode.FromConsoleColor(ANSIColor.Yellow),
            ProgressCountColor   = TerminalColorCode.FromConsoleColor(ANSIColor.Magenta),
            RateColor            = TerminalColorCode.FromConsoleColor(ANSIColor.Cyan),
            TimeColor            = TerminalColorCode.FromConsoleColor(ANSIColor.Green),
        };
    }
}
