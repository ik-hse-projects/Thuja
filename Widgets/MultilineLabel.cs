using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thuja.Widgets
{
    public class MultilineLabel: IWidget
    {
        private string text;
        private int maxWidth;
        private string[] lines;

        public MultilineLabel(string text, int maxWidth = int.MaxValue)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
            MaxWidth = maxWidth;
        }
        
        public Style Style { get; set; } = Style.Default;

        public string Text
        {
            get => text;
            set
            {
                text = value;
                Recalculate();
            }
        }
        
        public int MaxWidth
        {
            get => maxWidth;
            set
            {
                maxWidth = value;
                Recalculate();
            }
        }

        private void Recalculate()
        {
            var words = text.Split();
            if (words.Length == 0)
            {
                lines = new string[0];
                return;
            }
            var lastLine = new StringBuilder(words[0]);
            var result = new List<string>();
            foreach (var word in words.Skip(1))
            {
                var newLength = lastLine.Length + word.Length + 1;
                if (newLength > maxWidth)
                {
                    result.Add(lastLine.ToString());
                    lastLine = new StringBuilder(word);
                }
                else
                {
                    lastLine.Append(' ').Append(word);
                }
            }
            result.Add(lastLine.ToString());
            lines = result.ToArray();
        }
        
        public void Render(RenderContext context)
        {
            for (var i = 0; i < lines.Length; i++)
            {
                context
                    .Derive((0, i, 0))
                    .PlaceString(lines[i], Style);
            }
        }
    }
}