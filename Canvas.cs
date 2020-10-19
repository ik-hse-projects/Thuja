using System;
using System.Collections.Generic;

namespace Thuja
{
    /// <summary>
    /// Объект, который содержит информацию о текущем состоянии экрана.
    /// </summary>
    public class Canvas
    {
        /// <summary>
        /// Содержит все символы, отображающиеся на экране.
        /// </summary>
        public readonly ColoredChar[,] Content;
        
        /// <summary>
        /// Размер холста.
        /// </summary>
        public readonly (int width, int height) Size;

        /// <summary>
        /// Создаёт новый холст указанного размера.
        /// </summary>
        public Canvas(int width, int height)
        {
            Size = (width, height);
            Content = new ColoredChar[width, height];
            Clear();
        }

        /// <summary>
        /// Очищает холст.
        /// </summary>
        public void Clear()
        {
            Clear(new ColoredChar(
                new Style(MyColor.Transparent, MyColor.Transparent),
                ' ',
                int.MinValue
            ));
        }

        /// <summary>
        /// Заполняет весь холст данным символом.
        /// </summary>
        /// <param name="filler">Символ, которым будет заполнен весь холст.</param>
        public void Clear(ColoredChar filler)
        {
            for (var x = 0; x < Size.width; x++)
            for (var y = 0; y < Size.height; y++)
            {
                Content[x, y] = filler;
            }
        }

        /// <summary>
        /// Возвращает символ по указанным координатам, если они не превосходят размер холста. Иначе возвращает null.
        /// </summary>
        public ColoredChar? Get(int x, int y)
        {
            var (width, height) = Size;
            if (x >= width || x < 0 || y >= height || y < 0)
            {
                return null;
            }

            return Content[x, y];
        }

        /// <summary>
        /// Помещает переданный символ на указанные координаты. Правильно обрабатывает прозрачность и следит за слоями.
        /// </summary>
        /// <returns>Возвращает true, если координаты не больше размера холста.</returns>
        public bool TrySet(int x, int y, ColoredChar character)
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
            if (old.Layer > character.Layer)
            {
                return true;
            }

            if (character.Style.Foreground == MyColor.Transparent)
            {
                var style = new Style(old.Style.Foreground, character.Style.Background);
                Content[x, y] = new ColoredChar(style, old.Char, character.Layer);
                return true;
            }

            if (character.Style.Background == MyColor.Transparent)
            {
                var style = new Style(character.Style.Foreground, old.Style.Background);
                Content[x, y] = new ColoredChar(style, character.Char, character.Layer);
                return true;
            }

            Content[x, y] = character;
            return true;
        }

        /// <summary>
        /// Создаёт новый контекст для отрисовки.
        /// </summary>
        public RenderContext BeginRender()
        {
            return new RenderContext((0, 0, 0), this);
        }

        /// <summary>
        /// Вычисляет отличия текущего холста по сравнению с переданным.
        /// </summary>
        /// <param name="other">Холст, отличия с которым нужно найти.</param>
        /// <returns>Возвращает итератор по отличиям.</returns>
        /// <exception cref="ArgumentException">Если другой размер имеет размер отличный от текущего.</exception>
        public IEnumerable<Difference> FindDifferences(Canvas other)
        {
            if (other.Size != Size)
            {
                throw new ArgumentException("Переданный экран имеет не тот размер", nameof(other));
            }

            if (ReferenceEquals(Content, other.Content))
            {
                yield break;
            }

            if (Content.Equals(other.Content))
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
                    var thisChar = Content[x, y];
                    var otherChar = other.Content[x, y];
                    if (thisChar.FlatEquals(otherChar))
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

    /// <summary>
    /// Содержит информацию об отличиях одного холста от другого.
    /// </summary>
    public readonly struct Difference
    {
        /// <summary>
        /// Строка, на которой начинается отличие.
        /// </summary>
        public readonly int Line;
        
        /// <summary>
        /// Колонка, на которой начинается отличие.
        /// </summary>
        public readonly int Column;
        
        /// <summary>
        /// Массив символов на которые они изменилиись.
        /// </summary>
        public readonly List<ColoredChar> Chars;

        /// <summary>
        /// Создаёт отличие без символов по указанным координатам.
        /// </summary>
        public Difference(int line, int column) : this()
        {
            Line = line;
            Column = column;
            Chars = new List<ColoredChar>();
        }
    }
}