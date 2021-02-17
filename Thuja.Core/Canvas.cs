using System;
using System.Collections.Generic;

namespace Thuja
{
    /// <summary>
    ///     Объект, который содержит информацию о текущем состоянии экрана.
    /// </summary>
    public class Canvas
    {
        /// <summary>
        ///     Содержит все символы, отображающиеся на экране.
        /// </summary>
        public readonly ColoredChar[,] Content;

        /// <summary>
        ///     Размер холста.
        /// </summary>
        public readonly Cooridantes Size;

        /// <summary>
        ///     Создаёт новый холст указанного размера.
        /// </summary>
        public Canvas(Cooridantes size)
        {
            Size = size;
            Content = new ColoredChar[size.Width, size.Height];
            Clear();
        }

        /// <summary>
        ///     Очищает холст.
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
        ///     Заполняет весь холст данным символом.
        /// </summary>
        /// <param name="filler">Символ, которым будет заполнен весь холст.</param>
        public void Clear(ColoredChar filler)
        {
            for (var x = 0; x < Size.Width; x++)
            for (var y = 0; y < Size.Height; y++)
            {
                Content[x, y] = filler;
            }
        }

        /// <summary>
        ///     Возвращает символ по указанным координатам, если они не превосходят размер холста. Иначе возвращает null.
        /// </summary>
        public ColoredChar? Get(int x, int y)
        {
            if (x >= Size.Width || x < 0 || y >= Size.Height || y < 0)
            {
                return null;
            }

            return Content[x, y];
        }

        /// <summary>
        ///     Помещает переданный символ на указанные координаты. Правильно обрабатывает прозрачность и следит за слоями.
        /// </summary>
        /// <returns>Возвращает true, если координаты не больше размера холста.</returns>
        public bool TrySet(int x, int y, ColoredChar character)
        {
            if (x >= Size.Width || x < 0 || y >= Size.Height || y < 0)
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
        ///     Создаёт новый контекст для отрисовки.
        /// </summary>
        public RenderContext BeginRender(IRenderer renderer)
        {
            return new RenderContext((0, 0, 0), this, renderer);
        }

        /// <summary>
        ///     Вычисляет отличия текущего холста по сравнению с переданным.
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

            for (var y = 0; y < Size.Height; y++)
            {
                var distanceFromLastDifference = 0;
                const int distanceThreshold = 8;
                Difference? diff = null;
                for (var x = 0; x < Size.Width; x++)
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
                        diff ??= new Difference(new Cooridantes(x, y));
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
    ///     Содержит информацию об отличиях одного холста от другого.
    /// </summary>
    public readonly struct Difference
    {
        /// <summary>
        ///     Координаты буквы, на которой начинается отличие.
        /// </summary>
        public readonly Cooridantes Position;

        /// <summary>
        ///     Массив символов на которые они изменилиись.
        /// </summary>
        public readonly List<ColoredChar> Chars;

        /// <summary>
        ///     Создаёт отличие без символов по указанным координатам.
        /// </summary>
        public Difference(Cooridantes position) : this()
        {
            Position = position;
            Chars = new List<ColoredChar>();
        }
    }
}