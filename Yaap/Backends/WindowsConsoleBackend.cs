using System;
using System.Collections.Generic;
using System.Text.Formatting;
using System.Threading;

namespace Yaap.Backends {
    internal class WindowsConsoleBackend : IYaapBackend
    {
        readonly object _consoleLock = new object();
        bool _wasCursorHidden;
        readonly StringBuffer _buffer = new StringBuffer(Console.WindowWidth * 10);
        char[] _chars = new char[Console.WindowWidth * 10];

        public void UpdateAllYaaps(ICollection<Yaap> instances)
        {
            bool lockWasTaken = false;
            try {
                _wasCursorHidden = false;
                foreach (var y in instances) {
                    if (!y.NeedsRepaint) {
                        continue;
                    }

                    if (!lockWasTaken)
                        Monitor.Enter(_consoleLock, ref lockWasTaken);

                    if (!_wasCursorHidden) {
                        Console.CursorVisible = false;
                        _wasCursorHidden      = true;
                    }

                    UpdateSingleYaap(y);
                }


                if (_wasCursorHidden) {
                    Console.CursorVisible = true;
                    _wasCursorHidden      = false;
                }

                if (lockWasTaken) {
                    lockWasTaken = false;
                    Monitor.Exit(_consoleLock);
                }

            }
            finally {
                if (lockWasTaken)
                    Monitor.Exit(_consoleLock);
            }        }

        public bool UpdateSingleYaap(Yaap yaap)
        {
            _buffer.Clear();
            var (x, y) = MoveTo(yaap);
            _buffer.Append('\r');
            _buffer.Append(ANSICodes.EraseToLineEnd);
            yaap.Repaint(_buffer);
            SpillBuffer();
            MoveTo(x, y);

        }

        public void ClearSingleYaap(Yaap yaap)
        {
            lock (_consoleLock) {
                var (x, y) = MoveTo(yaap);
                yaap.Repaint(_buffer);
                // Looks silly eh?
                // The reason is we don't want to bother to understand how many printable characters are in the buffer
                // so we simply backspace _buffer.Count and we know for sure we've deleted the entire line
                // without bothering to decode VT100
                _buffer.Append('\b', _buffer.Count);
                SpillBuffer();
                MoveTo(x, y);
            }
        }


        void SpillBuffer()
        {
            if (_buffer.Count > _chars.Length)
                Array.Resize(ref _chars, _buffer.Count);
            _buffer.CopyTo(0, _chars, 0, _buffer.Count);
            Console.Write(_chars, 0, _buffer.Count);
        }

        static void MoveTo(int x, int y) => Console.SetCursorPosition(x, y);

        static (int x, int y) MoveTo(Yaap yaap)
        {
            var (x, y) = Win32Console.CursorPosition;
            switch (yaap.Settings.Positioning) {
                case YaapPositioning.FlowAndSnapToTop:
                    Console.CursorTop = Math.Max(0, y - (_maxYaapPosition - yaap.Position + _totalLinesAddedAfterYaaps));
                    break;
                case YaapPositioning.ClearAndAlignToTop:
                case YaapPositioning.FixToBottom:
                    Console.CursorTop = yaap.Position;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return (x, y);

        }


        public void Dispose()
        {
            throw new NotImplementedException();
        }

    }
}
