using JetBrains.Annotations;

namespace Yaap
{
    static internal class YaapBarStyleDefs
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        internal static readonly char[][] Glyphs = {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new[] {'▏', '▎', '▍', '▌', '▋', '▊', '▉', '█'},
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new[] {'▁', '▂', '▃', '▄', '▅', '▆', '▇', '█'},
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new[] {'⣀', '⣄', '⣤', '⣦', '⣶', '⣷', '⣿'},
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new[] {'⣀', '⣄', '⣆', '⣇', '⣧', '⣷', '⣿'},
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new[] {'○', '◔', '◐', '◕', '⬤'},
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new[] {'□', '◱', '◧', '▣', '■'},
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new[] {'□', '◱', '▨', '▩', '■'},
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new[] {'□', '◱', '▥', '▦', '■'},
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new[] {'⬜', '⬛'},
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new[] {'░', '▒', '▓', '█'},
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new[] {'░', '█'},
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new[] {'▱', '▰'},
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new[] {'▭', '◼'},
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new[] {'▯', '▮'},
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new[] {'◯', '⬤'},
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new[] {'⚪', '⚫'},
        };
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
}
