using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Formatting;
using System.Threading;
using JetBrains.Annotations;
using System.Drawing;

namespace Yaap
{
    static class YaapRegistry
    {
        static Thread _monitorThread;
        static readonly char[] _chars = new char[Console.WindowWidth * 2];
        static readonly object _consoleLock = new object();
        static readonly object _threadLock = new object();
        static readonly IDictionary<int, Yaap> _instances = new ConcurrentDictionary<int, Yaap>();
        static int _totalLines;
        static int _isRunning;

        static bool IsRunning
        {
            get => _isRunning == 1;
            set => Interlocked.Exchange(ref _isRunning, value ? 1 : 0);
        }


        internal static void AddInstance(Yaap yaap)
        {
            lock (_threadLock)
            {
                lock (_consoleLock)
                {
                    _instances.Add(GetOrSetVerticalPosition(yaap), yaap);

                    if (IsRunning)
                    {
                        // Windows console (in the case we are on windows) has already been red-pilled
                        // So repaint() and byebye
                        Repaint(yaap);
                        return;
                    }

                    if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                        RedPill();
                    // If we are just starting up the monitoring thread, we've
                    // just potentially red-pilled the windows console, so we can repaint now
                    Repaint(yaap);

                    IsRunning = true;
                    Console.CancelKeyPress += OnCancelKeyPress;
                }

                _monitorThread = new Thread(UpdateYaaps) { Name = nameof(UpdateYaaps) };
                _monitorThread.Start();
            }

            void RedPill() => Win32Console.EnableVT100Stuffs();
        }

        internal static void RemoveInstance(Yaap yaap)
        {
            lock (_threadLock)
            {
                lock (_consoleLock)
                {
                    // Repaint just before remving for cosmetic purposes:
                    // In case we didn't have a recent update to the progress bar, it might be @ 100%
                    // "in reality" but not visually.... This call will close that gap
                    Repaint(yaap);
                    _instances.Remove(yaap.Position);

                    if (_instances.Count > 0)
                        return;

                    IsRunning = false;
                    Console.CancelKeyPress -= OnCancelKeyPress;

                    if (yaap.Position + 1 == _totalLines)
                        _totalLines = _instances.Count == 0 ? 0 : (_instances.Keys.Max() + 1);

                }
                _monitorThread.Join();
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                    BluePill();
            }
            void BluePill() => Win32Console.RestoreTerminalToPristineState();
        }

        static int GetOrSetVerticalPosition(Yaap yaap)
        {
            if (yaap.Settings.VerticalPosition.HasValue)
                yaap.Position = yaap.Settings.VerticalPosition.Value;
            else
            {
                var lastPos = -1;
                foreach (var p in _instances.Keys)
                {
                    if (p > lastPos + 1)
                        return yaap.Position = lastPos + 1;
                    lastPos = p;
                }

                yaap.Position = ++lastPos;
            }

            if (_totalLines > yaap.Position)
                return yaap.Position;

            // This progress bar is taking up one more line
            // than we previously accounted for, so bump the total line count + \n
            for (var l = _totalLines; l < yaap.Position + 1; l++)
                Console.WriteLine();
            _totalLines = yaap.Position + 1;
            return yaap.Position;
        }

        static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
        }


        static void UpdateYaaps()
        {
            const int INTERVAL_MS = 100;

            while (IsRunning)
            {
                foreach (var y in _instances.Values)
                {
                    if (y.NeedsRepaint)
                    {
                        Console.CursorVisible = false;
                        Repaint(y);
                    }
                }
                Console.Write('\r');
                Console.CursorVisible = true;
                Thread.Sleep(INTERVAL_MS);
            }
        }

        //static char[] _cursorOn  = {(char) 0x9B, (char) 0x3F, (char) 0x32, (char) 0x35, (char) 0x6C};
        //static char[] _cursorOff = {(char) 0x9B, (char) 0x3F, (char) 0x32, (char) 0x35, (char) 0x68};

