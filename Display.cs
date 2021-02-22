using System;
using System.Linq;

namespace Thuja
{
    /// <summary>
    ///     Класс, который занмается отрисовкой холста на экран консоли.
    /// </summary>
    public class Display
    {
        /// <summary>
        ///     Предыдущий холст.
        /// </summary>
        private Canvas? prev;

        /// <summary>
        ///     Создаёт новый, пустой дисплей.
        /// </summary>
        public Display()
        {
            prev = null;
            CurrentScreen = new Canvas(Console.WindowWidth, Console.WindowHeight);
        }

        /// <summary>
        ///     Текущий холст, на котором следует рисовать.
        /// </summary>
        public Canvas CurrentScreen { get; private set; }

        /// <summary>
        ///     Сбраасывает настройки консоли и очищает её.
        /// </summary>
        public void Clear()
        {
            Console.ResetColor();
            Console.Clear();
            prev = new Canvas(CurrentScreen.Size.width, CurrentScreen.Size.height);
        }

        /// <summary>
        ///     Отрисовывает текущий слой на экран.
        /// </summary>
        public void Draw()
        {
            var (cursorLeft, cursorTop) = (Console.CursorLeft, Console.CursorTop);

            Console.CursorVisible = false;
            if (prev == null || prev.Size != CurrentScreen.Size)
            {
                Clear();
            }

            foreach (var difference in CurrentScreen.FindDifferences(prev!))
            {
                DrawDifference(difference);
            }

            Console.SetCursorPosition(cursorLeft, cursorTop);
            Console.CursorVisible = true;

            Swap();
        }

        /// <summary>
        ///     Подготавливает дисплей к отрисовке нового содержимого.
        /// </summary>
        private void Swap()
        {
            var width = Console.WindowWidth;
            var height = Console.WindowHeight;

            if (prev != null && prev.Size == (width, height))
            {
                (CurrentScreen, prev) = (prev, CurrentScreen);
                CurrentScreen.Clear();
            }
            else
            {
                prev = null;
                CurrentScreen = new Canvas(width, height);
            }
        }

        /// <summary>
        ///     Отрисовывает ровно одно отличие.
        /// </summary>
        /// <param name="difference">Отличие, которое должно быть отрисовано.</param>
        private static void DrawDifference(Difference difference)
        {
            Console.SetCursorPosition(difference.Column, difference.Line);
            foreach (var (style, coloredChars) in difference.Chars.MakeGroups(c => c.Style))
            {
                var chars = coloredChars.Select(c => c.Char).ToArray();
                var str = new string(chars);
                WriteString(style, str);
            }
        }

        /// <summary>
        ///     Отрисовывает строку текста со стилем.
        /// </summary>
        /// <param name="style">Стиль символов строки.</param>
        /// <param name="str">Символы строки</param>
        private static void WriteString(Style style, string str)
        {
            if (style.Foreground == MyColor.Transparent)
            {
                str = new string(' ', str.Length);
            }

            var background = style.Background == MyColor.Transparent
                ? MyColor.Default
                : style.Background;

            var changed = false;
            if (style.Foreground != MyColor.Default)
            {
                Console.ForegroundColor = style.Foreground.ToConsoleColor();
                changed = true;
            }

            if (background != MyColor.Default)
            {
                Console.BackgroundColor = background.ToConsoleColor();
                changed = true;
            }

            Console.Write(str);

            if (changed)
            {
                Console.ResetColor();
            }
        }
    }
}