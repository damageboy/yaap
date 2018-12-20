using JetBrains.Annotations;

namespace Yaap
{
    /// <summary>
    /// A wrapper for <see cref="System.Console"/> Write*() APIs that makes sure the console writing doesn't
    /// interefere with Yaap operations
    /// </summary>
    public static class YaapConsole
    {
        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// </summary>
        /// <param name="s">The value to write.</param>
        /// <remarks>If value is null, nothing is written to the standard output stream.</remarks>
        [PublicAPI]
        public static void Write(string s) => YaapRegistry.Write(s);

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="s">The value to write.</param>
        [PublicAPI]
        public static void WriteLine(string s) => YaapRegistry.WriteLine(s);

        /// <summary>
        /// Writes the current line terminator to the standard output stream.
        /// </summary>
        [PublicAPI]
        public static void WriteLine() => YaapRegistry.WriteLine();

        /// <summary>
        /// Clear the entire terminal screen
        /// </summary>
        public static void ClearScreen() => YaapRegistry.ClearScreen();
    }
}
