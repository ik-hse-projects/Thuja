using System;
using System.Linq;

namespace Thuja
{
    public class Display
    {
        private Canvas? prev;

        public Display()
        {
            prev = null;
            CurrentScreen = new Canvas(Console.WindowWidth, Console.WindowHeight);
        }

        public Canvas CurrentScreen { get; private set; }

        public void Clear()
        {
            Console.Clear();
            prev = new Canvas(CurrentScreen.Size.width, CurrentScreen.Size.height);
        }

        public void Draw()
        {
            var (cursorLeft, cursorTop) = (Console.CursorLeft, Console.CursorTop);

            Console.CursorVisible = false;
            if (prev == null || prev.Size != CurrentScreen.Size) Clear();

            foreach (var difference in CurrentScreen.FindDifferences(prev)) DrawDifference(difference);

            Console.SetCursorPosition(cursorLeft, cursorTop);
            Console.CursorVisible = true;

            Swap();
        }

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

        private static void WriteString(Style style, string str)
        {
            // Draw transparent foreground as whitespace.
            if (style.Foreground == MyColor.Transparent)
            {
                str = new string(' ', str.Length);
            }

            // Treat transparent background as default.
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

            if (changed) Console.ResetColor();
        }
    }
}