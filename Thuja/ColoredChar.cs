using System;
using System.Drawing;

namespace Thuja
{
    public struct Style
    {
        public readonly MyColor Foreground;
        public readonly MyColor Background;
        public readonly ushort Flags;

        public Style(MyColor foreground, MyColor background, ushort flags)
        {
            Foreground = foreground;
            Background = background;
            Flags = flags;
        }

        public bool Equals(Style other)
        {
            return Foreground == other.Foreground && Background == other.Background && Flags == other.Flags;
        }

        public override bool Equals(object? obj)
        {
            return obj is Style other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int) Foreground, (int) Background, Flags);
        }
    }

    public struct ColoredChar
    {
        public readonly Style Style;
        public readonly char Char;

        public ColoredChar(MyColor foreground, MyColor background, ushort flags, char c)
        {
            Style = new Style(foreground, background, flags);
            Char = c;
        }

        public ColoredChar(MyColor foreground, MyColor background, char c) : this(foreground, background, 0, c)
        {
        }

        public ColoredChar(MyColor foreground, char c) : this(foreground, MyColor.Default, c)
        {
        }

        public ColoredChar(char c) : this(MyColor.Default, c)
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