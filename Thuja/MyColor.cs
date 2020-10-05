using System;
using System.Collections.Generic;

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
        Transparent = 17,
    }

    public static class MyColorExt
    {
        private static Dictionary<MyColor, ConsoleColor> _mapping = new Dictionary<MyColor, ConsoleColor>();

        public static MyColor FromInt(int b)
        {
            if (b <= 17)
            {
                return (MyColor) b;
            }

            return MyColor.Default;
        }
        
        public static ConsoleColor ToConsoleColor(this MyColor color)
        {
            if (color == MyColor.Default || color == MyColor.Transparent)
            {
                return default;
            }
            
            if (_mapping.TryGetValue(color, out var cached))
            {
                return cached;
            }
            // Slow, but robust
            if (Enum.TryParse<ConsoleColor>(color.ToString(), out var parsed))
            {
                _mapping.Add(color, parsed);
                return parsed;
            }

            // Should not be here, but we do not want to crash because of colors
            return default;
        }
    }
}