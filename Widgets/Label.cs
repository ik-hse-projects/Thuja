using System;

namespace Thuja.Widgets
{
    public class Label: IWidget
    {
        public Label(string text)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));

            CurrentStyle = Style.Default;
        }
        
        public string Text { get; set; }

        public Style CurrentStyle { get; set; }
        
        public void Render(RenderContext context)
        {
            context.PlaceString(Text, CurrentStyle);
        }
    }
}