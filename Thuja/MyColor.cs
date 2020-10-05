using System;

namespace Thuja
{
    public enum MyColor: byte
    {
        Black = 0,
        DarkBlue = 1,
        DarkGreen = 2,
        DarkCyan = 3,
        DarkRed = 4,
        DarkMagenta = 5,
        DarkYellow = 6,
        Gray = 7,
        DarkGray = 8,
        Blue = 10,
        Green = 11,
        Cyan = 12,
        Red = 13,
        Magenta = 14,
        Yellow = 14,
        White = 15,
        Default = 16,
        Transparent = 32,
    }

    public static class MyColorExt
    {
        public static ConsoleColor ToConsoleColor(this MyColor color)
        {
            if (color == MyColor.Default || color == MyColor.Transparent)
            {
                return default;
            }

            // Slow, but robust
            if (Enum.TryParse<ConsoleColor>(color.ToString(), out var res))
            {
                return res;
            }

            // Should not be here, but we do not want to crash because of colors
            return default;
        }
    }
}