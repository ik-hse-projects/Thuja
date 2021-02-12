using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Thuja.Widgets
{
    /// <summary>
    ///     Диапазон разрешённых для ввода символов.
    /// </summary>
    public readonly struct CharRange
    {
        /// <summary>
        ///     Символы, которые допустимы в путях.
        /// </summary>
        public static CharRange FilenameChars = new CharRange(char.MinValue, char.MaxValue,
            Path.GetInvalidFileNameChars()
                .Select(ch => new CharRange(ch))
                .ToHashSet());

        /// <summary>
        ///     Все символы, кроме непечатных ASCII.
        /// </summary>
        public static CharRange Printable = new CharRange(' ', char.MaxValue);

        /// <summary>
        ///     Первый разрешённый символ диапазона.
        /// </summary>
        private readonly char start;

        /// <summary>
        ///     Последний разрешённый символ диапазона.
        /// </summary>
        private readonly char end;

        /// <summary>
        ///     Диапазоны, символы в которых исключены из этого диапазона.
        /// </summary>
        private readonly HashSet<CharRange> denied;

        /// <summary>
        ///     Создаёт диапазон из единственного символа.
        /// </summary>
        private CharRange(char only) : this(only, only)
        {
        }

        /// <summary>
        ///     Создаёт диапазон символов.
        /// </summary>
        private CharRange(char start, char end, HashSet<CharRange>? denied = null)
        {
            if (start > end)
            {
                throw new ArgumentException("start > end");
            }

            this.start = start;
            this.end = end;
            this.denied = denied ?? new HashSet<CharRange>();
        }

        /// <summary>
        ///     Проверяет, входит ли символ в диапазон.
        /// </summary>
        public bool Check(char ch)
        {
            return start <= ch && ch <= end && !denied.Any(i => i.Check(ch));
        }
    }

    /// <summary>
    ///     Текст, который отображается в <see cref="InputField" />, если на нём не стоит фокус.
    /// </summary>
    public readonly struct Placeholder
    {
        /// <summary>
        ///     Стиль текста.
        /// </summary>
        public readonly Style Style;

        /// <summary>
        ///     Текст.
        /// </summary>
        public readonly string Text;

        /// <summary>
        ///     Создаёт <see cref="Placeholder" /> с указанными параметрами.
        /// </summary>
        public Placeholder(Style style, string text)
        {
            Style = style;
            Text = text;
        }
    }

    /// <summary>
    ///     Поле для ввода текста пользователем.
    /// </summary>
    public class InputField : IKeyHandler
    {
        /// <summary>
        ///     Положение курсора.
        /// </summary>
        private int cursorLeft;

        private bool isFocused;

        /// <summary>
        ///     Максимальная длина текста.
        /// </summary>
        public int MaxLength { get; set; } = int.MaxValue;

        /// <summary>
        ///     Символы, разрешённые для ввода.
        /// </summary>
        public HashSet<CharRange> AllowedChars { get; } = new() {CharRange.Printable};

        /// <summary>
        ///     Стиль текста, когда поле сфокусировано.
        /// </summary>
        public Style ActiveStyle { get; set; } = Style.Active;

        /// <summary>
        ///     Стиль текста, когда поле не сфокусировано.
        /// </summary>
        public Style InactiveStyle { get; set; } = Style.Inactive;

        /// <summary>
        ///     Введённый пользователем текст.
        /// </summary>
        public StringBuilder Text { get; } = new StringBuilder();

        /// <summary>
        ///     Текст, который отображается, когда поле не сфокусировано и ничего ещё не введено.
        /// </summary>
        public Placeholder? Placeholder { get; set; }

        /// <summary>
        ///     Устанавливает положение курсора, совершая все необходимые проверки.
        /// </summary>
        private int CursorLeft
        {
            get => cursorLeft;
            set
            {
                if (value < 0)
                {
                    cursorLeft = 0;
                }
                else if (value > Text.Length)
                {
                    cursorLeft = Text.Length;
                }
                else
                {
                    cursorLeft = value;
                }
            }
        }

        /// <inheritdoc />
        public Dictionary<HashSet<KeySelector>, Action> Actions { get; } = new();

        /// <inheritdoc />
        public void Render(RenderContext context)
        {
            if (isFocused)
            {
                context.CursorPosition = (cursorLeft, 0);
            }

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

            context.PlaceString(text, style);
        }

        /// <inheritdoc />
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
            if (ch != '\0' && (AllowedChars.Count == 0 || AllowedChars.Any(range => range.Check(ch))))
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

        /// <inheritdoc />
        public void FocusChange(bool isFocused)
        {
            this.isFocused = isFocused;
        }

        /// <summary>
        ///     Преобразовывает этот контейнер в экземпляр <see cref="IKeyHandler" />
        /// </summary>
        /// <returns>Объект типа <see cref="IKeyHandler" />, который может быть преобразован в <see cref="InputField" />.</returns>
        public IKeyHandler AsIKeyHandler()
        {
            return this;
        }

        /// <summary>
        ///     Удаляет один символ позади курсора.
        /// </summary>
        private void Del()
        {
            if (cursorLeft < Text.Length)
            {
                Text.Remove(cursorLeft, 1);
            }
        }
    }
}