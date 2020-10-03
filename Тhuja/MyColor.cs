using System;

namespace Тhuja
{
    public enum MyColor: byte
    {
        // Dark
        Black = 0,
        DarkRed = 1,
        DarkGreen = 2,
        DarkYellow = 3,
        DarkBlue = 4,
        DarkMagenta = 5,
        DarkCyan = 6,
        DarkGray = 8,
        
        // Bright
        Gray = 7,
        Red = 9,
        Green = 10,
        Yellow = 11,
        Blue = 12,
        Magenta = 13,
        Cyan = 14,
        White = 15,

        Default = 255,
        Transparent = 254,
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