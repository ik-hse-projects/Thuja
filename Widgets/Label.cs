using System;

namespace Thuja.Widgets
{
    public class Label : LimitFps, IWidget
    {
        private int position;
        private string text;
        private string withSeparator;

        public Label(string text, int maxWidth = int.MaxValue)
        {
            Fps = 2;
            Text = text ?? throw new ArgumentNullException(nameof(text));
            MaxWidth = maxWidth;
        }

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

        public virtual Style CurrentStyle { get; set; } = Style.Default;

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