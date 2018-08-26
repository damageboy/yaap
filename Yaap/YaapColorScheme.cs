using System;

namespace Yaap
{
    /// <summary>
    /// A class representing the various colors that can be applied to Yaap
    /// </summary>
    public class YaapColorScheme
    {
        /// <summary>
        /// The <see cref="TerminalColor"/> of the <see cref="YaapElement.ProgressBar"/> element when in <see cref="YaapState.Running"/>
        /// </summary>
        public TerminalColor ProgressBarColor { get; set; } = TerminalColor.None;

        /// <summary>
        /// The <see cref="TerminalColor"/> of the <see cref="YaapElement.ProgressBar"/> element when in <see cref="YaapState.Paused"/>
        /// </summary>
        public TerminalColor ProgressBarPausedColor { get; set; } = TerminalColor.None;


        /// <summary>
        /// The <see cref="TerminalColor"/> of the <see cref="YaapElement.ProgressBar"/> element when in <see cref="YaapState.Stalled"/>
        /// </summary>
        public TerminalColor ProgressBarStalledColor { get; set; } = TerminalColor.None;

        /// <summary>
        /// The <see cref="TerminalColor"/> of the <see cref="YaapElement.ProgressPercent"/> element
        /// </summary>
        public TerminalColor ProgressPercentColor { get; set; } = TerminalColor.None;

        /// <summary>
        /// The <see cref="TerminalColor"/> of the <see cref="YaapElement.ProgressCount"/> element
        /// </summary>
        public TerminalColor ProgressCountColor { get; set; } = TerminalColor.None;

        /// <summary>
        /// The <see cref="TerminalColor"/> of the <see cref="YaapElement.Rate"/> element
        /// </summary>
        public TerminalColor RateColor { get; set; } = TerminalColor.None;

        /// <summary>
        /// The <see cref="TerminalColor"/> of the <see cref="YaapElement.Time"/> element
        /// </summary>
        public TerminalColor TimeColor { get; set; } = TerminalColor.None;

        /// <summary>
        /// The "no-color" color scheme for Yaap
        /// </summary>
        public static YaapColorScheme NoColor = new YaapColorScheme()
        {
            ProgressBarColor     = TerminalColor.None,
            ProgressPercentColor = TerminalColor.None,
        };

        /// <summary>
        /// The Bright Yaap color scheme for Yaap
        /// </summary>
        public static YaapColorScheme Bright = new YaapColorScheme()
        {
            ProgressBarColor         = TerminalColor.FromConsoleColor(ANSIColor.BrightGreen),
            ProgressBarPausedColor   = TerminalColor.FromConsoleColor(ANSIColor.BrightYellow),
            ProgressBarStalledColor  = TerminalColor.FromConsoleColor(ANSIColor.Red),
            ProgressPercentColor     = TerminalColor.FromConsoleColor(ANSIColor.BrightYellow),
            ProgressCountColor       = TerminalColor.FromConsoleColor(ANSIColor.BrightMagenta),
            RateColor                = TerminalColor.FromConsoleColor(ANSIColor.BrightCyan),
            TimeColor                = TerminalColor.FromConsoleColor(ANSIColor.BrightGreen),
        };

        /// <summary>
        /// The Bright Yaap color scheme for Yaap
        /// </summary>
        public static YaapColorScheme Dark = new YaapColorScheme()
        {
            ProgressBarColor         = TerminalColor.FromConsoleColor(ANSIColor.Green),
            ProgressBarPausedColor   = TerminalColor.FromConsoleColor(ANSIColor.Yellow),
            ProgressBarStalledColor  = TerminalColor.FromConsoleColor(ANSIColor.Red),
            ProgressPercentColor     = TerminalColor.FromConsoleColor(ANSIColor.Yellow),
            ProgressCountColor       = TerminalColor.FromConsoleColor(ANSIColor.Magenta),
            RateColor                = TerminalColor.FromConsoleColor(ANSIColor.Cyan),
            TimeColor                = TerminalColor.FromConsoleColor(ANSIColor.Green),
        };
    }
}
