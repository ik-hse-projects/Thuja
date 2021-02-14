using System;
using System.Collections.Generic;

namespace Thuja
{
    /// <summary>
    ///     Цвета консоли.
    /// </summary>
    public enum MyColor : byte
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
        Blue = 9,
        Green = 10,
        Cyan = 11,
        Red = 12,
        Magenta = 13,
        Yellow = 14,
        White = 15,
        Default = 16,
        Transparent = 17
    }

    /// <summary>
    ///     Функции над цветами консоли.
    /// </summary>
    public static class MyColorExt
    {
        /// <summary>
        ///     Кэш преобразования <see cref="MyColor" /> в <see cref="ConsoleColor" />.
        /// </summary>
        private static readonly Dictionary<MyColor, ConsoleColor> Mapping = new Dictionary<MyColor, ConsoleColor>();

        /// <summary>
        ///     Преобразует число в цвет. Если это невозможно, то возвращает цвет по-умолчанию.
        /// </summary>
        public static MyColor FromInt(int b)
        {
            if (0 <= b && b <= 17)
            {
                return (MyColor) b;
            }

            return MyColor.Default;
        }

        /// <summary>
        ///     Преобразует <see cref="MyColor" /> в <see cref="ConsoleColor" />.
        /// </summary>
        public static ConsoleColor ToConsoleColor(this MyColor color)
        {
            if (color == MyColor.Default || color == MyColor.Transparent)
            {
                return default;
            }

            if (Mapping.TryGetValue(color, out var cached))
            {
                return cached;
            }

            // Довольно медленно, но это только пока не заполнится кэш.
            if (Enum.TryParse<ConsoleColor>(color.ToString(), out var parsed))
            {
                Mapping.Add(color, parsed);
                return parsed;
            }

            // Не должно доходить до этого места, но на всякий случай не кидаем исключение.
            return default;
        }
    }
}