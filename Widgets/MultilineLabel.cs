using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thuja.Widgets
{
    public class MultilineLabel : AssertRegistered
    {
        private string[] lines;
        private int maxWidth;
        private string text;

        public MultilineLabel(string text, int maxWidth = int.MaxValue)
        {
            this.text = text ?? throw new ArgumentNullException(nameof(text));
            this.maxWidth = maxWidth;
            Recalculate();
        }

        /// <summary>
        ///     Стиль текста.
        /// </summary>
        public Style Style { get; set; } = Style.Default;

        /// <summary>
        ///     Текст.
        /// </summary>
        public string Text
        {
            get => text;
            set
            {
                text = value;
                Recalculate();
            }
        }

        /// <summary>
        ///     Максимальная ширина.
        /// </summary>
        public int MaxWidth
        {
            get => maxWidth;
            set
            {
                maxWidth = value;
                Recalculate();
            }
        }

        /// <inheritdoc />
        public override void Render(RenderContext context)
        {
            for (var i = 0; i < lines.Length; i++)
            {
                context
                    .Derive((0, i, 0))
                    .PlaceString(lines[i], Style);
            }
        }

        /// <summary>
        ///     Разбивает текст на строки, не превышающие максимальную ширину.
        /// </summary>
        private void Recalculate()
        {
            if (text.Length == 0)
            {
                lines = new string[0];
                return;
            }

            var result = new List<string>();
            foreach (var paragraph in text.Split('\n'))
            {
                var words = paragraph.Split();
                var lastLine = new StringBuilder(words[0]);
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
            }

            lines = result.ToArray();
        }
    }
}