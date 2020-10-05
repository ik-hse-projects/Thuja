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
        public readonly int Layer;

        public ColoredChar(Style style, char c, int layer)
        {
            Style = style;
            Char = c;
            Layer = layer;
        }
        
        public ColoredChar(Style style, char c): this(style, c, 0)
        {
        }
        
        public ColoredChar(char c) : this(new Style(MyColor.Default, MyColor.Default), c)
        {
        }

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