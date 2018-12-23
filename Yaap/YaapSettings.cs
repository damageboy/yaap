using System;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace Yaap
{
    /// <summary>
    /// The current state of the <see cref="Yaap"/> instance
    /// </summary>
    [PublicAPI]
    public enum YaapState
    {
        /// <summary>
        /// The yaap instance is running (progressing)
        /// </summary>
        Running,
        /// <summary>
        /// The yaap instance is paused
        /// </summary>
        Paused,
        /// <summary>
        /// The yaap instance is stalled
        /// </summary>
        Stalled,
    }

    /// <summary>
    /// An enumeration representing the various yaap progress bar elements
    /// </summary>
    [PublicAPI]
    [Flags]
    public enum YaapElement
    {
        /// <summary>
        /// The description prefix visual Yaap element
        /// </summary>
        Description = 0x1,
        /// <summary>
        /// The numerical percent (e.g. 99%) visual Yaap element
        /// </summary>
        ProgressPercent = 0x2,
        /// <summary>
        /// The graphical progress bar visual Yaap element
        /// </summary>
        ProgressBar = 0x4,
        /// <summary>
        /// The progress count (e.g. 199/200) visual Yaap elements
        /// </summary>
        ProgressCount = 0x8,
        /// <summary>
        /// The elapsed/total time visual Yaap elements
        /// </summary>
        Time = 0x10,
        /// <summary>
        /// The rate visual Yaap element
        /// </summary>
        Rate = 0x20,
        /// <summary>
        /// A special or'd value representing all elements of Yaap
        /// </summary>
        All = Description|ProgressPercent|ProgressBar|ProgressCount|Time|Rate,
    }

    /// <summary>
    /// An enumeration representing the different positioning/alignment options for a Yaap progress bar(s)
    /// </summary>
    public enum YaapPositioning
    {
        /// <summary>
        /// Start displaying the Yaap at the current screen position, but snap it to the
        /// top of the terminal when it eventually scrolls over there
        /// </summary>
        FlowAndSnapToTop,
        /// <summary>
        /// Immediately clear the terminal once the Yaap is displayed, and display the Yaap(s) at the top of the terminal
        /// while the text scrolls below the Yaap(s)
        /// </summary>
        ClearAndAlignToTop,
        /// <summary>
        /// Immediately display the Yaap at the bottom of the terminal and allow the text to scroll in the region above
        /// the Yaap. Note that this might look funky when multiple Yaaps are used dynamically without pre-specifying their
        /// vertical position in advance...
        /// </summary>
        FixToBottom,
    }

    /// <summary>
    /// Yaap visual settings, used when constructing a <see cref="Yaap"/> object
    /// </summary>
    public class YaapSettings
    {
        /// <summary>
        /// Specifies a prefix for the progress bar text that should be used to uniquely identify the progress bar
        /// meaning/content to the user.
        /// </summary>
        [PublicAPI]
        public string Description { get; set; }

        /// <summary>
        /// A flags or'd value specifying which visual elements will be presented to the user
        /// </summary>
        [PublicAPI]
        public YaapElement Elements { get; set; } = YaapElement.All;

        /// <summary>
        /// A <see cref="YaapColorScheme"/> instance representing the desired color scheme for Yaap
        /// </summary>
        [PublicAPI]
        public YaapColorScheme ColorScheme { get; set; } = YaapColorScheme.NoColor;

        /// <summary>
        /// Use only ASCII charchters (notably the '#' charchter as the progress bar 'progress' glyph
        /// </summary>
        [PublicAPI]
        public bool UseASCII { get; set; }

        /// <summary>
        /// The select visual style for the progress bar, only taken into account when <see cref="UseASCII"/> is set to false (which is the default)
        /// and the underlying terminal supports unicode properly
        /// </summary>
        [PublicAPI]
        public YaapBarStyle Style { get; set; }

        /// <summary>
        /// Constrain the prgoress bar to a specific width, when not specified, the progress bar will take up
        /// the width of the terminal. If not set, the progress bar will resize dynamically as the windows changes size
        /// <remarks>Can be set to <see cref="Console.WindowWidth"/> in case the user wishes to constrain the progress bar to the current windows width, or any other width for that matter.</remarks>
        /// </summary>
        [PublicAPI]
        public int? Width { get; set; }

        /// <summary>
        /// Exponential moving average smoothing factor for speed estimates. Ranges from 0 (average speed) to 1 (current/instantaneous speed) [default: 0.3].
        /// </summary>
        [PublicAPI]
        public double SmoothingFactor { get; set; }

        /// <summary>
        /// used to name the unit unit of progress. [default: 'it']
        /// </summary>
        [PublicAPI]
        public string UnitName { get; set; } = "it";

        /// <summary>
        /// If set, will be used to scale the <see cref="Yaap.Progress"/> and <see cref="Yaap.Total"/> values, using
        /// the International System of Units standard. (kilo, mega, etc.)
        /// </summary>
        [PublicAPI]
        public bool MetricAbbreviations { get; set; }

        /// <summary>
        /// If set, and <see cref="MetricAbbreviations"/> is set to false, will be used to scale the
        /// <see cref="Yaap.Progress"/> and <see cref="Yaap.Total"/> values
        /// </summary>
        [PublicAPI]
        public double? UnitScale { get; set; }

        /// <summary>
        /// Leave the progress bar visually on screen once it is done/closed
        /// </summary>
        [PublicAPI]
        public bool Leave { get; set; } = true;

        /// <summary>
        /// Specify the line offset to print this bar (starting from 0). Automatic when unspecified.
        /// Useful to manage multiple bars at once (eg, from multiple threads).
        /// </summary>
        [PublicAPI]
        public int? VerticalPosition { get; set; }

        /// <summary>
        /// Specify the <see cref="YaapPositioning"/> for this progress bar. This property control how/where a Yaap
        /// remains "on" the Terminal in case of scrolling
        /// </summary>
        public YaapPositioning Positioning { get; set; }
    }
}
