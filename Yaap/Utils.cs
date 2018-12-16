using System;
using System.Drawing;

namespace Yaap
{
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

    internal static class ANSICodes
    {
        public const string ESC = "\u001B";
        public const string ResetTerminal = ESC + "c";
        public const string CSI = ESC +"[";
        public const string ClearScreen = CSI + "2J";
        public const string EraseToLineEnd = CSI + "K";
        public const string EraseEntireLine = CSI + "2K";
        public const string EraseToLineStart = CSI + "1K";

        internal static void SetScrollableRegion(int top, int bottom) =>
            Console.Write($"{CSI}{top};{bottom}r");
    }
}
