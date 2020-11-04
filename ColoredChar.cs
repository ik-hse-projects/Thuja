using System;

namespace Thuja
{
    /// <summary>
    ///     Стиль символа.
    /// </summary>
    public readonly struct Style
    {
        /// <summary>
        ///     Стиль по-умолчанию.
        /// </summary>
        public static Style Default = new Style(MyColor.Default, MyColor.Default);

        /// <summary>
        ///     Стиль активных элементов.
        /// </summary>
        public static Style Active = new Style(MyColor.Black, MyColor.White);

        /// <summary>
        ///     Стиль неактивных элементов.
        /// </summary>
        public static Style Inactive = new Style(MyColor.Gray, MyColor.DarkGray);

        /// <summary>
        ///     Стиль декоративных элементов.
        /// </summary>
        public static Style Decoration = new Style(MyColor.DarkGray, MyColor.Black);

        /// <summary>
        ///     Тёмносерый цвет на фоне по-умолчанию.
        /// </summary>
        public static Style DarkGrayOnDefault = new Style(MyColor.DarkGray, MyColor.Default);

        /// <summary>
        ///     Цвет текста.
        /// </summary>
        public readonly MyColor Foreground;

        /// <summary>
        ///     Цвет фона.
        /// </summary>
        public readonly MyColor Background;

        /// <summary>
        ///     Создаёт новый стиль с указанными цветами.
        /// </summary>
        public Style(MyColor foreground, MyColor background)
        {
            Foreground = foreground;
            Background = background;
        }

        /// <summary>
        ///     Сравнивает два цвета на равенство.
        /// </summary>
        /// <param name="other">Стиль, с которым нужно сравнить текущий.</param>
        /// <returns>true, если они одинаковы.</returns>
        public bool Equals(Style other)
        {
            return Foreground == other.Foreground && Background == other.Background;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is Style other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine((int) Foreground, (int) Background);
        }
    }

    /// <summary>
    ///     Символ, который также содержит его стиль и слой.
    /// </summary>
    public readonly struct ColoredChar
    {
        /// <summary>
        ///     Стиль, с которым должен быть отображен символ.
        /// </summary>
        public readonly Style Style;

        /// <summary>
        ///     Сам символ.
        /// </summary>
        public readonly char Char;

        /// <summary>
        ///     Слой, на котором будет отображен символ.
        ///     Используется для выбора отображаемого символа, если одновременно несколько
        ///     находятся на одних и тех же координтатх. Выше слой — выше приоритет.
        /// </summary>
        public readonly int Layer;

        /// <summary>
        ///     Создает новый символ с указанными параметрами.
        /// </summary>
        public ColoredChar(Style style, char c, int layer = 0)
        {
            Style = style;
            Char = c;
            Layer = layer;
        }

        /// <summary>
        ///     Сравнивает два символа, независимо от их слоёв.
        /// </summary>
        /// <param name="other">Символ, на равенство с которым необходимо сравнить этот.</param>
        /// <returns>true, если они одинаковы.</returns>
        public bool FlatEquals(ColoredChar other)
        {
            return Style.Equals(other.Style) && Char == other.Char;
        }

        /// <summary>
        ///     Сравнивает два символа.
        /// </summary>
        /// <param name="other">Символ, с которым нужно сравнить текущий.</param>
        /// <returns>true, если они одинаковы.</returns>
        public bool Equals(ColoredChar other)
        {
            return FlatEquals(other) && Layer == other.Layer;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is ColoredChar other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(Style, Char, Layer);
        }
    }
}