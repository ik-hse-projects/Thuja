using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thuja.Widgets
{
    public readonly struct CharRange
    {
        public static CharRange Ascii = new CharRange(' ', '~');
        public static CharRange Digits = new CharRange('0', '9');
        
        public readonly char Start;
        public readonly char End;

        public CharRange(char start, char end)
        {
            if (start > end)
            {
                throw new ArgumentException("start > end");
            }
            Start = start;
            End = end;
        }
    }

    public readonly struct Placeholder
    {
        public readonly Style Style;
        public readonly string Text;

        public Placeholder(Style style, string text)
        {
            Style = style;
            Text = text;
        }
    }

    public class InputField : IWidget
    {
        public int MaxLength { get; set; } = int.MaxValue;
        public HashSet<CharRange> AllowedChars { get; } = new HashSet<CharRange>();
        public Style ActiveStyle { get; set; } = new Style(MyColor.White, MyColor.DarkGray);
        public Style InactiveStyle { get; set; } = new Style(MyColor.Gray, MyColor.DarkGray);
        public StringBuilder Text { get; } = new StringBuilder();
        public Placeholder? Placeholder { get; set; }

        private bool isFocused = false;

        private int cursorLeft = 0;

        private int CursorLeft
        {
            get => cursorLeft;
            set
            {
                if (value < 0)
                    cursorLeft = 0;
                else if (value > Text.Length)
                    cursorLeft = Text.Length;
                else
                    cursorLeft = value;
            }
        }

        public (int x, int y, int layer) Position { get; set; }

        public ColoredChar[,] Render()
        {
            string text;
            Style style;
            if (!isFocused && Text.Length == 0)
            {
                text = Placeholder?.Text ?? "";
                style = Placeholder?.Style ?? ActiveStyle;
            }
            else
            {
                text = Text.ToString();
                style = isFocused ? ActiveStyle : InactiveStyle;
            }

            if (MaxLength != int.MaxValue && text.Length < MaxLength)
            {
                text += new string('_', MaxLength - text.Length);
            }

            return text.ToColoredRow(style);
        }

        public int Fps { get; } = 0;

        public void Update(Tick tick)
        {
            isFocused = tick.IsFocused;
            if (isFocused)
            {
                Console.CursorLeft = Position.x + CursorLeft;
                Console.CursorTop = Position.y;
            }
        }

        private void Del()
        {
            if (cursorLeft < Text.Length)
            {
                Text.Remove(cursorLeft, 1);
            }
        }

        public bool BubbleDown(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.LeftArrow:
                    CursorLeft--;
                    return true;
                case ConsoleKey.RightArrow:
                    CursorLeft++;
                    return true;
                case ConsoleKey.End:
                    CursorLeft = Text.Length - 1;
                    return true;
                case ConsoleKey.Home:
                    CursorLeft = 0;
                    return true;
                case ConsoleKey.Backspace:
                    CursorLeft--;
                    Del();
                    return true;
                case ConsoleKey.Delete:
                    Del();
                    return true;
            }

            var ch = key.KeyChar;
            if (AllowedChars.Any(range => ch >= range.Start && ch <= range.End))
            {
                if (Text.Length < MaxLength)
                {
                    Text.Insert(CursorLeft, ch);
                    CursorLeft++;
                }

                return true;
            }

            return false;
        }
    }
}