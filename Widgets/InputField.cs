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
        public static CharRange Letters = new CharRange('A', 'z');

        private readonly char start;
        private readonly char end;

        private CharRange(char start, char end)
        {
            if (start > end) throw new ArgumentException("start > end");

            this.start = start;
            this.end = end;
        }

        public bool Check(char ch)
        {
            return start <= ch && ch <= end;
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

    public class InputField : IKeyHandler
    {
        private int cursorLeft;

        private bool isFocused;
        public int MaxLength { get; set; } = int.MaxValue;
        public HashSet<CharRange> AllowedChars { get; } = new HashSet<CharRange>();
        public Style ActiveStyle { get; set; } = Style.Active;
        public Style InactiveStyle { get; set; } = Style.Inactive;
        public StringBuilder Text { get; } = new StringBuilder();
        public Placeholder? Placeholder { get; set; }
        
        public Dictionary<HashSet<KeySelector>, Action> Actions { get; } = new Dictionary<HashSet<KeySelector>, Action>();

        public IKeyHandler AsIKeyHandler() => this;

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

        public void Render(RenderContext context)
        {
            if (isFocused) context.CursorPosition = (cursorLeft, 0);

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

            if (MaxLength != int.MaxValue && text.Length < MaxLength) text += new string('_', MaxLength - text.Length);

            context.PlaceString(text, style);
        }

        public bool BubbleDown(ConsoleKeyInfo key)
        {
            if (AsIKeyHandler().TryHandleKey(key))
            {
                return true;
            }
            
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
            if (AllowedChars.Count == 0 || AllowedChars.Any(range => range.Check(ch)))
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

        public void FocusChange(bool isFocused)
        {
            this.isFocused = isFocused;
        }

        private void Del()
        {
            if (cursorLeft < Text.Length) Text.Remove(cursorLeft, 1);
        }
    }
}