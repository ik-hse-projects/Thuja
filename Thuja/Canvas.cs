using System;
using System.Collections.Generic;

namespace Thuja
{
    public class Canvas
    {
        public readonly (int width, int height) Size;
        public readonly ColoredChar[,] Content;

        public void Clear(ColoredChar filler)
        {
            for (var x = 0; x < Size.width; x++)
            {
                for (var y = 0; y < Size.height; y++)
                {
                    Content[x, y] = filler;
                }
            }
        }

        public Canvas(int width, int height)
        {
            Size = (width, height);
            Content = new ColoredChar[width, height];
            Clear(ColoredChar.Whitespace);
        }

        private bool TrySet(int x, int y, ColoredChar character)
        {
            var (width, height) = Size;
            if (x >= width || x < 0 || y >= height || y < 0)
            {
                return false;
            }

            if (character.Style.Foreground == MyColor.Transparent &&
                character.Style.Background == MyColor.Transparent)
            {
                return true;
            }

            var old = Content[x, y];
            if (character.Style.Foreground == MyColor.Transparent)
            {
                var style = new Style(old.Style.Foreground, character.Style.Background);
                Content[x, y] = new ColoredChar(style, old.Char);
                return true;
            }

            if (character.Style.Background == MyColor.Transparent)
            {
                var style = new Style(character.Style.Foreground, old.Style.Background);
                Content[x, y] = new ColoredChar(style, character.Char);
                return true;
            }

            Content[x, y] = character;
            return true;
        }

        public void PlaceWidget(IDisplayable widget)
        {
            var (x, y) = widget.Position;
            var rendered = widget.Render();
            for (var i = 0; i < rendered.GetLength(0); i++)
            {
                for (var j = 0; j < rendered.GetLength(1); j++)
                {
                    var ch = rendered[i, j];
                    if (ch != null)
                    {
                        TrySet(x + i, y + j, ch.Value);
                    }
                }
            }
        }

        public IEnumerable<Difference> FindDifferences(Canvas other)
        {
            if (other.Size != Size)
            {
                throw new ArgumentException("Переданный экран имеет не тот размер", nameof(other));
            }

            if (ReferenceEquals(this.Content, other.Content))
            {
                yield break;
            }
            if (this.Content.Equals(other.Content))
            {
                yield break;
            }

            for (var y = 0; y < Size.height; y++)
            {
                var distanceFromLastDifference = 0;
                const int distanceThreshold = 8;
                Difference? diff = null;
                for (var x = 0; x < Size.width; x++)
                {
                    var thisChar = this.Content[x, y];
                    var otherChar = other.Content[x, y];
                    if (thisChar.Equals(otherChar))
                    {
                        distanceFromLastDifference++;
                        if (distanceFromLastDifference > distanceThreshold && diff != null)
                        {
                            diff.Value.Chars.RemoveLast(distanceFromLastDifference - 1);
                            yield return (Difference) diff;
                            diff = null;
                        }
                    }
                    else
                    {
                        distanceFromLastDifference = 0;
                        diff ??= new Difference(y, x);
                    }

                    diff?.Chars.Add(thisChar);
                }

                if (diff != null)
                {
                    yield return (Difference) diff;
                }
            }
        }
    }

    public struct Difference
    {
        public int Line;
        public int Column;
        public List<ColoredChar> Chars;

        public Difference(int line, int column) : this()
        {
            Line = line;
            Column = column;
            Chars = new List<ColoredChar>();
        }
    }
}