using System;
using System.Drawing;

namespace Thuja
{
    public struct Style
    {
        public readonly MyColor Foreground;
        public readonly MyColor Background;

        public Style(MyColor foreground, MyColor background)
        {
            Foreground = foreground;
            Background = background;
        }

        public bool Equals(Style other)
        {
            return Foreground == other.Foreground && Background == other.Background;
        }

        public override bool Equals(object? obj)
        {
            return obj is Style other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int) Foreground, (int) Background);
        }
    }

    public struct ColoredChar
    {
        public readonly Style Style;
        public readonly char Char;

        public ColoredChar(Style style, char c)
        {
            Style = style;
            Char = c;
        }
        
        public ColoredChar(char c) : this(new Style(), c)
        {
        }

        public static ColoredChar Whitespace = new ColoredChar(' ');

        public bool Equals(ColoredChar other)
        {
            return Style.Equals(other.Style) && Char == other.Char;
        }

        public override bool Equals(object? obj)
        {
            return obj is ColoredChar other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Style, Char);
        }
    }
}