        static void Repaint(Yaap yaap)
        {
            var buffer = yaap.Repaint();
            lock (_consoleLock)
            {
                buffer.CopyTo(0, _chars, 0, buffer.Count);
                var oldLine = MoveTo(yaap);
                Console.Write(_chars, 0, buffer.Count);
                Console.CursorTop = oldLine;
            }
        }

        static int MoveTo(Yaap yaap)
        {
            var currentLine = Console.CursorTop;
            Console.CursorTop = currentLine - (_totalLines - yaap.Position);
            return currentLine;
        }

        internal static void Write(string s) { lock (_consoleLock) { Console.Write(s); } }

        internal static void WriteLine(string s) { lock (_consoleLock) { Console.WriteLine(s); if (_totalLines > 0) _totalLines++; } }

        internal static void WriteLine() { lock (_consoleLock) { Console.WriteLine(); if (_totalLines > 0) _totalLines++; } }

    }

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
        /// The yaap instance is paused/frozen
        /// </summary>
        Paused,
        /// <summary>
        /// The yaap instance is paused
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
    /// An enumeration representing the various visual styles of a Yaap progress bar component
    /// </summary>
    [PublicAPI]
    public enum YaapBarStyle
    {
        /// <summary>
        /// A Yaap progress bar style that uses the unicode charchters:
        /// '▏', '▎', '▍', '▌', '▋', '▊', '▉', '█'
        /// </summary>
        BarHorizontal,
        /// <summary>
        /// A Yaap progress bar style that uses the unicode charchters:
        /// '▁', '▂', '▃', '▄', '▅', '▆', '▇', '█'
        /// </summary>
        BarVertical,
        /// <summary>
        /// A Yaap progress bar style that uses the unicode charchters:
        /// '⣀', '⣄', '⣤', '⣦', '⣶', '⣷', '⣿'
        /// </summary>
        DotsHorizontal,
        /// <summary>
        /// A Yaap progress bar style that uses the unicode charchters:
        /// '⣀', '⣄', '⣆', '⣇', '⣧', '⣷', '⣿'
        /// </summary>
        DotsVertical,
        /// <summary>
        /// A Yaap progress bar style that uses the unicode charchters:
        /// '○', '◔', '◐', '◕', '⬤'
        /// </summary>
        Clock,
        /// <summary>
        /// A Yaap progress bar style that uses the unicode charchters:
        /// '□', '◱', '◧', '▣', '■'
        /// </summary>
        Squares1,
        /// <summary>
        /// A Yaap progress bar style that uses the unicode charchters:
        /// '□', '◱', '▨', '▩', '■'
        /// </summary>
        Squares2,
        /// <summary>
        /// A Yaap progress bar style that uses the unicode charchters:
        /// '□', '◱', '▥', '▦', '■'
        /// </summary>
        Squares3,
        /// <summary>
        /// A Yaap progress bar style that uses the unicode charchters:
        /// </summary>
        ShortSquares,
        /// <summary>
        /// A Yaap progress bar style that uses the unicode charchters:
        /// </summary>
        LongMesh,
        /// <summary>
        /// A Yaap progress bar style that uses the unicode charchters:
        /// </summary>
        ShortMesh,
        /// <summary>
        /// A Yaap progress bar style that uses the unicode charchters:
        /// </summary>
        Parallelogram,
        /// <summary>
        /// A Yaap progress bar style that uses the unicode charchters:
        /// </summary>
        Rectangles1,
        /// <summary>
        /// A Yaap progress bar style that uses the unicode charchters:
        /// </summary>
        Rectangles2,
        /// <summary>
        /// A Yaap progress bar style that uses the unicode charchters:
        /// </summary>
        Circles1,
        /// <summary>
        /// A Yaap progress bar style that uses the unicode charchters:
        /// </summary>
        Circles2,
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
        /// Attempt to detect progress stalls, e.g. if the transient progress/rate falls preciptously
        /// </summary>
        [PublicAPI]
        public bool DetectStalls { get; set; }

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
        public bool UseMetricAbbreviations { get; set; }

        /// <summary>
        /// If set, and <see cref="UseMetricAbbreviations"/> is set to false, will be used to scale the
        /// <see cref="Yaap.Progress"/> and <see cref="Yaap.Total"/> values
        /// </summary>
        [PublicAPI]
        public double? UnitScale { get; set; }

        /// <summary>
        /// Leave the progress bar visually on screen once it is done/closed
        /// </summary>
        [PublicAPI]
        public bool Leave { get; set; }

        /// <summary>
        /// Specify the line offset to print this bar (starting from 0). Automatic when unspecified.
        /// Useful to manage multiple bars at once (eg, from multiple threads).
        /// </summary>
        [PublicAPI]
        public int? VerticalPosition { get; set; }

    }

    /// <inheritdoc />
    /// <summary>
    /// Represents a text mode progress bar control, that can visually provide user feedback as to the progress
    /// a long-standing operation, including progress visualization, elapsed time, total time, rate and more
    /// </summary>
    public class Yaap : IDisposable
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        static readonly char[][] _barStyles = {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new [] { '▏', '▎', '▍', '▌', '▋', '▊', '▉', '█'},
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new [] { '▁', '▂', '▃', '▄', '▅', '▆', '▇', '█'},
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new [] { '⣀', '⣄', '⣤', '⣦', '⣶', '⣷', '⣿' },
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new [] { '⣀', '⣄', '⣆', '⣇', '⣧', '⣷', '⣿' },
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new [] { '○', '◔', '◐', '◕', '⬤' },
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new [] { '□', '◱', '◧', '▣', '■' },
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new [] { '□', '◱', '▨', '▩', '■' },
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new [] { '□', '◱', '▥', '▦', '■' },
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new [] { '⬜', '⬛' },
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new [] { '░', '▒', '▓', '█' },
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new [] { '░', '█' },
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new [] { '▱', '▰' },
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new [] { '▭', '◼' },
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new [] { '▯', '▮' },
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new [] { '◯', '⬤' },
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new [] { '⚪','⚫' },
        };

        static bool _unicodeNotWorky;
        static readonly char[] _asciiBarStyle = { '#' };
        readonly char[] _selectedBarStyle;
        long _nextRepaintProgress;
        readonly string _progressCountFmt;
        readonly int _maxGlyphWidth;
        readonly long _repaintProgressIncrement;
        internal readonly Stopwatch _sw;
        TimeSpan _totalTime;
        static readonly long _swTicksIn1Hour = Stopwatch.Frequency * 3600;
        long _lastRepaintTicks;
        double _rate;
        long _lastProgress;
        readonly string _unitName;
        readonly string _description;
        readonly bool _useMetricAbbreviations;
        readonly double _smoothingFactor;
        readonly StringBuffer _buffer;

        static Yaap()
        {
            void DoUnspeakableThingsOnWindows()
            {
                // ReSharper disable once HeapView.ObjectAllocation.Evident
                var acceptableUnicodeFonts = new[] {
                    "Hack",
                    "InputMono",
                    "Hasklig",
                    "DejaVu Sans Mono",
                    "Iosevka",
                };

                var vt100IsGo = Win32Console.EnableVT100Stuffs();
                _unicodeNotWorky = vt100IsGo &&
                    acceptableUnicodeFonts.FirstOrDefault(s => Win32Console.ConsoleFontName.StartsWith(s, StringComparison.InvariantCulture)) == null;
                Win32Console.RestoreTerminalToPristineState();
            }

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                DoUnspeakableThingsOnWindows();
            else
                _unicodeNotWorky = !(Console.OutputEncoding is UTF8Encoding);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Yaap.Yaap"/> class.
        /// </summary>
        /// <param name="total">The (optional)total number of elements of the wrapped <see cref="IEnumerable{T}"/> that will be enumerated</param>
        /// <param name="initialProgress">The (optional) initial progress value</param>
        /// <param name="settings">The (optional) visual settings overrides</param>
        [PublicAPI]
        public Yaap(long total, long initialProgress = 0, YaapSettings settings = null)
        {
            int epilogueLen;
            if (total < 0)
                throw new ArgumentOutOfRangeException(nameof(total), "cannot be negative");

            if (initialProgress < 0)
                throw new ArgumentOutOfRangeException(nameof(initialProgress), "cannot be negative");

            Settings = settings ?? new YaapSettings();
            Total = total;
            Progress = initialProgress;

            _unitName = Settings.UnitName;
            _description = Settings.Description;
            _useMetricAbbreviations = Settings.UseMetricAbbreviations;
            _smoothingFactor = Settings.SmoothingFactor;

            _buffer = new StringBuffer(Console.WindowWidth);

            if (Settings.UseASCII || _unicodeNotWorky)
                _selectedBarStyle = _asciiBarStyle;
            else
                _selectedBarStyle = _barStyles[(int)Settings.Style];

            if (Settings.UseMetricAbbreviations) {
                var (abbrevTotal, suffix) = GetMetricAbbreviation(total);
                _progressCountFmt = $"{{0,3}}{{1}}/{abbrevTotal}{suffix}";
                epilogueLen = "|123K/999K".Length;
            } else {
                var totalChars = CountDigits(Total);
                _progressCountFmt = $"{{0,{totalChars}}}/{total}";
                epilogueLen = 1 + totalChars * 2 + 1;
            }

            //_timeFmt = $"[{{0}}<{{1}}, {{2}}{unitName}/s]";
            const string EPILOGUE_SAMPLE = " [11:22s<33:44s, 123.45/s]";

            epilogueLen += EPILOGUE_SAMPLE.Length + Settings.UnitName.Length;

            var capturedWidth = Console.WindowWidth - 2;
            if (Settings.Width.HasValue && Settings.Width.Value < capturedWidth)
                capturedWidth = Settings.Width.Value;

            var prologueCount = (string.IsNullOrWhiteSpace(Settings.Description) ? 0 : Settings.Description.Length) + 7;

            _maxGlyphWidth = capturedWidth - prologueCount - epilogueLen;

            _repaintProgressIncrement = Total / (_maxGlyphWidth * _selectedBarStyle.Length);
            if (_repaintProgressIncrement == 0)
                _repaintProgressIncrement = 1;

            _nextRepaintProgress =
                Progress / _repaintProgressIncrement * _repaintProgressIncrement +
                _repaintProgressIncrement;

            _totalTime = TimeSpan.MaxValue;
            _sw = Stopwatch.StartNew();
            YaapRegistry.AddInstance(this);
        }

        /// <summary>
        /// The current progress value of the progress bar
        /// <remarks>Always between 0 .. <see cref="Total"/></remarks>
        /// </summary>
        [PublicAPI]
        public long Progress { get; set; }

        /// <summary>
        /// The maximal value of the progress bar which represents 100%
        /// <remarks>When the value is not supplied, only basic statistics will be displayed</remarks>
        /// </summary>
        [PublicAPI]
        public long Total { get; }

        /// <summary>
        /// Whether to disable the entire progressbar display
        /// </summary>
        [PublicAPI]
        public bool Disable { get; set; }

        /// <summary>
        /// The vertical position of this instance in relation to other concurrently "live" <see cref="Yaap"/> objects
        /// </summary>
        [PublicAPI]
        public int Position { get; internal set; }

        /// <summary>
        /// The visual settings used for this instance
        /// </summary>
        /// <value>The settings.</value>
        [PublicAPI]
        public YaapSettings Settings { get; }

        /// <summary>
        /// The elapsed amount of time this operation has taken so far
        /// </summary>
        /// <value>The elapsed time.</value>
        [PublicAPI]
        public TimeSpan ElapsedTime { get; private set; }

        /// <summary>
        /// The predicted total amount of time this operation will take
        /// </summary>
        /// <value>The total time.</value>
        [PublicAPI]
        public TimeSpan TotalTime { get; private set; }

        /// <summary>
        /// The current <see cref="YaapState"/> of the instance
        /// </summary>
        /// <value>The state.</value>
        public YaapState State { get; set; }

        static readonly string[] _metricUnits = { "", "k", "M", "G", "T", "P", "E", "Z" };
        TerminalColorCode _lastColor;

        static (long num, string abbrev) GetMetricAbbreviation(long num)
        {
            for (var i = 0; i < _metricUnits.Length; i++) {
                if (num < 1000)
                    return (num, _metricUnits[i]);
                num /= 1000;
            }
            throw new ArgumentOutOfRangeException(nameof(num), "is too large");
        }

        static (double num, string abbrev) GetMetricAbbreviation(double num)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < _metricUnits.Length; i++) {
                if (num < 1000)
                    return (num, _metricUnits[i]);
                num /= 1000;
            }

            throw new ArgumentOutOfRangeException(nameof(num), "is too large");
        }

        static int CountDigits(long number)
        {
            var digits = 0;
            while (number != 0) {
                number /= 10;
                digits++;
            }
            return digits;
        }

        internal bool NeedsRepaint
        {
            get
            {
                var updateSpan = Stopwatch.Frequency;
                if (_sw.ElapsedTicks >= _swTicksIn1Hour || _totalTime.Ticks >= TimeSpan.TicksPerHour)
                    updateSpan = Stopwatch.Frequency * 60;

                if (_lastRepaintTicks + updateSpan < _sw.ElapsedTicks)
                    return true;

                if (Progress < _nextRepaintProgress)
                    return false;
                _nextRepaintProgress = _nextRepaintProgress + _repaintProgressIncrement;
                return true;
            }
        }

        /// <summary>
        /// Releases all resources used by the progress bar
        /// </summary>
        public void Dispose() => YaapRegistry.RemoveInstance(this);


        const string Esc = "\u001B[";
        const string EraseEndLine = Esc + "K";
        //const string FgBold = ESC + "1m";

        bool ShouldShoveDescription     => (Settings.Elements & YaapElement.Description) != 0;
        bool ShouldShoveProgressPercent => (Settings.Elements & YaapElement.ProgressPercent) != 0;
        bool ShouldShoveProgressBar     => (Settings.Elements & YaapElement.ProgressBar) != 0;
        bool ShouldShoveProgressCount   => (Settings.Elements & YaapElement.ProgressCount) != 0;
        bool ShouldShoveTime            => (Settings.Elements & YaapElement.Time) != 0;
        bool ShouldShoveRate            => (Settings.Elements & YaapElement.Rate) != 0;

        internal StringBuffer Repaint()
        {
            // Capture progress while repainting
            var progress = Progress;
            var elapsedTicks = _sw.ElapsedTicks;

            var elements = Settings.Elements;

            RecalculateRateAndTotalTime();

            var cs = Settings.ColorScheme;

            _buffer.Clear();
            _buffer.Append('\r');
            _buffer.Append(EraseEndLine);

            if (ShouldShoveDescription)
                ShoveDescription();
            if (ShouldShoveProgressPercent)
                ShoveProgressPercentage();
            if (ShouldShoveProgressBar)
                ShoveProgressBar();
            if (ShouldShoveProgressCount)
                ShoveProgressTotals();
            _buffer.Append(' ');
            //[{{0}}<{{1}}, {{2}}{unitName}/s]

            // At least one of Time|Rate is turned on?
            if ((Settings.Elements & (YaapElement.Rate | YaapElement.Time)) != 0)
                _buffer.Append('[');
            ShoveTime();
            if (ShouldShoveTime)
            _buffer.Append(", ");
            if (ShouldShoveRate)
                ShoveRate();
            if ((Settings.Elements & (YaapElement.Rate | YaapElement.Time)) != 0)
                _buffer.Append(']');

            _lastProgress = progress;
            _lastRepaintTicks = elapsedTicks;
            return _buffer;

            void ShoveDescription()
            {
                if (string.IsNullOrWhiteSpace(_description)) return;
                _buffer.Append(_description);
                _buffer.Append(": ");
            }

            void ShoveProgressPercentage()
            {
                ChangeColor(cs.ProgressPercentColor);
                _buffer.AppendFormat("{0,3}%", progress * 100 / Total);
                ResetColor();
            }

            void ShoveProgressBar()
            {
                _buffer.Append('|');
                _buffer.Append(Settings.ColorScheme.ProgressBarColor.EscapeCode);
                ChangeColor(cs.ProgressBarColor);
                var numChars = _selectedBarStyle.Length > 1
                    ? RenderComplexProgressGlyphs(_buffer, progress)
                    : RenderSimpleProgressGlyphs(_buffer, progress);
                ResetColor();
                _buffer.Append(' ', _maxGlyphWidth - numChars);
                _buffer.Append('|');
            }

            void ShoveProgressTotals()
            {
                ChangeColor(cs.ProgressCountColor);
                if (_useMetricAbbreviations)
                {
                    var (abbrevNum, suffix) = GetMetricAbbreviation(Progress);
                    _buffer.AppendFormat(_progressCountFmt, abbrevNum, suffix);
                }
                else
                    _buffer.AppendFormat(_progressCountFmt, Progress);

                ResetColor();
            }

            void RecalculateRateAndTotalTime()
            {
                // If we're "told" not to smooth out the rate/total time prediciton,
                // we just use the whole thing for the progress calc, otherwise we continuously sample
                // the last rate update since the previous rate and smooth it out using EMA/SmoothingFactor
                const double TOLERANCE = 1e-6;
                if (Math.Abs(_smoothingFactor) < TOLERANCE)
                    _rate = ((double) progress * Stopwatch.Frequency) / elapsedTicks;
                else
                {
                    var dProgress = progress - _lastProgress;
                    var dTicks = elapsedTicks - _lastRepaintTicks;

                    var lastRate = ((double) dProgress * Stopwatch.Frequency) / dTicks;
                    _rate = _lastRepaintTicks == 0 ? lastRate : _smoothingFactor * lastRate + (1 - _smoothingFactor) * _rate;
                }

                _totalTime = _rate <= 0 ? TimeSpan.MaxValue : new TimeSpan((long) (Total * TimeSpan.TicksPerSecond / _rate));
            }

            void ShoveTime()
            {
                ChangeColor(cs.TimeColor);
                WriteTimes(_buffer, new TimeSpan((elapsedTicks * TimeSpan.TicksPerSecond) / Stopwatch.Frequency), _totalTime);
                ResetColor();
            }

            void ShoveRate()
            {
                ChangeColor(cs.RateColor);
                if (_useMetricAbbreviations)
                {
                    var (abbrevNum, suffix) = GetMetricAbbreviation(_rate);
                    if (abbrevNum < 100)
                        _buffer.AppendFormat("{0:F2}{1}{2}/s]", abbrevNum, suffix, _unitName);
                    else
                        _buffer.AppendFormat("{0}{1}{2}/s]", (int) abbrevNum, suffix, _unitName);
                }
                else
                {
                    if (_rate < 100)
                        _buffer.AppendFormat("{0:F2}{1}/s", _rate, _unitName);
                    else
                        _buffer.AppendFormat("{0}{1}/s", (int) _rate, _unitName);
                }
                ResetColor();
            }
        }

        void ChangeColor(TerminalColorCode color)
        {
            _lastColor = color;
            _buffer.Append(color.EscapeCode);
        }

        void ResetColor()
        {
            if (_lastColor == TerminalColorCode.None)
                return;
            _buffer.Append(TerminalColorCode.Reset.EscapeCode);
        }

        static void WriteTimes(StringBuffer buffer, TimeSpan elapsed, TimeSpan remaining)
        {
            Debug.Assert(elapsed.Ticks >= 0);
            Debug.Assert(remaining.Ticks >= 0);
            var (edays, ehours, eminutes, eseconds, _) = elapsed;
            var (rdays, rhours, rminutes, rseconds, _) = remaining;

            if (edays + rdays > 0)
            {
                // Print days formatting
            }
            else if (ehours + rhours > 0)
            {
                if (elapsed == TimeSpan.MaxValue)
                    buffer.Append("--:--?<");
                else
                    buffer.AppendFormat("{0:D2}:{1:D2}m<", ehours, eminutes);
                if (remaining == TimeSpan.MaxValue)
                    buffer.Append("--:--?");
                else
                    buffer.AppendFormat("{0:D2}:{1:D2}m", rhours, rminutes);
            }
            else
            {
                if (elapsed == TimeSpan.MaxValue)
                    buffer.Append("--:--?<");
                else
                    buffer.AppendFormat("{0:D2}:{1:D2}s<", eminutes, eseconds);
                if (remaining == TimeSpan.MaxValue)
                    buffer.Append("--:--?");
                else
                    buffer.AppendFormat("{0:D2}:{1:D2}s", rminutes, rseconds);
            }
        }

        int RenderSimpleProgressGlyphs(StringBuffer buffer, long progress)
        {
            var numChars = (int)((progress * _maxGlyphWidth) / Total);
            buffer.Append(_selectedBarStyle[0], numChars);
            return numChars;
        }

        int RenderComplexProgressGlyphs(StringBuffer buffer, long progress)
        {
            var blocks = (progress * (_maxGlyphWidth * _selectedBarStyle.Length)) / Total;
            Debug.Assert(blocks >= 0);
            var completeBlocks = (int)(blocks / _selectedBarStyle.Length);
            buffer.Append(_selectedBarStyle[_selectedBarStyle.Length - 1], completeBlocks);
            var lastCharIdx = (int)(blocks % _selectedBarStyle.Length);

            if (lastCharIdx == 0)
                return completeBlocks;

            buffer.Append(_selectedBarStyle[lastCharIdx]);
            return completeBlocks + 1;
        }
    }

    /// <summary>
    /// Represents a Yaap wrapped <see cref="IEnumerable{T}"/> object, where the enumerator progress automatically changes
    /// the Yaap visual representation without further need to manually update the progress state
    /// </summary>
    /// <typeparam name="T">The type of objects to enumerate</typeparam>
    public class YaapEnumerable<T> : Yaap, IEnumerable<T>
    {
        readonly IEnumerable<T> _enumerable;
        static Func<IEnumerable<T>, int> _cheapCount;

        internal YaapEnumerable(IEnumerable<T> e, long total = -1, long initialProgress = 0, YaapSettings settings = null) :
            base(total != -1 ? total : GetCheapCount(e), initialProgress, settings)
        {
            _enumerable = e;
        }

        /// <summary>
        /// Attempt to get a "cheap" count value for the <see cref="IEnumerable{T}"/>, where "cheap" means that the enumerable is
        /// never consumed no matter what
        /// </summary>
        /// <param name="source">The <see cref="IEnumerable{T}"/> object</param>
        /// <returns>The count value, or -1 in case the cheap count failed</returns>
        public static int GetCheapCount(IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            switch (source)
            {
                case ICollection<T> collectionoft:
                    return collectionoft.Count;
                case ICollection collection:
                    return collection.Count;
            }

            return CheapCountDelegate(source);

        }

        #region Avert Your Eyes!
        static Func<IEnumerable<T>, int> CheapCountDelegate
        {
            get
            {
                return _cheapCount ?? (_cheapCount = GenerateGetCount());

                Func<IEnumerable<T>, int> GenerateGetCount()
                {
                    var iilp = typeof(Enumerable).Assembly.GetType("System.Linq.IIListProvider`1");
                    Debug.Assert(iilp != null);
                    var iilpt = iilp.MakeGenericType(typeof(T));
                    Debug.Assert(iilpt != null);
                    var getCountMI = iilpt.GetMethod("GetCount", BindingFlags.Public | BindingFlags.Instance, null,
                        new[] { typeof(bool) }, null);
                    Debug.Assert(getCountMI != null);
                    var param = Expression.Parameter(typeof(IEnumerable<T>));

                    var castAndCall = Expression.Call(Expression.Convert(param, iilpt), getCountMI,
                        Expression.Constant(true));

                    var body = Expression.Condition(Expression.TypeIs(param, iilpt), castAndCall,
                        Expression.Constant(-1));

                    return Expression.Lambda<Func<IEnumerable<T>, int>>(body, new[] { param }).Compile();
                }
            }
        }
        #endregion

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>Returns an enumerator that iterates through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            // In the case of enumerable we can actually start ticking the elapsed clock
            // at a later, more precise, time, so lets do it...
            _sw.Restart();
            foreach (var t in _enumerable)
            {
                yield return t;
                Progress++;
            }
            Dispose();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>
    /// A static extension class that provides <see cref="IEnumerable{T}"/> <see cref="YaapEnumerable{T}"/> wrappers
    /// </summary>
    public static class YaapEnumerableExtensions
    {
        /// <summary>
        /// Wrap the provided <see cref="IEnumerable{T}"/> with a <see cref="YaapEnumerable{T}"/> object
        /// </summary>
        /// <typeparam name="T">The type of objects to enumerate</typeparam>
        /// <param name="e">The <see cref="IEnumerable{T}"/> instance to wrap</param>
        /// <param name="total">The (optional)total number of elements of the wrapped <see cref="IEnumerable{T}"/> that will be enumerated</param>
        /// <param name="initialProgress">The (optional) initial progress value</param>
        /// <param name="settings">The (optional) visual settings overrides</param>
        /// <returns>The newly instantiated <see cref="YaapEnumerable{T}"/> wrapping the provided <see cref="IEnumerable{T}"/></returns>
        public static YaapEnumerable<T> Yaap<T>(this IEnumerable<T> e, long total = -1, long initialProgress = 0, YaapSettings settings = null) =>
        new YaapEnumerable<T>(e, total, initialProgress, settings);
    }

    static class DateTimeDeconstruction
    {
        const long TicksPerMicroSeconds = 10;
        const long TicksPerMillisecond = 10_000;
        const long TicksPerSecond = TicksPerMillisecond * 1_000;
        const long TicksPerMinute = TicksPerSecond * 60;
        const long TicksPerHour = TicksPerMinute * 60;
        const long TicksPerDay = TicksPerHour * 24;

        public static void Deconstruct(this TimeSpan timeSpan, out int days, out int hours, out int minutes, out int seconds, out int ticks)
        {
            if (timeSpan == TimeSpan.MaxValue)
            {
                days = hours = minutes = seconds = ticks = 0;
                return;
            }
            var t = timeSpan.Ticks;
            days = (int)(t / (TicksPerHour * 24));
            hours = (int)((t / TicksPerHour) % 24);
            minutes = (int)((t / TicksPerMinute) % 60);
            seconds = (int)((t / TicksPerSecond) % 60);
            ticks = (int)(t % 10_000_000);
        }
    }

    static class ColorDeconstruction
    {
        public static void Deconstruct(this Color color, out int r, out int g, out int b, out int a)
        {
            r = color.R;
            g = color.G;
            b = color.B;
            a = color.A;
        }
    }
}
