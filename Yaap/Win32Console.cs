using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Yaap
{
    /// <summary>
    /// All of the pivoke stuff below is shamelssly stolen from https://github.com/AArnott/pinvoke
    /// The reason for this IP theft is that the netstandard nuget packages for netstandard DO NOT include
    /// any of the console stuff in to weird decision I don't care to understand
    /// </summary>
    internal class Win32Console
    {
        const string Kernel32 = "kernel32.dll";
        [DllImport(nameof(Kernel32), SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out ConsoleBufferModes lpMode);

        [DllImport(nameof(Kernel32), SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, ConsoleBufferModes dwMode);

        [DllImport(nameof(Kernel32), SetLastError = true)]
        static extern IntPtr GetStdHandle(StdHandle nStdHandle);

        [DllImport(nameof(Kernel32), SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetStdHandle(StdHandle nStdHandle, IntPtr nHandle);

        [DllImport(nameof(Kernel32), SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetConsoleOutputCP(uint wCodePageID);

        [DllImport(nameof(Kernel32), SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetConsoleCP(uint wCodePageID);

        [DllImport(nameof(Kernel32), SetLastError = true)]
        static extern uint GetConsoleOutputCP();

        [DllImport(nameof(Kernel32), SetLastError = true)]
        static extern uint GetConsoleCP();

        [DllImport(Kernel32, SetLastError = true)]
        static extern bool GetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool bMaximumWindow, [In, Out] CONSOLE_FONT_INFOEX lpConsoleCurrentFont);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal class CONSOLE_FONT_INFOEX
        {
            readonly int cbSize = Marshal.SizeOf(typeof(CONSOLE_FONT_INFOEX));
            internal  int FontIndex;
            internal  short FontWidth;
            internal  short FontHeight;
            internal  int FontFamily;
            internal  int FontWeight;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            internal  string FaceName;
        }


        /// <summary>
        /// Designates the console buffer mode on the <see cref="GetConsoleMode(IntPtr, out ConsoleBufferModes)"/> and <see cref="SetConsoleMode(IntPtr, ConsoleBufferModes)"/> functions
        /// </summary>
        [Flags]
        enum ConsoleBufferModes
        {
            ENABLE_PROCESSED_INPUT = 0x0001,

            ENABLE_PROCESSED_OUTPUT = 0x0001,

            ENABLE_LINE_INPUT = 0x0002,

            ENABLE_WRAP_AT_EOL_OUTPUT = 0x0002,

            ENABLE_ECHO_INPUT = 0x0004,

            ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004,

            ENABLE_WINDOW_INPUT = 0x0008,

            DISABLE_NEWLINE_AUTO_RETURN = 0x0008,

            ENABLE_MOUSE_INPUT = 0x0010,

            ENABLE_LVB_GRID_WORLDWIDE = 0x0010,

            ENABLE_INSERT_MODE = 0x0020,

            ENABLE_QUICK_EDIT_MODE = 0x0040,

            ENABLE_VIRTUAL_TERMINAL_INPUT = 0x0200,
        }

        /// <summary>
        /// Standard handles for the <see cref="GetStdHandle(StdHandle)"/> and <see cref="SetStdHandle"/> methods.
        /// </summary>
        [Flags]
        enum StdHandle
        {
            /// <summary>
            /// The standard input device. Initially, this is the console input buffer, CONIN$.
            /// </summary>
            STD_INPUT_HANDLE = -10,

            /// <summary>
            /// The standard output device. Initially, this is the active console screen buffer, CONOUT$.
            /// </summary>
            STD_OUTPUT_HANDLE = -11,

            /// <summary>
            /// The standard error device. Initially, this is the active console screen buffer, CONOUT$.
            /// </summary>
            STD_ERROR_HANDLE = -12,
        }

        static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
        static ConsoleBufferModes _originalOutMode, _originalInMode;
        static uint _originalConsoleOutCP, _originalConsoleCP;
        static string _consoleFontName;

        internal static string ConsoleFontName
        {
            get {
                if (_consoleFontName != null)
                    return _consoleFontName;
                // Set output mode to handle virtual terminal sequences
                var hOut = GetStdHandle(StdHandle.STD_OUTPUT_HANDLE);
                if (hOut == INVALID_HANDLE_VALUE)
                    return _consoleFontName = string.Empty;

                var consoleInfo = new CONSOLE_FONT_INFOEX();
                GetCurrentConsoleFontEx(hOut, false, consoleInfo);
                return _consoleFontName = consoleInfo.FaceName;
            }
        }

        /// <summary>
        /// Adapted from:
        /// https://docs.microsoft.com/en-us/windows/console/console-virtual-terminal-sequences
        /// </summary>
        /// <returns></returns>
        internal static bool EnableVT100Stuffs()
        {
            // Set output mode to handle virtual terminal sequences
            var hOut = GetStdHandle(StdHandle.STD_OUTPUT_HANDLE);
            if (hOut == INVALID_HANDLE_VALUE)
                return false;
            var hIn = GetStdHandle(StdHandle.STD_INPUT_HANDLE);
            if (hIn == INVALID_HANDLE_VALUE)
                return false;

            if (!GetConsoleMode(hOut, out _originalOutMode))
                return false;
            if (!GetConsoleMode(hIn, out _originalInMode))
                return false;

            // Apparently this can't fail?
            _originalConsoleOutCP = GetConsoleOutputCP();
            _originalConsoleCP = GetConsoleCP();

            var dwOutMode = _originalOutMode | ConsoleBufferModes.ENABLE_VIRTUAL_TERMINAL_PROCESSING;

            if (!SetConsoleMode(hOut, dwOutMode))
                return false; // Failed to set any VT mode, can't do anything here.

            var dwInMode = _originalInMode | ConsoleBufferModes.ENABLE_VIRTUAL_TERMINAL_INPUT;
            if (!SetConsoleMode(hIn, dwInMode))
                return false; // Failed to set VT input mode, can't do anything here.

            Console.OutputEncoding = Encoding.UTF8;
            const uint CP_UTF8 = 65001;
            SetConsoleOutputCP(CP_UTF8);
            SetConsoleCP(CP_UTF8);
            return true;
        }

        internal static void RestoreTerminalToPristineState()
        {
            // Set output mode to handle virtual terminal sequences
            var hOut = GetStdHandle(StdHandle.STD_OUTPUT_HANDLE);
            if (hOut == INVALID_HANDLE_VALUE)
                return;
            var hIn = GetStdHandle(StdHandle.STD_INPUT_HANDLE);
            if (hIn == INVALID_HANDLE_VALUE)
                return;

            SetConsoleMode(hOut, _originalOutMode);
            SetConsoleMode(hIn, _originalInMode);
            SetConsoleOutputCP(_originalConsoleOutCP);
            SetConsoleCP(_originalConsoleCP);
        }
    }
}
