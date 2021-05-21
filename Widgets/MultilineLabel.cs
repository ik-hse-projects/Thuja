using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thuja.Widgets
{
    /// <summary>
    /// Аналог <see cref="Label"/>, но поддерживающий несколько строк.
    /// Умеет правильно расставлять переносы где необходимо.
    /// </summary>
    /// 
    public class MultilineLabel : IWidget
    {
        /// <summary>
        /// Строки, не превышающие <see cref="maxWidth"/>.
        /// </summary>
        private string[] lines;

        /// <summary>
        /// Максимальная длина каждой строки.
        /// </summary>
        private int maxWidth;

        /// <summary>
        /// Текст, который отображается.
        /// </summary>
        private string text;

        public MultilineLabel(string text, int maxWidth = int.MaxValue)
        {
            this.text = text ?? throw new ArgumentNullException(nameof(text));
            this.maxWidth = maxWidth;
            Recalculate();
        }

        /// <summary>
        /// Стиль текста.
        /// </summary>
        public Style Style { get; set; } = Style.Default;

        /// <summary>
        /// Текст.
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
        /// Максимальная ширина.
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
        public void Render(RenderContext context)
        {
            for (var i = 0; i < lines.Length; i++)
            {
                context
                    .Derive((0, i, 0))
                    .PlaceString(lines[i], Style);
            }
        }

        /// <summary>
        /// Разбивает текст на строки, не превышающие максимальную ширину.
        /// </summary>
        private void Recalculate()
        {
            if (text.Length == 0)
            {
                lines = new string[0];
                return;
            }

            var result = new List<string>();

            // Перебираем все абзацы. \n — маркер того, что здесь точно нужно сделать перенос строки.
            foreach (var paragraph in text.Split('\n'))
            {
                var words = paragraph.Split();

                // Первое слово всегда помещается на строку. Даже если оно силишком длинное.
                var lastLine = new StringBuilder(words[0]);

                // Пытаемся добавлять слова в lastLine, но только до тех пор, пока lastLine не станет слишком длинной.
                foreach (var word in words.Skip(1))
                {
                    // Слово ещё влезает?
                    var newLength = lastLine.Length + word.Length + 1;
                    if (newLength > maxWidth)
                    {
                        // Если нет, то добавляем в результат новую строку, а это слово пенеосим на следующую.
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