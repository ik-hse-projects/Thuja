using System;

namespace Thuja.Widgets
{
    /// <summary>
    /// Простой виджет, отображающий текст. Умеет плавно прокручивать его, если текст длинный.
    /// </summary>
    public class Label : LimitFps, IWidget
    {
        /// <summary>
        /// Индекс первого символа, который сейчас виден.
        /// </summary>
        private int position;
        
        /// <summary>
        /// Сама строка, которую нужно отобразить.
        /// </summary>
        private string text;
        
        /// <summary>
        /// <see cref="text"/>, но к которому в конец приписали разделитель.
        /// </summary>
        private string withSeparator;

        public Label(string text, int maxWidth = int.MaxValue)
        {
            Fps = 2;
            Text = text ?? throw new ArgumentNullException(nameof(text));
            MaxWidth = maxWidth;
        }

        /// <summary>
        /// Строка, которую нужно отображать.
        /// </summary>
        public string Text
        {
            get => text;
            set
            {
                position = 0;
                withSeparator = $"{value}   ";
                text = value;
            }
        }

        /// <summary>
        /// Стиль, с которым отображается текст.
        /// </summary>
        public virtual Style CurrentStyle { get; set; } = Style.Default;

        /// <summary>
        /// Мксимальная ширина, которая будет использоваться для отображения строки.
        /// Если <see cref="Text"/> длиннее, то будет медленно прокручиваться.
        /// </summary>
        public int MaxWidth { get; set; }

        /// <inheritdoc />
        public virtual void Render(RenderContext context)
        {
            // Прокрутка:
            if (IsTimeToDraw())
            {
                position = (position + 1) % withSeparator.Length;
            }

            if (Text.Length == 0)
            {
                context.PlaceString(" ", CurrentStyle);
                return;
            }

            if (MaxWidth >= Text.Length)
            {
                context.PlaceString(Text, CurrentStyle);
                return;
            }

            var scroll = new char[MaxWidth];

            for (var i = 0; i < MaxWidth; i++)
            {
                scroll[i] = withSeparator[(position + i) % withSeparator.Length];
            }

            context.PlaceString(new string(scroll), CurrentStyle);
        }
    }
}