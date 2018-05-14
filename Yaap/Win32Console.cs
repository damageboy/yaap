using System;
using System.Runtime.InteropServices;

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

        /// <summary>
        /// Retrieves a handle to the specified standard device (standard input, standard output, or standard error).
        /// </summary>
        /// <param name="nStdHandle">The standard device.</param>
        /// <returns>
        /// If the function succeeds, the return value is a handle to the specified device, or a redirected handle set by a previous call to <see cref="SetStdHandle"/>. The handle has GENERIC_READ and GENERIC_WRITE access rights, unless the application has used <see cref="SetStdHandle"/> to set a standard handle with lesser access.
        /// If the function fails, the return value is <see cref="INVALID_HANDLE_VALUE"/>. To get extended error information, call <see cref="GetLastError"/>.
        /// If an application does not have associated standard handles, such as a service running on an interactive desktop, and has not redirected them, the return value is NULL.
        /// </returns>
        [DllImport(nameof(Kernel32), SetLastError = true)]
        static extern IntPtr GetStdHandle(StdHandle nStdHandle);

        /// <summary>
        /// Sets the handle for the specified standard device (standard input, standard output, or standard error).
        /// </summary>
        /// <param name="nStdHandle">The standard device for which the handle is to be set.</param>
        /// <param name="nHandle">The handle for the standard device.</param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero. To get extended error information, call <see cref="GetLastError"/>.
        /// </returns>
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
        public class CONSOLE_FONT_INFOEX
        {
            int cbSize = Marshal.SizeOf(typeof(CONSOLE_FONT_INFOEX));
            public int FontIndex;
            public short FontWidth;
            public short FontHeight;
            public int FontFamily;
            public int FontWeight;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string FaceName;
        }


        /// <summary>
        /// Designates the console buffer mode on the <see cref="GetConsoleMode(IntPtr, out ConsoleBufferModes)"/> and <see cref="SetConsoleMode(IntPtr, ConsoleBufferModes)"/> functions
        /// </summary>
        [Flags]
        enum ConsoleBufferModes
        {
            /// <summary>
            /// CTRL+C is processed by the system and is not placed in the input buffer.
            /// If the input buffer is being read by <see cref="ReadFile(SafeObjectHandle, void*, int)"/> or <see cref="ReadConsole(IntPtr, void*, int, int, IntPtr)"/>,
            /// other control keys are processed by the system and are not returned in the <see cref="ReadFile(SafeObjectHandle, void*, int)"/> or <see cref="ReadConsole(IntPtr, void*, int, int, IntPtr)"/> buffer.
            /// If the <see cref="ENABLE_LINE_INPUT"/> mode is also enabled, backspace, carriage return, and line feed characters are handled by the system.
            /// </summary>
            ENABLE_PROCESSED_INPUT = 0x0001,

            /// <summary>
            /// Characters written by the <see cref="WriteFile(SafeObjectHandle, void*, int)"/> or <see cref="WriteConsole(IntPtr, void*, int, int*, IntPtr)"/> function
            /// or echoed by the <see cref="ReadFile(SafeObjectHandle, void*, int)"/> or <see cref="ReadConsole(IntPtr, void*, int, int, IntPtr)"/> function are parsed for ASCII control sequences, and the correct action is performed.
            /// Backspace, tab, bell, carriage return, and line feed characters are processed.
            /// </summary>
            ENABLE_PROCESSED_OUTPUT = 0x0001,

            /// <summary>
            /// The <see cref="ReadFile(SafeObjectHandle, void*, int)"/> or <see cref="ReadConsole(IntPtr, void*, int, int, IntPtr)"/> function returns only when a carriage return character is read.
            /// If this mode is disabled, the functions return when one or more characters are available.
            /// </summary>
            ENABLE_LINE_INPUT = 0x0002,

            /// <summary>
            /// When writing with <see cref="WriteFile(SafeObjectHandle, void*, int)"/> or <see cref="WriteConsole(IntPtr, void*, int, int*, IntPtr)"/> or echoing with <see cref="ReadFile(SafeObjectHandle, void*, int)"/> or <see cref="ReadConsole(IntPtr, void*, int, int, IntPtr)"/>,
            /// the cursor moves to the beginning of the next row when it reaches the end of the current row. This causes the rows displayed in the console window to scroll up automatically when the cursor advances beyond the last row in the window.
            /// It also causes the contents of the console screen buffer to scroll up (discarding the top row of the console screen buffer) when the cursor advances beyond the last row in the console screen buffer.
            /// If this mode is disabled, the last character in the row is overwritten with any subsequent characters.
            /// </summary>
            ENABLE_WRAP_AT_EOL_OUTPUT = 0x0002,

            /// <summary>
            /// Characters read by the <see cref="ReadFile(SafeObjectHandle, void*, int)"/> or <see cref="ReadConsole(IntPtr, void*, int, int, IntPtr)"/> function are written to the active screen buffer as they are read.
            /// This mode can be used only if the <see cref="ENABLE_LINE_INPUT"/> mode is also enabled.
            /// </summary>
            ENABLE_ECHO_INPUT = 0x0004,

            /// <summary>
            /// When writing with <see cref="WriteFile(SafeObjectHandle, void*, int)"/> or <see cref="WriteConsole(IntPtr, void*, int, int*, IntPtr)"/>, characters are parsed for VT100 and similar control character sequences that control cursor movement,
            /// color/font mode, and other operations that can also be performed via the existing Console APIs.
            /// </summary>
            ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004,

            /// <summary>
            /// User interactions that change the size of the console screen buffer are reported in the console's input buffer.
            /// Information about these events can be read from the input buffer by applications using the <see cref="ReadConsoleInput"/> function, but not by those using <see cref="ReadFile(SafeObjectHandle, void*, int)"/> or <see cref="ReadConsole(IntPtr, void*, int, int, IntPtr)"/>.
            /// </summary>
            ENABLE_WINDOW_INPUT = 0x0008,

            /// <summary>
            /// When writing with <see cref="WriteFile(SafeObjectHandle, void*, int)"/> or <see cref="WriteConsole(IntPtr, void*, int, int*, IntPtr)"/>, this adds an additional state to end-of-line wrapping that can delay the cursor move and buffer scroll operations.
            /// Normally when <see cref="ENABLE_WRAP_AT_EOL_OUTPUT"/> is set and text reaches the end of the line, the cursor will immediately move to the next line and the contents of the buffer will scroll up by one line. In contrast with this flag set,
            /// the scroll operation and cursor move is delayed until the next character arrives. The written character will be printed in the final position on the line and the cursor will remain above this character as if <see cref="ENABLE_WRAP_AT_EOL_OUTPUT"/> was off,
            /// but the next printable character will be printed as if ENABLE_WRAP_AT_EOL_OUTPUT is on. No overwrite will occur. Specifically, the cursor quickly advances down to the following line, a scroll is performed if necessary, the character is printed, and the cursor advances one more position.
            /// The typical usage of this flag is intended in conjunction with setting ENABLE_VIRTUAL_TERMINAL_PROCESSING to better emulate a terminal emulator where writing the final character on the screen (in the bottom right corner) without triggering an immediate scroll is the desired behavior.
            /// </summary>
            DISABLE_NEWLINE_AUTO_RETURN = 0x0008,

            /// <summary>
            /// If the mouse pointer is within the borders of the console window and the window has the keyboard focus, mouse events generated by mouse movement and button presses are placed in the input buffer.
            /// These events are discarded by <see cref="ReadFile(SafeObjectHandle, void*, int)"/> or <see cref="ReadConsole(IntPtr, void*, int, int, IntPtr)"/>, even when this mode is enabled.
            /// </summary>
            ENABLE_MOUSE_INPUT = 0x0010,

            /// <summary>
            /// The APIs for writing character attributes including <see cref="WriteConsoleOutput(IntPtr, CHAR_INFO*, COORD, COORD, SMALL_RECT*)"/> and WriteConsoleOutputAttribute allow the usage of flags from character attributes to adjust the color of the foreground and background of text.
            /// Additionally, a range of DBCS flags was specified with the COMMON_LVB prefix. Historically, these flags only functioned in DBCS code pages for Chinese, Japanese, and Korean languages.
            /// With exception of the leading byte and trailing byte flags, the remaining flags describing line drawing and reverse video (swap foreground and background colors) can be useful for other languages to emphasize portions of output.
            /// Setting this console mode flag will allow these attributes to be used in every code page on every language.
            /// It is off by default to maintain compatibility with known applications that have historically taken advantage of the console ignoring these flags on non-CJK machines to store bits in these fields for their own purposes or by accident.
            /// Note that using the <see cref="ENABLE_VIRTUAL_TERMINAL_PROCESSING"/> mode can result in LVB grid and reverse video flags being set while this flag is still off if the attached application requests underlining or inverse video via Console Virtual Terminal Sequences.
            /// </summary>
            ENABLE_LVB_GRID_WORLDWIDE = 0x0010,

            /// <summary>
            /// When enabled, text entered in a console window will be inserted at the current cursor location and all text following that location will not be overwritten.
            /// When disabled, all following text will be overwritten.
            /// </summary>
            ENABLE_INSERT_MODE = 0x0020,

            /// <summary>
            /// This flag enables the user to use the mouse to select and edit text.
            /// </summary>
            ENABLE_QUICK_EDIT_MODE = 0x0040,

            /// <summary>
            /// Setting this flag directs the Virtual Terminal processing engine to convert user input received by the console window into Console Virtual Terminal Sequences that can be retrieved by a supporting application through ReadFile or ReadConsole functions.
            /// The typical usage of this flag is intended in conjunction with ENABLE_VIRTUAL_TERMINAL_PROCESSING on the output handle to connect to an application that communicates exclusively via virtual terminal sequences.
            /// </summary>
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
