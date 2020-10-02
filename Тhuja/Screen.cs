using System;
using System.Collections.Generic;

namespace Тhuja
{
    public class Screen
    {
        private readonly (int width, int height) Size;
        private char[,] Content { get; }

        public Screen(char[,] content)
        {
            Content = content;
            Size = (content.GetLength(0), content.GetLength(1));
        }

        public static Screen Cleared(int width, int height)
        {
            var content = new char[width, height];
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    content[x, y] = ' ';
                }
            }

            return new Screen(content);
        }

        private bool TrySet(int x, int y, char character)
        {
            var (width, height) = Size;
            var isOk = x < width && x >= 0 && y < height && y >= 0;
            if (isOk)
            {
                Content[x, y] = character;
            }

            return isOk;
        }

        public void PlaceWidget(int x, int y, IDisplayable widget)
        {
            var rendered = widget.Render();
            for (var i = 0; i < rendered.GetLength(0); i++)
            {
                for (var j = 0; i < rendered.GetLength(1); j++)
                {
                    var ch = rendered[i, j];
                    if (ch != null)
                    {
                        TrySet(x + i, y + j, ch.Value);
                    }
                }
            }
        }

        public IEnumerable<Difference> FindDifferences(Screen other)
        {
            if (other.Size != Size)
            {
                throw new ArgumentException("Переданный экран имеет не тот размер", nameof(other));
            }

            for (var y = 0; y < Size.height; y++)
            {
                var distanceFromLastDifference = 0;
                const int distanceThreshold = 8;
                Difference? diff = null;
                for (var x = 0; x < Size.width; x++)
                {
                    var thisChar = Content[x, y];
                    var otherChar = other.Content[x, y];
                    if (thisChar != otherChar)
                    {
                        distanceFromLastDifference = 0;
                        diff ??= new Difference(y, x);
                    }
                    else
                    {
                        distanceFromLastDifference++;
                        if (distanceFromLastDifference > distanceThreshold && diff != null)
                        {
                            diff.Value.Chars.RemoveLast(distanceFromLastDifference);
                            yield return (Difference) diff;
                            diff = null;
                        }
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
        public List<char> Chars;

        public Difference(int line, int column) : this()
        {
            Line = line;
            Column = column;
            Chars = new List<char>();
        }
    }
}