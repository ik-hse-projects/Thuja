using System;

namespace Thuja.Widgets
{
    public class Label: IWidget
    {
        public Label(string text)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));

            Style = Style.Default;
        }
        
        public string Text { get; set; }

        public Style Style { get; set; }
        
        public void Render(RenderContext context)
        {
            context.PlaceString(Text, Style);
        }
    }
}