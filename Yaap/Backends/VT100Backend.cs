using System;
using System.Collections.Generic;
using System.Text.Formatting;

using static Yaap.YaapRegistry;

namespace Yaap.Backends {
    internal class VT100Backend : IYaapBackend
    {
        readonly StringBuffer _buffer = new StringBuffer(Console.WindowWidth * 10);
        char[] _chars = new char[Console.WindowWidth * 10];

        public void UpdateAllYaaps(ICollection<Yaap> instances)
        {

            _buffer.Clear();
            foreach (var y in instances) {
                if (!y.NeedsRepaint) {
                    continue;
                }

                if (_buffer.Count == 0) {
                    _buffer.Append(ANSICodes.SaveCursorPosition);
                }

                AppendYaapToBuffer(y, _buffer);
            }

            if (_buffer.Count > 0) {
                _buffer.Append(ANSICodes.RestoreCursorPosition);
                SpillBuffer();
            }
        }

        static void AppendYaapToBuffer(Yaap yaap, StringBuffer buffer)
        {
            buffer.AppendFormat(ANSICodes.CSI + "{0}d", yaap.Position + 1);
            buffer.Append('\r');
            buffer.Append(ANSICodes.EraseToLineEnd);
            yaap.Repaint(buffer);
        }

        public void UpdateSingleYaap(Yaap yaap)
        {
            _buffer.Clear();
            _buffer.Append(ANSICodes.SaveCursorPosition);
            AppendYaapToBuffer(yaap, _buffer);
            _buffer.Append(ANSICodes.RestoreCursorPosition);
            SpillBuffer();
        }

        public void ClearSingleYaap(Yaap yaap)
        {
            _buffer.Append(ANSICodes.SaveCursorPosition);
            _buffer.AppendFormat(ANSICodes.CSI + "{0}d", yaap.Position + 1);
            _buffer.Append(ANSICodes.EraseEntireLine);
            _buffer.Append(ANSICodes.RestoreCursorPosition);
            SpillBuffer();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        void SpillBuffer()
        {
            if (_buffer.Count > _chars.Length)
                Array.Resize(ref _chars, _buffer.Count);
            _buffer.CopyTo(0, _chars, 0, _buffer.Count);
            Console.Write(_chars, 0, _buffer.Count);
        }
    }
}
