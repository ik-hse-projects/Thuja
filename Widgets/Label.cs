using System;
using System.Diagnostics;

namespace Thuja.Widgets
{
    public class Label: IWidget
    {
        public Label(string text)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));

            CurrentStyle = Style.Default;
        }

        public string Text
        {
            get => text;
            set
            {
                position = 0;
                withSeparator = $"{value} | ";
                text = value;
            }
        }

        public Style CurrentStyle { get; set; }

        public int MaxWidth { get; set; } = 30;

        public int Fps => 2;

        private int position = 0;
        private string text;
        private string withSeparator;

        public void Update(Tick tick)
        {
            position = (position + 1) % withSeparator.Length;
        }

        public virtual void Render(RenderContext context)
        {
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