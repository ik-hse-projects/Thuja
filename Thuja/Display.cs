using System;
using System.Linq;

namespace Thuja
{
    public class Display
    {
        private Canvas? _prev;
        private Canvas _curr;

        public Display()
        {
            _prev = null;
            _curr = new Canvas(Console.WindowWidth, Console.WindowHeight);
        }

        public Canvas CurrentScreen => _curr;

        public void Clear()
        {
            Console.Clear();
            _prev = new Canvas(_curr.Size.width, _curr.Size.height);
        }

        public void Draw()
        {
            var position = (Console.CursorLeft, Console.CursorTop);

            Console.CursorVisible = false;
            if (_prev == null || _prev.Size != _curr.Size)
            {
                Clear();
            }
            
            foreach (var difference in _curr.FindDifferences(_prev))
            {
                DrawDifference(difference);
            }

            Console.SetCursorPosition(position.CursorLeft, position.CursorTop);
            Console.CursorVisible = true;

            Swap();
        }

        private void DrawDifference(Difference difference)
        {
            Console.SetCursorPosition(difference.Column, difference.Line);
            foreach (var (style, coloredChars) in difference.Chars.MakeGroups(c => c.Style))
            {
                var chars = coloredChars.Select(c => c.Char).ToArray();
                var str = new string(chars);
                WriteString(style, str);
            }
        }

        private void Swap()
        {
            var width = Console.WindowWidth;
            var height = Console.WindowHeight;

            if (_prev != null && _prev.Size == (width, height))
            {
                (_curr, _prev) = (_prev, _curr);
                _curr.Clear();
            }
            else
            {
                _prev = null;
                _curr = new Canvas(width, height);
            }
        }

        public static void WriteString(Style style, string str)
        {
            if (style.Foreground == MyColor.Transparent && style.Background == MyColor.Transparent)
            {
                Console.SetCursorPosition(Console.CursorLeft + str.Length, Console.CursorTop);
                return;
            }

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

            if (changed)
            {
                Console.ResetColor();
            }
        }
    }
}